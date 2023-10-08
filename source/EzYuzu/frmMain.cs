using EzYuzu.Classes.Settings;
using EzYuzu.Classes.Updaters;
using EzYuzu.Classes.Yuzu.Detectors;
using EzYuzu.Classes.Yuzu.Managers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json;
using static EzYuzu.Classes.Yuzu.Detectors.YuzuBranchDetector;
using static EzYuzu.Classes.Yuzu.Detectors.YuzuInstallationStateDetector;

namespace EzYuzu
{
    public partial class FrmMain : Form
    {
        private readonly IHttpClientFactory? clientFactory;
        private readonly YuzuBranchDetector? branchDetector;
        private readonly int expandedGrpOptionsHeight;  // hold instance of expanded height
        private bool grpOptionsIsExpanded;
        private bool formHasLoaded;

        public FrmMain()
        {
            InitializeComponent();
            grpOptionsIsExpanded = false;
            expandedGrpOptionsHeight = grpOptions.Height;

            // set UI defaults - alter them if Ctrl is held down on launch 
            LoadApplicationSettings(IsControlDown());
        }

        public FrmMain(IServiceProvider serviceProvider) : this()
        {
            clientFactory = serviceProvider.GetService<IHttpClientFactory>();
            branchDetector = new YuzuBranchDetector(clientFactory!);
            formHasLoaded = false;
        }

        private async void FrmMain_LoadAsync(object sender, EventArgs e)
        {
            // check if software is latest 
            await CheckAppVersionAsync();

            // check last set path and refresh update channel and versions
            if (!string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                var branch = await RefreshDetectedYuzuInstallationUpdateBranchAsync();
                await RefreshDetectedYuzuInstallationUpdateVersionsAsync(branch);
                var installationState = await RefreshDetectedYuzuInstallationStateAsync(branch);

                // if auto-update on app start is checked, and if there is an update available... begin update
                if (installationState == YuzuInstallationState.UpdateAvailable && autoupdateOnEzYuzuStartToolStripMenuItem.Checked)
                    BtnProcess_ClickAsync(sender, e);

                // launch Yuzu if install up to date and option checked 
                if (installationState == YuzuInstallationState.LatestVersionInstalled && launchYuzuAfterUpdateToolStripMenuItem.Checked)
                {
                    Process.Start(new ProcessStartInfo(Path.Combine(txtYuzuLocation.Text, "yuzu.exe"))
                    {
                        UseShellExecute = true
                    })?.Dispose();
                    Application.Exit();
                }
            }

            formHasLoaded = true;
        }

        private async void FrmMain_FormClosedAsync(object sender, FormClosedEventArgs e)
        {
            await SaveApplicationSettings();
        }

        private async void CboUpdateChannel_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            if (!formHasLoaded || !cboUpdateChannel.Enabled)
                return;

            // get branch based on selected index 
            var branch = cboUpdateChannel.SelectedIndex switch
            {
                0 => YuzuBranch.Mainline,
                1 => YuzuBranch.EarlyAccess,
                _ => YuzuBranch.Mainline
            };

            await RefreshDetectedYuzuInstallationUpdateVersionsAsync(branch);
            await RefreshDetectedYuzuInstallationStateAsync(branch);
        }

        private async void CboUpdateVersion_SelectedIndexChangedAsync(object sender, EventArgs e)
        {
            if (!cboUpdateVersion.Enabled)
                return;

            var branch = cboUpdateChannel.SelectedIndex switch
            {
                1 => YuzuBranch.EarlyAccess,
                _ => YuzuBranch.Mainline  // mainline by default 
            };
            await RefreshDetectedYuzuInstallationStateAsync(branch);
        }

        private void LblYuzuLocation_Click(object sender, EventArgs e)
        {
            // invoke browse
            BtnBrowse_ClickAsync(sender, e);
        }

        private void TxtYuzuLocation_Click(object sender, EventArgs e)
        {
            // invoke browse
            BtnBrowse_ClickAsync(sender, e);
        }

