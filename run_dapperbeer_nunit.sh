#!/bin/bash
clear
#dotnet build /workspace/src/DapperBeer_nunit
dotnet test /workspace/src/DapperBeer_nunit -v n --diag:/workspace/logs/nunit/nunit_log.txt --tl:off --nologo