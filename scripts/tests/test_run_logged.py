import sys
import tempfile
import unittest
from pathlib import Path

from scripts.run_logged import archive_root_logs, run_command


class RunLoggedTests(unittest.TestCase):
    def test_run_command_writes_stdout_and_stderr_to_logs_directory(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            result = run_command(
                root_dir=root,
                log_name="sample",
                command=[
                    sys.executable,
                    "-c",
                    "import sys; print('hello stdout'); print('hello stderr', file=sys.stderr)",
                ],
            )

            self.assertEqual(result.returncode, 0)
            self.assertTrue((root / "logs" / "sample.out.log").exists())
            self.assertTrue((root / "logs" / "sample.err.log").exists())
            self.assertEqual((root / "logs" / "sample.out.log").read_text(encoding="utf-8").strip(), "hello stdout")
            self.assertEqual((root / "logs" / "sample.err.log").read_text(encoding="utf-8").strip(), "hello stderr")

    def test_archive_root_logs_moves_only_root_log_files(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            (root / "backend-run.log").write_text("backend", encoding="utf-8")
            (root / "frontend-stage3.out.log").write_text("frontend", encoding="utf-8")
            (root / "frontend-stage3.err.log").write_text("stderr", encoding="utf-8")
            (root / "notes.txt").write_text("keep", encoding="utf-8")
            (root / "logs").mkdir()
            (root / "logs" / "existing.log").write_text("keep-in-place", encoding="utf-8")

            archive_dir = archive_root_logs(root)

            self.assertTrue((archive_dir / "backend-run.log").exists())
            self.assertTrue((archive_dir / "frontend-stage3.out.log").exists())
            self.assertTrue((archive_dir / "frontend-stage3.err.log").exists())
            self.assertFalse((root / "backend-run.log").exists())
            self.assertFalse((root / "frontend-stage3.out.log").exists())
            self.assertFalse((root / "frontend-stage3.err.log").exists())
            self.assertTrue((root / "notes.txt").exists())
            self.assertTrue((root / "logs" / "existing.log").exists())


if __name__ == "__main__":
    unittest.main()
