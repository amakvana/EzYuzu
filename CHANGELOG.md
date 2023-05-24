# Changelog

All notable changes to this project will be documented in this file.

<br>

## [1.6.0.0] - 2023-05-24

### Added

- Complete rewrite in .NET 7. Goodbye .NET Framework 4.8.1 - better performance, newer c# features and increased support for await/async model
- Ability to override Update Versions - this enables the ability to rollback versions - thanks [@Nicolas-Miranda](https://github.com/amakvana/EzYuzu/issues/16)
  - `Options` > `Advanced` > `Override Update Versions`
- Added Command Line arguments to allow EzYuzu to be executed via Command Line. This enables bulk updates in different locations, see [GUIDE](https://github.com/amakvana/EzYuzu/blob/master/GUIDE.md) for usage - thanks [@miapuffia](https://github.com/amakvana/EzYuzu/issues/17)
- Added option to Launch Yuzu after successful update - thanks [@Xarishark](https://github.com/amakvana/EzYuzu/issues/21)
  - `Options` > `General` > `Update Yuzu` > `Launch Yuzu after Update`
- When EzYuzu detects a newer version of Yuzu available to install, the `Update` button now also displays the build number available - thanks [@Nicolas-Miranda](https://github.com/amakvana/EzYuzu/issues/16)
- Added `.editorconfig` file to repo - thanks [@informagico](https://github.com/amakvana/EzYuzu/issues/20)

### Changed

- `EzYuzu` now bundles all `.dll` dependencies inside the executable - no need to drag any additional `.dll` files into EzYuzu's working directory
- `EzYuzu` no longer requires Administrator rights to run - thanks [@stephannn](https://github.com/amakvana/EzYuzu/issues/1)
- `Visual C++` is automatically installed when EzYuzu is ran as Administrator. Ignored when ran as User.
- Goodbye `Newtonsoft.Json`, hello `System.Text.Json`
  - Bundled into .NET 7 and adds better performance when pulling json data - incompatibilites before have been resolved
- Goodbye `Ookii.Dialogs.WinForms`, .NET 7 now provides modern `FolderBrowseDialog`
- Another code refactor to handle both Update Channels. Removed redundant code
- Further GUI tweaks to improve UX
  - Advanced options such as `Overriding Update Channel` and `Update Versions` is now moved to `Options` > `Advanced`. Hidden by default to declutter main UI
- Removed redundant code from codebase
- Optimised several methods to utilise await/async.
- Retired `WebClient`, now utilises `IHttpClientFactory` to pull data - faster and memory efficient

### Fixed

- [Issue #22](https://github.com/amakvana/EzYuzu/issues/22) - EzYuzu now properly supports mapped drives for network drives, no need to type FQDN

<br>

## [1.5.1.0] - 2023-03-27

### Changed

- Downgraded `Ookii.Dialogs.WinForms` binaries from `4.0.0` to `1.2.0` due to incompatibilities with certain systems and configurations
- Switched `System.Text.Json` to `Newtonsoft.Json` due to incompatibilities with certain systems and configurations
- Tweaked `GetEarlyAccessGitHubRepoJsonData()` within `EarlyAccessYuzuManager.cs` to remove the `MemoryStream` dependency
- Tweaked `GetMainlineGitHubRepoJsonData()` within `MainlineYuzuManager.cs` to remove the `MemoryStream` dependency

### Fixed

- [Issue #15](https://github.com/amakvana/EzYuzu/issues/15) - Can't load 'System.Runtime.CompilerServices.Unsafe'

<br>

## [1.5.0.0] - 2023-03-26

### Added

- Added support for Early Access Builds via [pineappleEA](https://github.com/pineappleEA/pineapple-src)
  - Mainline remains default for `New Install`, unless overridden.
- Ability to switch Update Channels for current installations.
  - `Options` > `General` > `Update Channel` > `Override Update Channel`
- EzYuzu will automatically detect which Update Channel the selected copy of Yuzu is on.

### Changed

- Goodbye `RegEx`, hello `System.Text.Json`

  - Completely refactored how EzYuzu pulls the latest archives down for both Update Channels - now uses api.github.com instead of downloading the entire webpage and parsing with RegEx. Increased robustness and further speed improvements.

- Updated `FrmAbout` credits to point to Readme Acknowledgements
- Updated `Ookii.Dialogs.WinForms` binaries from `1.2.0` to `4.0.0`
- Another code refactor to handle both Update Channels. Removed redundant code
- Improved yuzu version detection - checks current channel through `yuzu-cmd.exe`
- Path handling - now uses `Path.Combine` to build filepaths instead of hard-coding
- Further UI tweaks to improve UX
  - Disabling dropdowns and other controls when `btnProcess` is initiated

### Fixed

- Improved error handling
- Typo's within `FrmAbout`

<br>

## [1.4.0.0] - 2022-12-30

### Added

- Updated app to `Microsoft .NET Framework 4.8`
- Option to `Check for Updates` within EzYuzu. This can be found via `Help` > `Check for Updates`
- EzYuzu now closes `Yuzu.exe` before updating files to prevent file locks - thanks [@buruto](https://github.com/amakvana/EzYuzu/issues/11#issuecomment-1368053397)
- EzYuzu now uses locally installed 7-Zip if present to extract archives
- EzYuzu now displays which version of Yuzu is being downloaded

### Changed

- Complete code refactor of EzYuzu

  - Extracted duplicate methods from `frmMain.cs` and moved into `EzYuzuManager.cs`
  - Utilises `await/async` model
  - Redundant / duplicate code removed
  - Improved Error handling

- EzYuzu now downloads `yuzu-windows-msvc-xxx-xxx.7z` instead of `yuzu-windows-msvc-xxx-xxx.zip` and uses `7z.exe` to unpack rather than `System.IO.Compression.ZipFile` - upto 25% speed increase

- Tweaked `FetchLatestYuzuVersionNumber` method within `EzYuzuManager.cs` - pulls version from URL instead of downloading and parsing the page header

- EzYuzu update module will now allow overlapping of versions before older versions are no longer supported.

- EzYuzu now automatically selected the appropriate action on the selected directory

  - If `yuzu.exe` is not found, assumes `New Install`
  - If `yuzu.exe` exists and `version` file is old, assumes `Update Yuzu`
  - If `version` file is latest, disable update button

- EzYuzu GUI changes

  - Removed `Check` button - now checks when directory is selected
  - Removed `Dropdown` options - now dynamically decides option based on directory selected
  - Added `Reinstall Visual C++` options. This can be found via `Options` > `General` > `Update Yuzu` > `Reinstall Visual C++`
  - Increased `Update` button size to improve UX

### Fixed

- An issue with parsing the version number from yuzu-mainline repo
- [Issue #11](https://github.com/amakvana/EzYuzu/issues/11) and [Issue #8](https://github.com/amakvana/EzYuzu/issues/8) - Unhandled exception has occurred in your application when `yuzu-windows-msvc` exists within same directory

<br>

## [1.3.0.0] - 2022-09-21

### Fixed

- Unhandled Exception error when Updating Yuzu ([Issue #7](https://github.com/amakvana/EzYuzu/issues/7)) - due to GitHub updating their URL structure

<br>

## [1.2.0.0] - 2021-08-20

### Added

- EzYuzu version detection
- EzYuzu now remembers previously selected Yuzu Folder Location when launched

### Changed

- Folder Browse dialog - modern UI implemented

### Fixed

- Visual C++ Redistributable installer no longer forces reboot
- Improved cleanup process
- UI tweaks to improve UX

<br>

## [1.1.0.0] - 2020-07-22

### Added

- `New Install` - now also sets "Optimised GPU Defaults"

### Changed

- Improved GPU detection

### Fixed

- UI grammar and punctuation

<br>

## [1.0.0.0] - 2020-07-21

### Added

- Initial release

### Changed

### Fixed