        private async void BtnBrowse_ClickAsync(object sender, EventArgs e)
        {
            btnProcess.Enabled = false;
            txtYuzuLocation.Text = await ShowFolderBrowserDialogWindowAndGetResultAsync();
            enableHDRToolStripMenuItem.Checked = File.Exists(Path.Combine(txtYuzuLocation.Text, "cemu.exe"));

            // refresh yuzu installation data 
            var branch = await RefreshDetectedYuzuInstallationUpdateBranchAsync();
            await RefreshDetectedYuzuInstallationUpdateVersionsAsync(branch);
            var installationState = await RefreshDetectedYuzuInstallationStateAsync(branch);

            // launch Yuzu if install up to date and option checked 
            if (installationState == YuzuInstallationState.LatestVersionInstalled && launchYuzuAfterUpdateToolStripMenuItem.Checked)
            {
                string exeToLaunch = enableHDRToolStripMenuItem.Checked ? "cemu.exe" : "yuzu.exe";
                Process.Start(new ProcessStartInfo(Path.Combine(txtYuzuLocation.Text, exeToLaunch))
                {
                    UseShellExecute = true
                })?.Dispose();
                Application.Exit();
            }
        }

        private async void BtnProcess_ClickAsync(object sender, EventArgs e)
        {
            // no path selected? computer says no
            if (string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                MessageBox.Show("Please browse and select your Yuzu.exe folder location first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // path has been selected, begin processing

            // disable UI controls 
            ToggleUiControls(false);
            btnProcess.Enabled = false;

            // prepare YuzuManager to download latest copy of yuzu based on selected/detected branch 
            string yuzuLocation = txtYuzuLocation.Text;
            string yuzuTagName = cboUpdateVersion.Text;
            var yuzuDownloadUrl = cboUpdateVersion.SelectedValue?.ToString();

            YuzuManager yuzuManager = cboUpdateChannel.SelectedIndex switch
            {
                1 => new EarlyAccessYuzuManager(clientFactory!, yuzuTagName, yuzuDownloadUrl!),
                _ => new MainlineYuzuManager(clientFactory!, yuzuTagName, yuzuDownloadUrl!)
            };

            yuzuManager.YuzuDirectoryPath = yuzuLocation;
            yuzuManager.TempUpdateDirectoryPath = Path.Combine(yuzuLocation, "TempUpdate");
            yuzuManager.UpdateVisualCppRedistributables = reinstallVisualCToolStripMenuItem.Checked;
            yuzuManager.UpdateProgress += EzYuzuDownloader_UpdateCurrentProgress;
            await yuzuManager.DownloadPrerequisitesAsync();

            // Process yuzu depending on directory selected and type of YuzuManager
            switch (yuzuManager)
            {
                case EarlyAccessYuzuManager eaym when btnProcess.Text == "New Install":
                    await eaym.ProcessYuzuNewInstallationAsync();
                    break;

                case EarlyAccessYuzuManager eaym when btnProcess.Text.StartsWith("Update", StringComparison.Ordinal):
                    await eaym.ProcessYuzuUpdateAsync();
                    break;

                case MainlineYuzuManager mym when btnProcess.Text == "New Install":
                    await mym.ProcessYuzuNewInstallationAsync();
                    break;

                case MainlineYuzuManager mym when btnProcess.Text.StartsWith("Update", StringComparison.Ordinal):
                    await mym.ProcessYuzuUpdateAsync();
                    break;
            }

            // refresh directory installation status 
            var branch = await RefreshDetectedYuzuInstallationUpdateBranchAsync();
            await RefreshDetectedYuzuInstallationUpdateVersionsAsync(branch);
            await RefreshDetectedYuzuInstallationStateAsync(branch);

            // if EnableHDR option is checked 
            File.Delete(Path.Combine(yuzuLocation, "cemu.exe"));
            if (enableHDRToolStripMenuItem.Checked)
                RenameFile(yuzuLocation, "yuzu.exe", "cemu.exe");

            // Enable UI controls
            lblProgress.Text = "Done!";
            ToggleUiControls(true);

            // launch Yuzu if option checked 
            if (launchYuzuAfterUpdateToolStripMenuItem.Checked)
            {
                string exeToLaunch = enableHDRToolStripMenuItem.Checked ? "cemu.exe" : "yuzu.exe";
                Process.Start(new ProcessStartInfo(Path.Combine(yuzuLocation, exeToLaunch))
                {
                    UseShellExecute = true
                })?.Dispose();
                Application.Exit();
            }

            // if "Exit After Update" is selected, exit app 
            if (exitAfterUpdateToolStripMenuItem.Checked)
                Application.Exit();
        }

        private void EzYuzuDownloader_UpdateCurrentProgress(int progressPercentage, string progressText)
        {
            pbarCurrentProgress.Value = progressPercentage;
            pbarCurrentProgress.Refresh();
            lblProgress.Text = progressText;
            lblProgress.Refresh();
        }

        //// ====================================================
        ////  MENUSTRIP CONTROLS 
        //// ====================================================

        private void YuzuWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://yuzu-emu.org/")
            {
                UseShellExecute = true,
                Verb = "Open"
            })?.Dispose();
        }

