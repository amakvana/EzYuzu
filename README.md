![GitHub Release](https://img.shields.io/github/v/release/amakvana/EzYuzu?style=for-the-badge&logo=appveyor)
![GitHub license](https://img.shields.io/github/license/amakvana/EzYuzu?style=for-the-badge&logo=appveyor)
![GitHub repo size](https://img.shields.io/github/repo-size/amakvana/EzYuzu?style=for-the-badge&logo=appveyor)
![GitHub all releases](https://img.shields.io/github/downloads/amakvana/EzYuzu/total?style=for-the-badge&logo=appveyor)

# EzYuzu

A Portable Yuzu Updater for Standalone versions of Yuzu Mainline or Early Access (EA).

Perfect for those who run Yuzu off an External HDD or through (but not limited to) frontends such as LaunchBox, Steam, EmulationStation and HyperSpin.

![EzYuzu v1.5.0.0](images/ezyuzu_1500.png)

## Table of Contents

- [Overview](#overview)
  - [Methodology](#methodology)
  - [Usage](#usage)
- [Download](#downloads)
- [Installation](#installation)
- [User Guide](#user-guide)
- [Acknowledgements](#acknowledgements)

## Overview

### Methodology

- Reads https://github.com/yuzu-emu/yuzu-mainline/releases/latest
- Fetches the latest .7z archive URL
- Downloads & extracts it into your Yuzu Root Folder

### Usage

1. Browse and locate the your Yuzu Root Folder, this is the folder containing `yuzu.exe`
2. EzYuzu will automatically detect the version of yuzu.exe
3. (Optional): Change Update Channels
4. Click on `New Install` or `Update Yuzu`

- Downloads the latest copy of yuzu-mainline & extracts it into your Yuzu Root Folder.
- Automatically checks if your standalone copy of Yuzu is up-to-date.
- `Dependencies` can be included within `Update Yuzu` by checking the option within `Options` > `General` > `Update Yuzu` > `Reinstall Visual C++`
- It shouldn't overwrite configs unless `New Install` is displayed. However, backup beforehand.
- Temp files are stored within `TempUpdate` and are deleted upon completion.
- [GUIDE](https://github.com/amakvana/EzYuzu/blob/master/GUIDE.md) for detailed instructions

## Downloads

https://github.com/amakvana/EzYuzu/releases/latest

Requires:

- Latest [7-Zip](https://www.7-zip.org/a/7z2201-x64.msi) installed.
- Latest [Microsoft .NET Framework](https://go.microsoft.com/fwlink/?linkid=2088631) installed.
- Latest [Visual C++ X64 Redistributable](https://aka.ms/vs/16/release/vc_redist.x64.exe) installed.

## Installation

Extract the entire contentsof the `EzYuzu.zip` file, including all `.dll` files, into a folder and run `EzYuzu.exe`

![EzYuzuSetupAnimated](images/ezyuzu-setup.gif)

EzYuzu is 100% portable - it can be run from any location.

EzYuzu requires Administrator privileges to ensure Dependencies can be installed.

## User Guide

The User Guide can be found [here](https://github.com/amakvana/EzYuzu/blob/master/GUIDE.md)

## Acknowledgements

Thanks:

- [Yuzu Team](https://yuzu-emu.org/) - Nintendo Switch Emulator Developers
- [PineappleEA](https://github.com/pineappleEA/pineapple-src) - Early Access Builds
- [Stellar](https://github.com/StellarUpdater/Stellar) - Inspiration
- [Agus Raharjo](https://www.iconfinder.com/agusraharj) - Icons
