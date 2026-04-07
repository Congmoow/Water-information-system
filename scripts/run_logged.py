from __future__ import annotations

import argparse
import re
import shutil
import subprocess
import sys
import threading
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path
from typing import Iterable, Sequence, TextIO


LOG_FILE_SUFFIXES = (".log", ".out.log", ".err.log")


@dataclass
class CommandResult:
    returncode: int
    stdout_log: Path
    stderr_log: Path


def repository_root() -> Path:
    return Path(__file__).resolve().parent.parent


def sanitize_log_name(log_name: str) -> str:
    cleaned = re.sub(r"[^\w.-]+", "-", log_name.strip())
    cleaned = cleaned.strip(".-")
    if not cleaned:
        raise ValueError("log name cannot be empty")
    return cleaned


def logs_dir(root_dir: Path) -> Path:
    directory = root_dir / "logs"
    directory.mkdir(parents=True, exist_ok=True)
    return directory


def build_log_paths(root_dir: Path, log_name: str) -> tuple[Path, Path]:
    safe_name = sanitize_log_name(log_name)
    directory = logs_dir(root_dir)
    return directory / f"{safe_name}.out.log", directory / f"{safe_name}.err.log"


def iter_root_log_files(root_dir: Path) -> Iterable[Path]:
    for path in root_dir.iterdir():
        if not path.is_file():
            continue
        if path.name.endswith(LOG_FILE_SUFFIXES):
            yield path


def _make_archive_dir(base_dir: Path) -> Path:
    timestamp = datetime.now().strftime("%Y%m%d-%H%M%S")
    candidate = base_dir / timestamp
    counter = 1
    while candidate.exists():
        candidate = base_dir / f"{timestamp}-{counter}"
        counter += 1
    candidate.mkdir(parents=True, exist_ok=False)
    return candidate


def archive_root_logs(root_dir: Path) -> Path | None:
    files = list(iter_root_log_files(root_dir))
    if not files:
        return None

    archive_base = logs_dir(root_dir) / "archive"
    archive_dir = _make_archive_dir(archive_base)
    for path in files:
        shutil.move(str(path), str(archive_dir / path.name))
    return archive_dir


def _write_to_sink(sink: TextIO, content: str) -> None:
    try:
        sink.write(content)
        sink.flush()
    except UnicodeEncodeError:
        if hasattr(sink, "buffer"):
            sink.buffer.write(content.encode(getattr(sink, "encoding", None) or "utf-8", errors="replace"))
            sink.buffer.flush()
        else:
            sink.write(content.encode("utf-8", errors="replace").decode("utf-8"))
            sink.flush()


def _pump_stream(stream: TextIO | None, sink: TextIO, log_file: TextIO) -> None:
    if stream is None:
        return

    try:
        for chunk in iter(stream.readline, ""):
            if not chunk:
                break
            log_file.write(chunk)
            log_file.flush()
            _write_to_sink(sink, chunk)
    finally:
        stream.close()


def run_command(
    *,
    root_dir: Path,
    log_name: str,
    command: Sequence[str],
    cwd: Path | None = None,
    archive_existing_root_logs: bool = False,
) -> CommandResult:
    if archive_existing_root_logs:
        archive_root_logs(root_dir)

    stdout_log, stderr_log = build_log_paths(root_dir, log_name)

    with stdout_log.open("w", encoding="utf-8", newline="") as stdout_file, stderr_log.open(
        "w", encoding="utf-8", newline=""
    ) as stderr_file:
        process = subprocess.Popen(
            list(command),
            cwd=str(cwd or root_dir),
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            encoding="utf-8",
            errors="replace",
        )

        stdout_thread = threading.Thread(target=_pump_stream, args=(process.stdout, sys.stdout, stdout_file))
        stderr_thread = threading.Thread(target=_pump_stream, args=(process.stderr, sys.stderr, stderr_file))
        stdout_thread.start()
        stderr_thread.start()

        returncode = process.wait()
        stdout_thread.join()
        stderr_thread.join()

    return CommandResult(returncode=returncode, stdout_log=stdout_log, stderr_log=stderr_log)


def parse_args(argv: Sequence[str] | None = None) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Run a command and save stdout/stderr into the repository logs directory.")
    parser.add_argument("--name", help="Base name for the generated log files.")
    parser.add_argument("--cwd", help="Working directory for the wrapped command.")
    parser.add_argument(
        "--archive-root-logs",
        action="store_true",
        help="Move existing root-level log files into logs/archive before running the command.",
    )
    parser.add_argument(
        "--archive-only",
        action="store_true",
        help="Archive existing root-level log files and exit without running a command.",
    )
    parser.add_argument("command", nargs=argparse.REMAINDER, help="Command to run after '--'.")
    args = parser.parse_args(argv)

    if not args.archive_only and not args.name:
        parser.error("--name is required unless --archive-only is used")

    return args


def main(argv: Sequence[str] | None = None) -> int:
    args = parse_args(argv)
    root_dir = repository_root()

    if args.archive_only:
        archive_dir = archive_root_logs(root_dir)
        if archive_dir is None:
            print("No root log files found.")
        else:
            print(f"Archived root log files to {archive_dir}")
        return 0

    command = list(args.command)
    if command and command[0] == "--":
        command = command[1:]
    if not command:
        raise SystemExit("a command is required after '--'")

    cwd = Path(args.cwd).resolve() if args.cwd else root_dir

    try:
        result = run_command(
            root_dir=root_dir,
            log_name=args.name,
            command=command,
            cwd=cwd,
            archive_existing_root_logs=args.archive_root_logs,
        )
    except FileNotFoundError as exc:
        print(f"Command not found: {exc}", file=sys.stderr)
        return 1

    print(f"stdout log: {result.stdout_log}")
    print(f"stderr log: {result.stderr_log}")
    return result.returncode


if __name__ == "__main__":
    raise SystemExit(main())
