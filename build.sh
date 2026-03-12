#!/bin/bash
dotnet build
mkdir -p '/mnt/sofia/SteamLibrary/steamapps/compatdata/544550/pfx/drive_c/users/steamuser/Documents/My Games/Stationeers/mods/IC10DefineWorkaround/'
echo Copying dll to Stationeers directory
cp bin/Debug/net48/IC10DefineWorkaround.dll '/mnt/sofia/SteamLibrary/steamapps/compatdata/544550/pfx/drive_c/users/steamuser/Documents/My Games/Stationeers/mods/IC10DefineWorkaround/'
echo Copying pdb to Stationeers directory
cp bin/Debug/net48/IC10DefineWorkaround.pdb '/mnt/sofia/SteamLibrary/steamapps/compatdata/544550/pfx/drive_c/users/steamuser/Documents/My Games/Stationeers/mods/IC10DefineWorkaround/'
echo Copying GameData to Stationeers directory
cp -r GameData '/mnt/sofia/SteamLibrary/steamapps/compatdata/544550/pfx/drive_c/users/steamuser/Documents/My Games/Stationeers/mods/IC10DefineWorkaround/'
echo Copying About to Stationeers directory
cp -r About '/mnt/sofia/SteamLibrary/steamapps/compatdata/544550/pfx/drive_c/users/steamuser/Documents/My Games/Stationeers/mods/IC10DefineWorkaround/'
