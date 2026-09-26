<#
.SYNOPSIS
Runs the service under a cgroup memory limit. Requires Linux + systemd (systemd-run --user needs the
memory controller delegated to your user; standard on Fedora). The memory limit itself is a real Linux
cgroups feature; on Windows/macOS only the unlimited ("none") run works natively. Run this lab under
WSL2 or a Linux container for the limited runs.

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

if ($Limit -eq "none") {
    foreach ($pair in $EnvVars) {
        $k, $v = $pair -split "=", 2
        Set-Item -Path "Env:$k" -Value $v
    }
    & dotnet bin/service.dll --seconds 8 --live-mb 150
    Write-Host "exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

if (-not (Get-Command systemd-run -ErrorAction SilentlyContinue)) {
    Write-Host "No systemd-run here (needs Linux + systemd). The memory limit is a real cgroups feature,"
    Write-Host "not something a script can emulate on Windows/macOS -- run this lab under WSL2 or a Linux"
    Write-Host "container to see the limited runs. './run.ps1 none' still works natively everywhere."
    exit 1
}

$sdArgs = @("--user", "--scope", "-q", "-p", "MemoryMax=$Limit", "-p", "MemorySwapMax=0")
foreach ($pair in $EnvVars) { $sdArgs += @("-E", $pair) }
$sdArgs += @("dotnet", "bin/service.dll", "--seconds", "8", "--live-mb", "150")

& systemd-run @sdArgs
Write-Host "exit code: $LASTEXITCODE"
exit $LASTEXITCODE
