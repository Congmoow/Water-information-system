Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$runner = Join-Path $PSScriptRoot "run_logged.py"
& py -3 $runner --archive-only
exit $LASTEXITCODE