        private async void ExitToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            await SaveApplicationSettings();
            Application.Exit();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var f = new FrmAbout();
            f.ShowDialog(this);
        }

        private async void OverrideUpdateChannelToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            cboUpdateChannel.Enabled = overrideUpdateChannelToolStripMenuItem.Checked;
            UpdateOptionsUi();

            if (!overrideUpdateChannelToolStripMenuItem.Checked)
            {
                var branch = await RefreshDetectedYuzuInstallationUpdateBranchAsync();
                await RefreshDetectedYuzuInstallationUpdateVersionsAsync(branch);
                await RefreshDetectedYuzuInstallationStateAsync(branch);
            }
        }

        private async void OverrideUpdateVersionToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            cboUpdateVersion.Enabled = overrideUpdateVersionToolStripMenuItem.Checked;
            UpdateOptionsUi();

            if (!overrideUpdateVersionToolStripMenuItem.Checked)
            {
                var branch = await RefreshDetectedYuzuInstallationUpdateBranchAsync();
                await RefreshDetectedYuzuInstallationUpdateVersionsAsync(branch);
                await RefreshDetectedYuzuInstallationStateAsync(branch);
            }
        }

        private void EnableHDRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // rename exe if option is checked and exe exists 
            if (enableHDRToolStripMenuItem.Checked && File.Exists(Path.Combine(txtYuzuLocation.Text, "yuzu.exe")))
            {
                RenameFile(txtYuzuLocation.Text, "yuzu.exe", "cemu.exe");
                return;
            }

            if (!enableHDRToolStripMenuItem.Checked && File.Exists(Path.Combine(txtYuzuLocation.Text, "cemu.exe")))
            {
                RenameFile(txtYuzuLocation.Text, "cemu.exe", "yuzu.exe");
                return;
            }
        }

        //// ====================================================
        ////  METHODS 
        //// ====================================================

        private void LoadApplicationSettings(bool isLaunchedInSafeMode)
        {
            // default settings 
            toolTip1.SetToolTip(cboUpdateChannel, "The current Update Channel of Yuzu. Auto-detected when the Yuzu Path is selected. \n" +
                                                  "Can be manually overridden by going into Options > General > Update Channel > Override Update Channel. \n\n" +
                                                  "Mainline - Contains all regular daily Yuzu updates. More stable. \n" +
                                                  "Early Access - Contains the latest experimental features and fixes, before they are merged into Mainline builds. Less stable. \n\n" +
                                                  "When no channel is detected, EzYuzu defaults to Mainline.");
            toolTip1.SetToolTip(lblUpdateChannel, toolTip1.GetToolTip(cboUpdateChannel));
            reinstallVisualCToolStripMenuItem.Checked = AppIsRunningAsAdministrator();

            // hide options groupbox and shrink form 
            this.Size = new Size(this.Size.Width, this.Size.Height - expandedGrpOptionsHeight);
            grpOptions.Height = 0;

            // safe mode ui adjustments 
            if (isLaunchedInSafeMode)
            {
                this.Text = $"{this.Text} - Safe Mode";
            }

            if (!File.Exists("EzYuzu.settings.json"))
                return;

            // if EzYuzu.json exists, load in preferences 
            string json = File.ReadAllText("EzYuzu.settings.json");
            var settings = JsonSerializer.Deserialize<EzYuzuSettings>(json)!;
            txtYuzuLocation.Text = settings.YuzuLocation;

            // non safe mode ui adjustments 
            if (!isLaunchedInSafeMode)
            {
                launchYuzuAfterUpdateToolStripMenuItem.Checked = settings.LaunchYuzuAfterUpdate;
                exitAfterUpdateToolStripMenuItem.Checked = settings.ExitEzYuzuAfterUpdate;
                autoupdateOnEzYuzuStartToolStripMenuItem.Checked = settings.AutoUpdateYuzuOnEzYuzuLaunch;
                enableHDRToolStripMenuItem.Checked = settings.EnableHdrSupport || File.Exists(Path.Combine(settings.YuzuLocation, "cemu.exe"));
                if (!Directory.Exists(txtYuzuLocation.Text))
                {
                    txtYuzuLocation.Clear();
                }
            }
        }

        private void ToggleUiControls(bool value)
        {
            lblYuzuLocation.Enabled = value;
            txtYuzuLocation.Enabled = value;
            btnBrowse.Enabled = value;
            grpOptions.Enabled = value;
            optionsToolStripMenuItem.Enabled = value;
        }

        private void UpdateOptionsUi()
        {
            // if global toggle is false and either options have been selected 
            // expand grpOptions
            if (!grpOptionsIsExpanded && (overrideUpdateVersionToolStripMenuItem.Checked || overrideUpdateChannelToolStripMenuItem.Checked))
            {
                grpOptions.Height = expandedGrpOptionsHeight;
                this.Size = new Size(this.Size.Width, this.Size.Height + expandedGrpOptionsHeight);
                grpOptionsIsExpanded = true;
                return;
            }

            // if grpOptions has been expanded already, check if both override options have been unchecked 
            // if both override options are unchecked, then shrink form
            if (grpOptionsIsExpanded && (!overrideUpdateVersionToolStripMenuItem.Checked && !overrideUpdateChannelToolStripMenuItem.Checked))
            {
                // revert ui back to defaults 
                this.Size = new Size(this.Size.Width, this.Size.Height - expandedGrpOptionsHeight);
                grpOptionsIsExpanded = false;
                grpOptions.Height = 0;
            }
        }

        private async Task SaveApplicationSettings()
        {
            // save ui settings to EzYuzu.json 
            var appSettings = new EzYuzuSettings()
            {
                YuzuLocation = txtYuzuLocation.Text,
                LaunchYuzuAfterUpdate = launchYuzuAfterUpdateToolStripMenuItem.Checked,
                ExitEzYuzuAfterUpdate = exitAfterUpdateToolStripMenuItem.Checked,
                AutoUpdateYuzuOnEzYuzuLaunch = autoupdateOnEzYuzuStartToolStripMenuItem.Checked,
                EnableHdrSupport = enableHDRToolStripMenuItem.Checked
            };
            await using var file = File.Create(Path.Combine(AppContext.BaseDirectory, "EzYuzu.settings.json"));
            await JsonSerializer.SerializeAsync(file, appSettings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private async Task CheckAppVersionAsync()
        {
            var updater = new AppUpdater(clientFactory!);
            var currentAppVersion = await updater.CheckVersionAsync();
            switch (currentAppVersion)
            {
                case AppUpdater.CurrentVersion.UpdateAvailable:
                    MessageBox.Show("New version of EzYuzu available, please download from https://github.com/amakvana/EzYuzu", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(new ProcessStartInfo("https://github.com/amakvana/EzYuzu")
                    {
                        UseShellExecute = true,
                        Verb = "Open"
                    })?.Dispose();
                    break;
                case AppUpdater.CurrentVersion.NotSupported:
                    MessageBox.Show("This version of EzYuzu is no longer supported, please download the latest version from https://github.com/amakvana/EzYuzu", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.Start(new ProcessStartInfo("https://github.com/amakvana/EzYuzu")
                    {
                        UseShellExecute = true,
                        Verb = "Open"
                    })?.Dispose();
                    Application.Exit();
                    break;
                case AppUpdater.CurrentVersion.Undetectable:
                    MessageBox.Show("Network Connection Error! Please check your internet connection and try again.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    break;
            }
        }

        private async Task<YuzuBranch> RefreshDetectedYuzuInstallationUpdateBranchAsync()
        {
            // if no path selected
            if (string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
                return YuzuBranch.None;

            // otherwise process 
            branchDetector!.YuzuDirectoryPath = txtYuzuLocation.Text;
            var detectedYuzuBranch = await branchDetector.GetCurrentlyInstalledBranchAsync();

            // refresh "Update Channel" dropdown
            lblUpdateChannel.Text = "Update Channel (Auto-Detected):";
            cboUpdateChannel.SelectedIndex = detectedYuzuBranch switch
            {
                YuzuBranch.Mainline => 0,
                YuzuBranch.EarlyAccess => 1,
                _ => 0  // mainline by default 
            };

            return detectedYuzuBranch;
        }

        private async Task RefreshDetectedYuzuInstallationUpdateVersionsAsync(YuzuBranch detectedYuzuBranch)
        {
            // otherwise refresh "Update Version" dropdown 
            cboUpdateVersion.DataSource = await branchDetector!.GetDetectedBranchAvailableUpdateVersionsAsync(detectedYuzuBranch);
            cboUpdateVersion.DisplayMember = "Key";
            cboUpdateVersion.ValueMember = "Value";
        }

        private async Task<YuzuInstallationState> RefreshDetectedYuzuInstallationStateAsync(YuzuBranch detectedYuzuBranch)
        {
            // if no path, do nothing 
            if (string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
                return YuzuInstallationState.NoInstallDetected;

            // otherwise refresh installation state 

            string yuzuDirectoryPath = txtYuzuLocation.Text;

            // get latest version from dropdown and extract just number from tag 
            int latestVersionAvailableFromBranch = int.Parse(cboUpdateVersion.Text[(cboUpdateVersion.Text.LastIndexOf("-") + 1)..]);

            // detect whether current install is up-to-date
            var stateDetector = new YuzuInstallationStateDetector(yuzuDirectoryPath, detectedYuzuBranch);
            var installationState = await stateDetector.GetYuzuInstallationStateAsync(latestVersionAvailableFromBranch);
            var currentInstalledVersion = await stateDetector.GetCurrentYuzuInstalledVersion();

            // update ui based on installation state 
            switch (installationState)
            {
                case YuzuInstallationState.LatestVersionInstalled:
                    btnProcess.Text = "Yuzu is currently Up-To-Date!";
                    toolTip1.SetToolTip(btnProcess, "The latest version of Yuzu is currently installed");
                    btnProcess.Enabled = false;
                    break;
                case YuzuInstallationState.UpdateAvailable:
                    btnProcess.Text = $"Update from {currentInstalledVersion} to {latestVersionAvailableFromBranch}";
                    toolTip1.SetToolTip(btnProcess, "Click to download the latest version of Yuzu");
                    btnProcess.Enabled = true;
                    break;
                case YuzuInstallationState.NoInstallDetected:
                default:
                    btnProcess.Text = "New Install";
                    toolTip1.SetToolTip(btnProcess, "Yuzu not detected, click to download a fresh copy of Yuzu");
                    btnProcess.Enabled = true;

                    // default options
                    lblUpdateChannel.Text = "Update Channel:";
                    break;
            }
            return installationState;
        }

        private static bool AppIsRunningAsAdministrator()
        {
            try
            {
                using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> ShowFolderBrowserDialogWindowAndGetResultAsync()
        {
            var folder = new TaskCompletionSource<string>();
            using var fbd = new FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true,
                Description = "Browse to the folder containing yuzu.exe"
            };

            Thread t = new(() =>
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                    folder.SetResult(fbd.SelectedPath.Trim());
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            return await folder.Task;
        }

        private static bool IsControlDown()
        {
            // https://stackoverflow.com/a/27939079
            return (ModifierKeys & Keys.Control) == Keys.Control;
        }

        private static void RenameFile(string path, string oldFileName, string newFileName)
        {
            // ensure renaming doesn't happen while file is being written to filesystem 
            bool renamed = false;
            while (!renamed)
            {
                if (!File.Exists(Path.Combine(path, oldFileName)))
                    continue;

                File.Move(Path.Combine(path, oldFileName), Path.Combine(path, newFileName), true);
                renamed = true;
            }
        }
    }
}