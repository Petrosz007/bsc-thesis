#!/bin/sh
# Copy the default avatar to the avatars folder
cp Avatars/default.jpg AvatarData/default.jpg

# Start the backend
dotnet IWA_Backend.API.dll
