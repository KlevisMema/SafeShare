#!/bin/bash

# Terminal 1: Blazor Proxy
#echo "Running Blazor Proxy..."
(cd SafeShare.Client && dotnet run) &

sleep 10

# Terminal 2: Main API
echo "Running Main API..."
(cd SafeShare.API && dotnet run) &

sleep 10

# Terminal 3: Proxy Api
echo "Running Another Application..."
(cd SafeShare.ProxyApi && dotnet run) &

# Wait for all background processes to finish
wait
