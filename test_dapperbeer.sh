#!/bin/bash
clear
dotnet build /workspace/src/DapperBeer_tunit
/workspace/test_dapperbeer_nunit.sh
/workspace/test_dapperbeer_tunit.sh