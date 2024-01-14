#!/bin/bash

read -p "1. Enter your SendGrid API key: " SENDGRID_KEY
setx SendGridKey "$SENDGRID_KEY"

echo -e "\n2. Generate SAFE_SHARE_HMAC key."
(cd SafeShare.InternalCrypto && dotnet run)
read -p "After obtaining the HMAC key, paste it here: " _SAFE_SHARE_HMAC
setx SAFE_SHARE_HMAC "$_SAFE_SHARE_HMAC"

echo -e "\n3.
# Step 3: Setup Database for CryptoKeysDb and Api key
# After some time check if the database is created and 
# if so cancel with ctrl+c and then close the terminal.
" 
setx SAFE_SHARE_API_KEY "249f5306cb93c276ef417bd8c8377be293b9489a6ab4ca3ee89740f44b5c03f7"

# SAFE_SHARE_API_KEY is a key to access to the API without that key no one can access to the server. In normal condition, you need to
# apply to a service outside of this project to get a key but for simplification we provide a key.

cd API_Client.API && dotnet run --urls=https://localhost:7261