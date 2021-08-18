![GitHub Release](https://img.shields.io/github/v/release/amakvana/EzYuzu?style=for-the-badge&logo=appveyor)
![GitHub license](https://img.shields.io/github/license/amakvana/EzYuzu?style=for-the-badge&logo=appveyor)
![GitHub repo size](https://img.shields.io/github/repo-size/amakvana/EzYuzu?style=for-the-badge&logo=appveyor)
![GitHub all releases](https://img.shields.io/github/downloads/amakvana/EzYuzu/total?style=for-the-badge&logo=appveyor)

# EzYuzu
A Portable Yuzu Updater for Standalone versions of Yuzu. 

Perfect for those who run Yuzu off an External HDD or through (but not limited to) frontends  such as LaunchBox, Steam, EmulationStation and HyperSpin.   

## Overview
![EzYuzu](images/ezyuzu.png)

### Methodology 
* Reads https://github.com/yuzu-emu/yuzu-mainline/releases/latest
* Fetches the latest .zip URL
* Downloads & extracts it into your Yuzu root folder

### Usage 
1. Browse and locate your Yuzu root folder
2. Select a `Download Option` from the dropdown menu
3. Click on ```Install/Update/Upgrade``` 

* Downloads the latest copy of yuzu-mainline & extracts it into your Yuzu root folder.
* `Check` button to check if your standalone copy of Yuzu is up-to-date.
* It shouldn't overwrite configs unless `New Install` is selected. However, backup beforehand. 
* Temp files are stored within TempUpdate and are deleted upon completion.
* [GUIDE](https://github.com/amakvana/EzYuzu/blob/master/GUIDE.md) for detailed instructions

## Downloads
https://github.com/amakvana/EzYuzu/releases/latest

Requires the latest [Microsoft .NET Framework](https://go.microsoft.com/fwlink/?linkid=2088631)

## Installation
EzYuzu is 100% portable - it can be run from any location.

EzYuzu requires Administrator privileges to ensure Dependencies can be installed.

## Acknowledgements
Thanks:
* [Yuzu Team](https://yuzu-emu.org/) - Nintendo Switch Emulator Developers 
* [Stellar](https://github.com/StellarUpdater/Stellar) - Inspiration
* [Agus Raharjo](https://www.iconfinder.com/agusraharj) - Icons
