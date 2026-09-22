#!/bin/pwsh
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

$venvPython = Join-Path $PSScriptRoot ".venv-docs\Scripts\python.exe"

if (-not (Test-Path $venvPython)) {
    Write-Host "Creating docs venv at .venv-docs ..."
    # Prefer the "py" launcher: plain "python" can resolve to the Microsoft
    # Store alias stub, which exists as a command but doesn't run Python.
    if (Get-Command py -ErrorAction SilentlyContinue) {
        py -m venv "$PSScriptRoot\.venv-docs"
    } else {
        python -m venv "$PSScriptRoot\.venv-docs"
    }
    if (-not (Test-Path $venvPython)) {
        throw "Couldn't create the venv. Make sure Python 3 is installed and on PATH."
    }
}

& $venvPython -m pip install --quiet -r ../requirements-docs.txt

if ($Serve) {
    & $venvPython -m mkdocs serve -f ../.mkdocs/mkdocs.yml
} else {
    & $venvPython -m mkdocs build --strict -f ../.mkdocs/mkdocs.yml
    Write-Host "`nBuilt to ../perflab-site (see site_dir in .mkdocs/mkdocs.yml)."
}
