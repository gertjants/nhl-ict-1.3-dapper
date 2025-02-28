#!/bin/bash
path="/tmp/DapperBeerStudent"
src="/workspace/src/DapperBeer"
test_frameworks=('tunit' 'nunit')

# Script to pull the repo from remote and sync the appropriate files.
if [ -d $path ]; then
    git -C $path pull
else
    git clone https://github.com/NHLStenden/DapperBeerStudent $path
fi;

# Moving appropriate files into the "root"
cp -rf "${path}/DapperBeer/DTO" "${src}"
cp -rf "${path}/DapperBeer/SQL" "${src}"
cp -rf "${path}/DapperBeer/Snapshots" "${src}"

# Ensuring symlinks have been set up between the frameworks.
for fw in "${test_frameworks[@]}"
do
    sspath="${src}_${fw}/Snapshots"

    if [ -d $sspath ]; then
        rm -rf $sspath
    fi;
    
    ln -s "${src}/Snapshots" $sspath

    #find "${src}/*" -maxdepth 1 -exec ln -s {} "${src}_${fw}"/ \;
done;