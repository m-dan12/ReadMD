# Скрипт сборки установщика ReadMD с Velopack

param(
    [string]$Version = "1.0.0"
)

Write-Host "Building ReadMD installer v$Version..." -ForegroundColor Cyan

# Сборка проекта
Write-Host "Building project..." -ForegroundColor Yellow
dotnet publish ReadMD/ReadMD.csproj -c Release -r win-x64 --self-contained -o publish/win-x64

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Упаковка с Velopack
Write-Host "Packing with Velopack..." -ForegroundColor Yellow
vpk pack `
    --packId ReadMD `
    --packVersion $Version `
    --packDir publish/win-x64 `
    --mainExe ReadMD.exe `
    --icon ReadMD/Assets/ReadMD-icon.ico `
    --shortcuts Desktop,StartMenu `
    --outputDir releases

if ($LASTEXITCODE -ne 0) {
    Write-Host "Packaging failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Installer created successfully in ./releases/" -ForegroundColor Green
