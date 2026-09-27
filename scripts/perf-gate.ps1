#!/usr/bin/env pwsh
<#
.SYNOPSIS
Perf gate: every *solution* must PASS its budgets (exit 0) and every *exercise* must FAIL them (exit 1).
A green run means the budgets still separate the fixed code from the slow code, i.e. the gate can actually
catch regressions.

.PARAMETER Prefix
Optional name prefix to restrict which exercises/solutions are checked, e.g. "L02" or "L04-01".

.EXAMPLE
./scripts/perf-gate.ps1
./scripts/perf-gate.ps1 L02
#>
param(
    [string]$Prefix = ""
)

Set-Location (Join-Path $PSScriptRoot "..")

function Invoke-Build {
    param([string]$SlnPath)
    $output = & dotnet build $SlnPath -c Release -v q 2>&1
    $errorLines = $output | Select-String -CaseSensitive -Pattern "error|Build FAILED"
    if ($errorLines) {
        $errorLines | ForEach-Object { Write-Host $_.Line }
        Write-Host "build failed"
        exit 1
    }
}

Invoke-Build "PerfLab.slnx"
Invoke-Build "PerfLab.Solutions.slnx"

"{0,-34} {1,-9} {2,-9} {3}" -f "project", "expected", "actual", "verdict" | Write-Host

$fail = 0
$total = 0

foreach ($kind in @("solutions", "exercises")) {
    $want = if ($kind -eq "exercises") { 1 } else { 0 }
    $dirs = Get-ChildItem -Path "levels/*/$kind/$Prefix*" -Directory -ErrorAction SilentlyContinue | Sort-Object FullName

    foreach ($d in $dirs) {
        $programCs = Join-Path $d.FullName "Program.cs"
        $hasProgram = Test-Path $programCs
        if ($kind -eq "exercises" -and -not $hasProgram) { continue }

        $name = "$kind/$($d.Name)"

        $total++

        $job = Start-Job -ScriptBlock {
            param($proj)
            & dotnet run -c Release --no-build --project $proj *> $null
            $LASTEXITCODE
        } -ArgumentList $d.FullName

        if (Wait-Job $job -Timeout 300) {
            $rc = Receive-Job $job
        } else {
            Stop-Job $job
            $rc = 124   # matches GNU `timeout`'s convention for "timed out"
        }
        Remove-Job $job -Force

        $verdict = "OK"
        if ($rc -gt $want) { $fail = 1; $verdict = "SURPRISE" }
        "{0,-34} {1,-9} {2,-9} {3}" -f $name, "exit $want", "exit $rc", $verdict | Write-Host
    }
}

$summary = if ($fail -eq 0) { "all as expected" } else { "SURPRISES found" }
Write-Host "checked $total projects: $summary"
exit $fail
