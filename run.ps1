#!/usr/bin/env pwsh
# Starts Timesheet.Api and Timesheet.Web together. Ctrl+C stops both. See specs/000-solution-setup.md (R9).
$ErrorActionPreference = 'Stop'

Set-Location $PSScriptRoot

dotnet build Timesheet.sln
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$processes = @(
    Start-Process dotnet -ArgumentList 'run', '--project', 'src/Timesheet.Api', '--no-build', '--launch-profile', 'http' -NoNewWindow -PassThru
    Start-Process dotnet -ArgumentList 'run', '--project', 'src/Timesheet.Web', '--no-build', '--launch-profile', 'http' -NoNewWindow -PassThru
)

Write-Host ''
Write-Host 'Api: http://localhost:5100'
Write-Host 'Web: http://localhost:5200'
Write-Host 'Press Ctrl+C to stop.'
Write-Host ''

try {
    while ($processes | Where-Object { -not $_.HasExited }) {
        Start-Sleep -Milliseconds 500
    }
}
finally {
    Write-Host 'Stopping...'
    foreach ($process in $processes) {
        if (-not $process.HasExited) {
            # Kill the whole tree: dotnet run starts the app as a child process.
            $process.Kill($true)
        }
    }
}
