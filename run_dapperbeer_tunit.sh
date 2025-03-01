#!/bin/bash
clear
dotnet build /workspace/src/DapperBeer_tunit
dotnet test /workspace/src/DapperBeer_tunit -v n --tl:off --no-build --nologo --diag:/workspace/logs/tunit/tunit_log.txt