#!/bin/sh
set -x

coverletExclusions="[BookingQueueSubscriber.*Tests?]*,[BookingQueueSubscriber]Startup,[*]BookingQueueSubscriber.Common.*,[Testing.Common]*,[BookingQueueSubscriber.Security]*"
configuration=Release

# Script is for docker compose tests where the script is at the root level
dotnet test BookingQueueSubscriber/BookingQueueSubscriber.UnitTests/BookingQueueSubscriber.UnitTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=SchedulerJobs-Sds-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${sdsUnitExclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""
