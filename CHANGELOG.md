# Changelog

All notable changes to this project will be documented in this file.

<br>

## [1.5.0.0] - 2023-03-26

### Added

- Added support for Early Access Builds via [pineappleEA](https://github.com/pineappleEA/pineapple-src)
  - Mainline remains default for `New Install`, unless overridden.
- Ability to switch Update Channels for current installations.
  - `Options` > `General` > `Update Channel` > `Override Update Channel`
- EzYuzu will automatically detect which Update Channel the seelcted copy of Yuzu is on.

### Changed

- Goodbye `RegEx`, hello `System.Text.Json`

  - Completely refactored how EzYuzu pulls the latest archives down for both Update Channels - now uses api.github.com instead of downloading the entire webpage and parsing with RegEx. Increased robustness and further speed improvements.

- Updated `FrmAbout` credits to point to Readme Acknowledgements
- Updated `Ookii.Dialogs.WinForms` binaries from `1.2.0` to `4.0.0`
- Another code refactor to handle both Update Channels. Removed redundant code
- Improved yuzu version detection - checks current channel through `yuzu-cmd.exe`
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
