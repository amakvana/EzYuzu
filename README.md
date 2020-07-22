# EzYuzu
A Portable Yuzu Updater for Standalone versions of Yuzu. 

Perfect for those who run Yuzu through (but not limited to) frontends  such as Launchbox, Steam, EmulationStation and Hyperspin.   

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
* `Check` button to check if your copy of Yuzu is up-to-date.
* It shouldn't overwrite configs unless New Install is selected. However, backup beforehand. 
* Temp files are stored within TempUpdate and are deleted upon completion.

## Downloads
https://github.com/amakvana/EzYuzu/releases/latest

Requires the latest [Microsoft .NET Framework](https://go.microsoft.com/fwlink/?linkid=2088631)

## Installation
EzYuzu is 100% portable - it can be run from any location.

EzYuzu requires Administrator privileges to ensure Dependencies can be installed.

## Hashes 
Hashes of latest EzYuzu.exe below: 
Hash | Value
--- | ---
MD5 | 8c87e3f5af6ebc3b7b525585c58a3fa7
SHA1 | 7a2d01e91e7c4fc5fa3470a6630fa52cc3e21dd4
SHA256 | 326529099c5f07fb1237f1c4840abd082d1973a55b80ebfd39caf0ad2422b4cf
SHA512 | 9b16503ce44bd05c0a8c411eb349b8a0d16bd8b7dbaff082912497e4e7ffe169dff24a1f7efc33ebb2f6c8855c39d565029c6b2e81c7d53d8315ef7c7699603a

## Acknowledgements
Thanks:
* [Stellar](https://github.com/StellarUpdater/Stellar) - Inspiration
* [Agus Raharjo](https://www.iconfinder.com/agusraharj) - Icons
