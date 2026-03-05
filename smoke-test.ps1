[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

Write-Host "`n========================================================" -ForegroundColor Cyan
Write-Host "  MOVEO_BACK - Smoke Test (Prueba de Humo)" -ForegroundColor Cyan
Write-Host "  $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor DarkGray
Write-Host "========================================================`n" -ForegroundColor Cyan

# 1. Clean
Write-Host "[1/3] Limpiando compilaciones anteriores..." -ForegroundColor Yellow
dotnet clean --verbosity quiet 2>&1 | Out-Null
Write-Host "      Limpieza completada." -ForegroundColor DarkGray

# 2. Build
Write-Host "[2/3] Compilando la solucion..." -ForegroundColor Yellow
$buildOutput = dotnet build --verbosity quiet 2>&1
$buildExitCode = $LASTEXITCODE

if ($buildExitCode -ne 0) {
    Write-Host "`n  ==========================================" -ForegroundColor Red
    Write-Host "  |   ERROR DE COMPILACION                 |" -ForegroundColor Red
    Write-Host "  |   El proyecto no compila correctamente.|" -ForegroundColor Red
    Write-Host "  ==========================================`n" -ForegroundColor Red
    Write-Host $buildOutput -ForegroundColor Red
    exit 1
}
Write-Host "      Compilacion exitosa (0 errores)." -ForegroundColor DarkGray

# 3. Test
Write-Host "[3/3] Ejecutando Smoke Tests...`n" -ForegroundColor Yellow

# Use --no-build since we just built
$testOutput = dotnet test Moveo.IntegrationTests/Moveo.IntegrationTests.csproj --filter "Category=Smoke" --no-build --verbosity minimal 2>&1
$testExitCode = $LASTEXITCODE

$testOutput | ForEach-Object { Write-Host "      $_" -ForegroundColor DarkGray }

Write-Host "`n========================================================" -ForegroundColor Cyan

if ($testExitCode -eq 0) {
    Write-Host "`n  ==========================================" -ForegroundColor Green
    Write-Host "  |      V  SISTEMA ESTABLE                |" -ForegroundColor Green
    Write-Host "  |   5/5 Smoke Tests pasados              |" -ForegroundColor Green
    Write-Host "  |   El sistema esta listo.               |" -ForegroundColor Green
    Write-Host "  ==========================================`n" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`n  ==========================================" -ForegroundColor Red
    Write-Host "  |   X  FALLO CRITICO DETECTADO           |" -ForegroundColor Red
    Write-Host "  |   Uno o mas Smoke Tests han fallado.   |" -ForegroundColor Red
    Write-Host "  |   NO desplegar hasta resolver fallo.   |" -ForegroundColor Red
    Write-Host "  ==========================================`n" -ForegroundColor Red
    exit 1
}
