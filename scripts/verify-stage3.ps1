param(
    [string]$LogName = "verify-stage3",
    [switch]$ArchiveRootLogs,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$ExtraArgs
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$runner = Join-Path $PSScriptRoot "run_logged.py"
$scriptPath = Join-Path $PSScriptRoot "verify_stage3.py"

$args = @("-3", $runner, "--name", $LogName)
if ($ArchiveRootLogs) {
    $args += "--archive-root-logs"
}

$args += "--"
$args += "py"
$args += "-3"
$args += $scriptPath
if ($ExtraArgs) {
    $args += $ExtraArgs
}

& py @args
exit $LASTEXITCODE
