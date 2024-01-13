#!/bin/bash

echo -e "1. Building the main api project: "
(cd API_Client.API && dotnet build)

echo -e "\n2. Running the project.
#
# Running the project will autocreate the databases the api main db and crypto db
# Make sure you have installed slq and mssql. After some secons try to navigate to
# this route https://localhost:7046/swagger/index.html. If you succsessfully
# navigated and the page opens close this script and execute the final script.
#
"
(cd SafeShare.API && dotnet run --urls=https://localhost:7046 &)