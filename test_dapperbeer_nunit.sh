#!/bin/bash
dotnet test /workspace/src/DapperBeer_nunit -v n --diag:/workspace/logs/nunit/nunit_log.txt --tl:off --nologo $@