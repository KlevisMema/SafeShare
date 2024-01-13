#!/bin/bash

read -p "1. Enter your SendGrid API key: " SENDGRID_KEY
setx SendGridKey "$SENDGRID_KEY"

echo -e "\n2. Generate SAFE_SHARE_HMAC key."
(cd SafeShare.InternalCrypto && dotnet run --no-build)
read -p "After obtaining the HMAC key, paste it here: " _SAFE_SHARE_HMAC
setx SAFE_SHARE_HMAC "$_SAFE_SHARE_HMAC"

echo -e "\n3.
# Step 3: Setup Database for CryptoKeysDb and Api key
# 
#
#
# Navigate to https://localhost:7261/swagger/index.html and create an account in the endpint
# [Post] api/Clients/, then after succsess account creation use the 
# [Post] api/Authentication/Login to login. If succsess login please
# check the response and copy the token value and in the very top 
# of the page click the button authorize and paste the token there 
# it should be in the format : "Bearer your-token" 
# Now you can use the => [Post] /api/keys run it and it will give you a api key.
" 
(cd API_Client.API && dotnet run --urls=https://localhost:7261 &)

sleep 5

echo -e "\n4."
read -p "After obtaining the API key, paste it here: " API_KEY
setx API_KEY "$API_KEY"