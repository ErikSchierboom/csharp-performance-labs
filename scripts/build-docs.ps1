#!/usr/bin/env pwsh
<#
.SYNOPSIS
Builds the docs site (MkDocs). Creates/reuses a local venv, so the first run is
slower than the rest.

.PARAMETER Serve
Serve locally with live-reload instead of a one-shot build.

.EXAMPLE
./build-docs.ps1
./build-docs.ps1 -Serve
#>
param(
    [switch]$Serve
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$venvDir = Join-Path $PSScriptRoot ".venv-docs"
# A venv puts its interpreter in Scripts\python.exe on Windows and bin/python elsewhere.
# ($IsWindows doesn't exist in Windows PowerShell 5.1, hence the PSEdition check.)
$onWindows = $IsWindows -or $PSVersionTable.PSEdition -eq "Desktop"
$venvPython = if ($onWindows) { Join-Path $venvDir "Scripts/python.exe" } else { Join-Path $venvDir "bin/python" }

if (-not (Test-Path $venvPython)) {
    Write-Host "Creating docs venv at .venv-docs ..."
    # On Windows prefer the "py" launcher: plain "python" can resolve to the Microsoft
    # Store alias stub, which exists as a command but doesn't run Python.
    # On Linux/macOS the interpreter is usually "python3"; "python" may not exist.
    $candidates = if ($onWindows) { "py", "python" } else { "python3", "python" }
    $python = $candidates | Where-Object { Get-Command $_ -ErrorAction SilentlyContinue } | Select-Object -First 1
    if (-not $python) { throw "Couldn't find Python ($($candidates -join ' or ')). Make sure Python 3 is installed and on PATH." }
    & $python -m venv $venvDir
    if (-not (Test-Path $venvPython)) {
        throw "Couldn't create the venv. Make sure Python 3 is installed and on PATH (on Debian/Ubuntu you may need the python3-venv package)."
    }
}

& $venvPython -m pip install --quiet -r ../requirements-docs.txt

if ($Serve) {
    & $venvPython -m mkdocs serve -f ../.mkdocs/mkdocs.yml
} else {
    & $venvPython -m mkdocs build --strict -f ../.mkdocs/mkdocs.yml
    Write-Host "`nBuilt to ../perflab-site (see site_dir in .mkdocs/mkdocs.yml)."
}
