#!/bin/bash

echo -e "\n1. Running the project.
#
# Running the project will autocreate the databases the api main db and crypto db
# Make sure you have installed slq and mssql. After some secons try to navigate to
# this route https://localhost:7046/swagger/index.html. If you succsessfully
# navigated and the page opens close this script and execute the final script.
#
# Before navigating to https://localhost:7027/ make sure the SafeShare and SafeShareCryptoKeys databases
# are created, if not run the script again and if succsessfully go to : https://localhost:7027/ .
#
# To quit, please use "CRTL+C".
#
"

echo "Running Main Api..."
(cd SafeShare.API && dotnet run) &
api_pid=$!

echo "Running Blazor Client..."
(cd SafeShare.Client && dotnet run) &
client_pid=$!

echo "Running Proxy Api..."
(cd SafeShare.ProxyApi && dotnet run) &
proxy_pid=$!

# Function to clean up resources
cleanup() {
    echo "Cleaning up..."
    # Kill the process or processes you want to terminate
    kill "$api_pid" "$client_pid" "$proxy_pid"
    taskkill -F -IM dotnet.exe
    exit 0
}

# Trap the EXIT signal and execute the cleanup function
trap cleanup EXIT

# Your script logic goes here (if needed)

# The script will wait here until all background processes are finished
wait "$api_pid" "$client_pid" "$proxy_pid"