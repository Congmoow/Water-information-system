param(
    [string]$LogName = "backend-run",
    [switch]$ArchiveRootLogs,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ExtraArgs
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$runner = Join-Path $PSScriptRoot "run_logged.py"
$apiDir = Join-Path $repoRoot "backend\src\WaterInfoSystem.API"

$args = @("-3", $runner, "--name", $LogName, "--cwd", $apiDir)
if ($ArchiveRootLogs) {
    $args += "--archive-root-logs"
}

$args += "--"
$args += "dotnet"
$args += "run"
if ($ExtraArgs) {
    $args += $ExtraArgs
}

& py @args
exit $LASTEXITCODE
