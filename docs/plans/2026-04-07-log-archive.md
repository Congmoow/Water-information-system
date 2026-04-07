# Root Log Archive Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Move root-level command log files into a dedicated `logs/` directory and provide repo-local entry points that write future command logs there by default.

**Architecture:** Add a small Python command runner in `scripts/` that executes arbitrary commands and writes stdout/stderr to `logs/<name>.out.log` and `logs/<name>.err.log`. Wrap the known backend, frontend, and verification workflows with PowerShell scripts that route through the runner, and add an archive step that moves existing root logs into `logs/archive/`.

**Tech Stack:** Python 3, PowerShell 5+, .gitignore

---

### Task 1: Add a failing test for the reusable log runner

**Files:**
- Create: `scripts/tests/test_run_logged.py`
- Create: `scripts/tests/__init__.py`
- Test: `scripts/tests/test_run_logged.py`

**Step 1: Write the failing test**

Add tests that assert:
- the runner creates `logs/<name>.out.log` and `logs/<name>.err.log`
- stdout and stderr content are written to the correct files
- root-level `*.log`, `*.out.log`, and `*.err.log` files can be archived into `logs/archive/<timestamp>/`

**Step 2: Run test to verify it fails**

Run: `python -m unittest scripts.tests.test_run_logged -v`
Expected: fail because `scripts/run_logged.py` does not exist yet

### Task 2: Implement the minimal reusable log runner

**Files:**
- Create: `scripts/run_logged.py`
- Test: `scripts/tests/test_run_logged.py`

**Step 1: Write minimal implementation**

Implement:
- log path helpers rooted at the repository `logs/` directory
- a subprocess runner that records stdout/stderr to separate files
- an archive helper that moves existing root log files into `logs/archive/<timestamp>/`
- a CLI surface: `python scripts/run_logged.py --name <name> [--cwd <dir>] [--archive-root-logs] -- <command ...>`

**Step 2: Run test to verify it passes**

Run: `python -m unittest scripts.tests.test_run_logged -v`
Expected: pass

### Task 3: Add PowerShell wrappers for common workflows

**Files:**
- Create: `scripts/archive-root-logs.ps1`
- Create: `scripts/run-backend.ps1`
- Create: `scripts/run-frontend.ps1`
- Create: `scripts/verify-stage3.ps1`
- Create: `scripts/verify-stage456.ps1`

**Step 1: Add wrapper scripts**

Add wrappers that call `scripts/run_logged.py` so the common workflows write to `logs/` by default:
- backend API run
- frontend Vite dev server
- stage 3 verification
- stage 4/5/6 verification
- standalone archive command for existing root logs

**Step 2: Keep wrapper behavior narrow**

Only cover the known repo workflows and pass through optional log names and extra command arguments when helpful.

### Task 4: Update ignore rules and archive existing root logs

**Files:**
- Modify: `.gitignore`

**Step 1: Update ignore rules**

Make the dedicated `logs/` directory an explicit ignored artifact location.

**Step 2: Archive the current root log files**

Run: `powershell -ExecutionPolicy Bypass -File scripts/archive-root-logs.ps1`
Expected: current root `*.log`, `*.out.log`, `*.err.log` files move under `logs/archive/<timestamp>/`

### Task 5: Verify the new workflow end-to-end

**Files:**
- Verify: `scripts/run_logged.py`
- Verify: `scripts/run-backend.ps1`
- Verify: `scripts/run-frontend.ps1`
- Verify: `logs/`

**Step 1: Run automated tests**

Run: `python -m unittest scripts.tests.test_run_logged -v`
Expected: pass

**Step 2: Run a command through the new runner**

Run: `python scripts/run_logged.py --name smoke-test -- python -c "import sys; print('out'); print('err', file=sys.stderr)"`
Expected: `logs/smoke-test.out.log` and `logs/smoke-test.err.log` are created

**Step 3: Verify root stays clean**

Run: `Get-ChildItem -File *.log,*.out.log,*.err.log`
Expected: no new matching files created in the repository root
