<#
.SYNOPSIS
Starts the triage service. Watch it from another terminal.

.PARAMETER Scenario
Which scenario to run: a, b, c, d, or e.

.PARAMETER Seconds
How long to run for.
#>
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("a", "b", "c", "d", "e")]
    [string]$Scenario,

    [int]$Seconds = 120
)

Set-Location $PSScriptRoot
& dotnet bin/service.dll --scenario $Scenario --seconds $Seconds
