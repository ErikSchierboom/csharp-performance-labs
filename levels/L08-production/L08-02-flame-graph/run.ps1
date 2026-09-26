param(
    [int]$Seconds = 120
)

Set-Location $PSScriptRoot
& dotnet bin/service.dll --seconds $Seconds
