param(
    [string]$LogName = "frontend-run",
    [switch]$ArchiveRootLogs,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ExtraArgs
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$runner = Join-Path $PSScriptRoot "run_logged.py"
$frontendDir = Join-Path $repoRoot "frontend\water-info-web"

$args = @("-3", $runner, "--name", $LogName, "--cwd", $frontendDir)
if ($ArchiveRootLogs) {
    $args += "--archive-root-logs"
}

$args += "--"
$args += "npm.cmd"
$args += "run"
$args += "dev"
if ($ExtraArgs) {
    $args += "--"
    $args += $ExtraArgs
}

& py @args
exit $LASTEXITCODE
