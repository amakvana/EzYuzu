# EzYuzu Guide

## Requirements

- Latest [7-Zip](https://www.7-zip.org/a/7z2201-x64.msi) installed.
- Latest [.NET 7 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.5-windows-x64-installer) installed.
- Latest [Visual C++ X64 Redistributable](https://aka.ms/vs/16/release/vc_redist.x64.exe) installed.

## Methodology

- Reads [Yuzu Mainline](https://github.com/yuzu-emu/yuzu-mainline/releases/latest) or [Yuzu Early Access (EA)](https://github.com/pineappleEA/pineapple-src/releases/latest) JSON data
- Fetches the latest `.7z` or `.zip` archive URL
- Downloads & extracts it into your Yuzu Root Folder

## Usage

1. Browse and locate the your Yuzu Root Folder, this is the folder containing `yuzu.exe`
2. EzYuzu will automatically detect the version of yuzu.exe
3. (Optional): Change Update Channel and/or Update Version
4. Click on `New Install` or `Update Yuzu`

- Downloads the latest copy of yuzu & extracts it into your Yuzu Root Folder.
- Automatically checks if your standalone copy of Yuzu is up-to-date.
- `Dependencies` are automatically installed when EzYuzu is ran as Administrator.
- `Update Channel` and `Update Version` can be overridden by checking the options within `Options` > `Advanced`
- It shouldn't overwrite configs unless `New Install` is displayed. However, backup beforehand.
- Temp files are stored within `TempUpdate` and are deleted upon completion.
- [GUIDE](https://github.com/amakvana/EzYuzu/blob/master/GUIDE.md) for detailed instructions

## Download Options

| Option         | Description                                                                                                                                        |
| -------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| `New Install`  | Installs Yuzu Mainline & Redistributables. Resets configs & sets [optimised GPU defaults](https://github.com/amakvana/EzYuzu/tree/master/configs). |
| `Update Yuzu`  | Updates Yuzu to the latest version, excluding Redistributables and optimised configs.                                                              |
| `Dependencies` | Automatically installed when EzYuzu is ran as Administrator.                                                                                       |

## Graphical User Interface Options

### New Install

To install Yuzu Portable for the first time:

1. Create an empty folder on your device and give it a name.
2. Select your newly created empty folder.
3. The button should now change to `New Install`
4. Click on `New Install`
5. Done

### Update Yuzu

1. Select your Yuzu root folder (the folder containing `yuzu.exe`)
2. The button should now change to `Update Yuzu`
3. Click on `Update Yuzu`
4. Done

### Switching Update Channels and Update Versions

1. Select your Yuzu root folder (the folder containing `yuzu.exe`)
2. Check the options within `Options` > `Advanced`
3. Select which `Update Channel` and `Update Version` you want to update Yuzu on, via the Dropdown menu.
4. Click on `Update Yuzu`
5. Done

### Checking Yuzu is up-to-date

1. Select your Yuzu root folder (the folder containing `yuzu.exe`)
2. EzYuzu will automatically check if the current copy of Yuzu is up-to-date
3. If Yuzu is up-to-date, the Update button will be disabled and will state `Yuzu is Up-To-Date!`
4. Done

## Command Line Interface Options

### Switches

```
-p, --path              Required. Set the Yuzu Location Directory Path, this is the path where Yuzu.exe resides. Must wrap path in double quotes.
-m, --mainline          Force EzYuzu to use Mainline channel when updating Yuzu
-e, --early-access      Force EzYuzu to use Early-Access channel when updating Yuzu
-v                      Set a specific version number to update/rollback Yuzu to. Useful when needing to rollback Yuzu. Must wrap version number in double quotes.
-l, --launch-yuzu       Launch Yuzu after successful New Install/Update

--help                  Displays the EzYuzu help screen
--version               Displays EzYuzu's version information
```

### Usage Examples

New Install/Update to latest Yuzu, automatically detecting Update Channel and Version:

```
EzYuzu.exe -p "D:\Yuzu"
```

Update to latest Yuzu, switching Update channel to Mainline:

```
EzYuzu.exe -p "D:\Yuzu" -m
```

Update to latest Yuzu, switching Update channel to Early-Access:

```
EzYuzu.exe -p "D:\Yuzu" -e
```

Update/rollback to specific version of Mainline Yuzu (e.g. 1437):

```
EzYuzu.exe -p "D:\Yuzu" -m -v "1437"
```

Update/rollback to specific version of Early-Access Yuzu (e.g. 3600):

```
EzYuzu.exe -p "D:\Yuzu" -e -v "3600"
```

Launch Yuzu after EzYuzu has completed an update

```
EzYuzu.exe -p "D:\Yuzu" -l
```
