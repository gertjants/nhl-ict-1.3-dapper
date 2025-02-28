#!/bin/bash
mkdir /tmp/dotnet
cd /tmp/dotnet
echo "Fetching dotnet"
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh

#./dotnet-install.sh --version latest
./dotnet-install.sh --channel 9.0 --install-dir /usr/share/dotnet
./dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet
ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

cd / && rm -rf /tmp/dotnet