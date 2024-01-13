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
"

echo "Running Main Api..."
(cd SafeShare.API && dotnet run) &


echo "Running Blazor Client..."
(cd SafeShare.Client && dotnet run) &

echo "Running Proxy Api..."
(cd SafeShare.ProxyApi && dotnet run) &