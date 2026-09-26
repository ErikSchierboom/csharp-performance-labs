<#
.SYNOPSIS
Publishes every lab's mystery service to levels/L08-production/<lab>/bin. Run once after cloning.
#>
$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

Get-ChildItem -Path (Join-Path $PSScriptRoot "L08-0[1-4]-*") -Directory | Sort-Object Name | ForEach-Object {
    Write-Host "== $($_.Name)"
    & dotnet publish -c Release -o (Join-Path $_.FullName "bin") (Join-Path $_.FullName "src") -v q | Select-Object -Last 1
}
