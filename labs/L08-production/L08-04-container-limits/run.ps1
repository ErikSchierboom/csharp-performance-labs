#!/usr/bin/env pwsh
<#
.SYNOPSIS
Runs the service in a Linux container under a memory limit (docker or podman). Works on Windows, macOS
and Linux: the limit is a cgroup applied by the container runtime, so it behaves the same everywhere.
The image is built from ./Dockerfile on first use (layers are cached afterwards).

.PARAMETER Limit
Memory limit, e.g. "220M", or "none" for unlimited.

.PARAMETER EnvVars
Extra environment variables for the run, e.g. "DOTNET_GCHeapHardLimit=0xC800000".

.EXAMPLE
./run.ps1 none
./run.ps1 220M DOTNET_GCHeapHardLimit=0xC800000
#>
param(
    [string]$Limit = "none",

    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$EnvVars = @()
)

Set-Location $PSScriptRoot

$engine = @("docker", "podman") | Where-Object { Get-Command $_ -ErrorAction SilentlyContinue } | Select-Object -First 1
if (-not $engine) {
    Write-Host "Neither docker nor podman found. Install Docker Desktop (Windows/macOS) or Docker/Podman (Linux)."
    exit 1
}

$image = "perflab-l08-04"
& $engine build -q -t $image . | Out-Null
if ($LASTEXITCODE -ne 0) { Write-Host "image build failed"; exit $LASTEXITCODE }

$runArgs = @("run", "--rm")
if ($Limit -ne "none") {
    # Limit RAM and disallow swap (memory-swap == memory), like MemorySwapMax=0 in the old systemd version.
    $runArgs += @("--memory=$Limit", "--memory-swap=$Limit")
}
foreach ($pair in $EnvVars) { $runArgs += @("-e", $pair) }
$runArgs += $image

& $engine @runArgs
Write-Host "exit code: $LASTEXITCODE"
exit $LASTEXITCODE
