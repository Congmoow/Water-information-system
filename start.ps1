param(
    [switch]$SkipDependencyInstall
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Join-Path $repoRoot "backend\src\WaterInfoSystem.API"
$frontendDir = Join-Path $repoRoot "frontend\water-info-web"

if (-not (Test-Path $backendDir)) {
    throw "Backend project path not found: $backendDir"
}

if (-not (Test-Path $frontendDir)) {
    throw "Frontend project path not found: $frontendDir"
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet command not found. Please install .NET 8 SDK first."
}

if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    throw "npm command not found. Please install Node.js first."
}

$backendUrl = "http://localhost:5000"
$frontendUrl = "http://localhost:5173"
$scalarUrl = "$backendUrl/scalar/v1"
$openApiUrl = "$backendUrl/openapi/v1.json"

Write-Host ""
Write-Host "Water Information System demo startup"
Write-Host "------------------------------------"
Write-Host "Please make sure SQL Server is running and the database connection in backend/src/WaterInfoSystem.API/appsettings.json is available."
Write-Host "Backend directory : $backendDir"
Write-Host "Frontend directory: $frontendDir"
Write-Host ""

$backendCommand = @(
    "Set-Location -LiteralPath '$backendDir'",
    "Write-Host 'Starting backend on $backendUrl ...'",
    "`$env:ASPNETCORE_URLS = '$backendUrl'",
    "dotnet run --no-launch-profile"
) -join "; "

$frontendInstall = if ($SkipDependencyInstall) {
    "Write-Host 'Skip dependency install check.'"
}
else {
    "if (-not (Test-Path 'node_modules')) { Write-Host 'node_modules not found, running npm install ...'; npm install }"
}

$frontendCommand = @(
    "Set-Location -LiteralPath '$frontendDir'",
    $frontendInstall,
    "Write-Host 'Starting frontend on $frontendUrl ...'",
    "npm run dev"
) -join "; "

Start-Process -FilePath "powershell.exe" -ArgumentList @("-NoExit", "-Command", $backendCommand) | Out-Null
Start-Sleep -Seconds 2
Start-Process -FilePath "powershell.exe" -ArgumentList @("-NoExit", "-Command", $frontendCommand) | Out-Null

Write-Host "Startup windows opened."
Write-Host "Frontend : $frontendUrl"
Write-Host "Backend  : $backendUrl"
Write-Host "Scalar   : $scalarUrl"
Write-Host "OpenAPI  : $openApiUrl"
Write-Host ""
Write-Host "If the database is empty, you can either:"
Write-Host "1. Execute sql/init/001_create_database.sql -> 002_create_tables.sql -> 003_seed_data.sql"
Write-Host "2. Or start the backend against an empty database and let EnsureCreated + DataSeeder initialize demo data automatically"
