# EzYuzu Guide

## Requirements
* Latest version of [Microsoft .NET Framework](https://go.microsoft.com/fwlink/?linkid=2088631) installed.
* Administrator rights, to allow redistributables to install.

## Methodology 
* Reads https://github.com/yuzu-emu/yuzu-mainline/releases/latest
* Fetches the latest .zip URL
* Downloads & extracts it into your Yuzu root folder

## Usage 
1. Browse and locate your Yuzu root folder
2. Select a `Download Option` from the dropdown menu
3. Click on ```Install/Update/Upgrade``` 

* Downloads the latest copy of yuzu-mainline & extracts it into your Yuzu root folder.
* `Check` button to check if your copy of Yuzu is up-to-date.
* It shouldn't overwrite configs unless New Install is selected. However, backup beforehand. 
* Temp files are stored within TempUpdate and are deleted upon completion.

## Download Options
Option | Description
--- | ---
`Dependencies` | (Re)Installs Redistributables. Helpful if Yuzu won't start after update.
`New Install` | Installs Yuzu & Redistributables. Resets configs & sets [optimised GPU defaults](https://github.com/amakvana/EzYuzu/tree/master/configs).
`Upgrade` | Upgrades Yuzu to the latest version, including Redistributables but excluding configs.
`Yuzu` | Updates Yuzu to the latest version, excluding Redistributables and configs.

## New Install
To install Yuzu Portable for the first time:
1. Create a Yuzu folder on your device.
2. Select your newly created Yuzu root folder.
3. Select `New Install` from the `Download Options` dropdown
4. Click `Install`

## Yuzu Update
1. Select your Yuzu root folder
2. Select `Yuzu` or `Upgrade` from the `Download Options` menu
3. Click the `Check` button to get update status (optional)
4. Click `Update/Upgrade`
