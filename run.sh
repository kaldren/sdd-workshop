#!/usr/bin/env bash
# Starts Timesheet.Api and Timesheet.Web together. Ctrl+C stops both. See specs/000-solution-setup.md (R9).
set -euo pipefail

cd "$(dirname "$0")"

dotnet build Timesheet.sln

pids=()

stop() {
    trap - INT TERM EXIT
    echo
    echo "Stopping..."
    for pid in "${pids[@]}"; do
        pkill -TERM -P "$pid" 2>/dev/null || true
        kill -TERM "$pid" 2>/dev/null || true
    done
    wait 2>/dev/null || true
}
trap stop INT TERM EXIT

dotnet run --project src/Timesheet.Api --no-build --launch-profile http &
pids+=($!)
dotnet run --project src/Timesheet.Web --no-build --launch-profile http &
pids+=($!)

echo
echo "Api: http://localhost:5100"
echo "Web: http://localhost:5200"
echo "Press Ctrl+C to stop."
echo

wait
