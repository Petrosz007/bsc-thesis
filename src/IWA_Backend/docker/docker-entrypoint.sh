#!/bin/sh

export IWA_ConnectionStrings__MySqlServer="Server=$IWA_MYSQL_HOST;Port=$IWA_MYSQL_PORT;Database=$IWA_MYSQL_DB;Uid=$IWA_MYSQL_USER;Pwd=$IWA_MYSQL_PASS"

# Copy the default avatars to the avatars folder
cp Avatars/* AvatarData/

# Start the backend
/wait-for.sh "$IWA_MYSQL_HOST:$IWA_MYSQL_PORT" -- dotnet IWA_Backend.API.dll
