using Ookii.Dialogs.WinForms;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EzYuzu
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private async void FrmMain_Load(object sender, EventArgs e)
        {
            // check if software is latest version 
            CheckAppVersion();

            // set UI defaults 
            btnProcess.Enabled = false;
            lblProgress.Text = "";
            toolTip1.SetToolTip(lblYuzuLocation, "Click and browse to your Yuzu.exe Folder Location");
            toolTip1.SetToolTip(txtYuzuLocation, toolTip1.GetToolTip(lblYuzuLocation));
            toolTip1.SetToolTip(btnBrowse, toolTip1.GetToolTip(lblYuzuLocation));
            toolTip1.SetToolTip(pbarCurrentProgress, "Progress completed of current action");
            toolTip1.SetToolTip(cboUpdateChannel, "The current Update Channel of Yuzu. Auto-detected when the Yuzu Path is selected. \n" +
                                                  "Can be manually overridden by going into Options > General > Update Channel > Override Update Channel. \n\n" +
                                                  "Mainline - Contains all regular daily Yuzu updates. More stable. \n" +
                                                  "Early Access - Contains the latest experimental features and fixes, before they are merged into Mainline builds. Less stable. \n\n" +
                                                  "When no channel is detected, EzYuzu defaults to Mainline.");
            toolTip1.SetToolTip(lblUpdateChannel, toolTip1.GetToolTip(cboUpdateChannel));

            // if directory saved does not exist, clear it
            txtYuzuLocation.Text = Properties.Settings.Default?.YuzuLocation;
            if (!Directory.Exists(txtYuzuLocation.Text))
            {
                txtYuzuLocation.Clear();
                Properties.Settings.Default.YuzuLocation = "";
                Properties.Settings.Default.Save();
            }

            // check path and refresh yuzu installation state 
            if (!string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                await RefreshYuzuInstallationStateAsync();
            }
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save selected yuzu location 
            Properties.Settings.Default.YuzuLocation = txtYuzuLocation.Text;
            Properties.Settings.Default.Save();
        }

        private async void BtnBrowse_ClickAsync(object sender, EventArgs e)
        {
            // browse to Yuzu folder and select it 
            using var fbd = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true,
                Description = "Browse to the folder containing yuzu.exe"
            };

            // if folder has been selected
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string yuzuLocationPath = fbd.SelectedPath.Trim();
                txtYuzuLocation.Text = yuzuLocationPath;

                // refresh installation status of Yuzu within selected folder
                await RefreshYuzuInstallationStateAsync(); 
            }
        }

        private void CboUpdateChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // force refresh of Yuzu if the channel has been manually changed 
            if (overrideUpdateChannelToolStripMenuItem.Checked && cboUpdateChannel.Enabled)
            {
                btnProcess.Enabled = true;
                btnProcess.Text = "Update Yuzu";
                lblUpdateChannel.Text = "Update Channel:";
                toolTip1.SetToolTip(btnProcess, "Click to download the latest version of Yuzu");
            }
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

        private async void BtnProcess_ClickAsync(object sender, EventArgs e)
        {
            // no path selected? computer says no
            if (string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                MessageBox.Show("Please browse and select your Yuzu.exe folder location first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Disable UI controls 
            ToggleControls(false);
            btnProcess.Enabled = false;

            // detect current branch. Mainline = default
            string yuzuLocation = txtYuzuLocation.Text;
            var branchDetector = new YuzuBranchDetector(yuzuLocation);
            var detectedBranch = branchDetector.GetCurrentlyInstalledBranch();

            // check if user has overriden update channel
            if (overrideUpdateChannelToolStripMenuItem.Checked)
            {
                cboUpdateChannel.Enabled = false;
                detectedBranch = branchDetector.GetCurrentlyInstalledBranch(cboUpdateChannel.SelectedIndex);
            }
            
            YuzuManager yuzuManager = detectedBranch switch
            {
                YuzuBranchDetector.Branch.EarlyAccess => new EarlyAccessYuzuManager(),
                _ => new MainlineYuzuManager(),     // default 
            };

            // prepare YuzuManager and download all prerequisites
            yuzuManager.YuzuDirectoryPath = yuzuLocation;
            yuzuManager.TempUpdateDirectoryPath = Path.Combine(yuzuLocation, "TempUpdate");
            yuzuManager.UpdateProgress += EzYuzuDownloader_UpdateCurrentProgress;
            await yuzuManager.DownloadPrerequisitesAsync();

            // Process yuzu depending on directory selected and type of YuzuManager
            if (yuzuManager is EarlyAccessYuzuManager eaym)
            {
                switch (btnProcess.Text)
                {
                    case "New Install":
                        await eaym.ProcessYuzuNewInstallationAsync();
                        break;

                    case "Update Yuzu":
                        if (reinstallVisualCToolStripMenuItem.Checked)
                        {
                            await eaym.DownloadInstallVisualCppRedistAsync();
                        }
                        await eaym.ProcessYuzuUpdateAsync();
                        break;
                }
            }
            else if (yuzuManager is MainlineYuzuManager mym)
            {
                switch (btnProcess.Text)
                {
                    case "New Install":
                        await mym.ProcessYuzuNewInstallationAsync();
                        break;

                    case "Update Yuzu":
                        if (reinstallVisualCToolStripMenuItem.Checked)
                        {
                            await mym.DownloadInstallVisualCppRedistAsync();
                        }
                        await mym.ProcessYuzuUpdateAsync();
                        break;
                }
            }

            // Refresh directory installation status 
            await RefreshYuzuInstallationStateAsync();

            // Enable UI controls 
            ToggleControls(true);
            if (overrideUpdateChannelToolStripMenuItem.Checked)
            {
                cboUpdateChannel.Enabled = true;
            }
        }

        private void EzYuzuDownloader_UpdateCurrentProgress(int progressPercentage, string progressText)
        {
            pbarCurrentProgress.Value = progressPercentage;
            pbarCurrentProgress.Refresh();
            lblProgress.Text = progressText;
            lblProgress.Refresh();
        }

        // MENUSTRIP CONTROLS
        private void YuzuWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://yuzu-emu.org/");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var f = new FrmAbout();
            f.ShowDialog();
        }

        private void CheckForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckAppVersion(true);
        }

        private async void OverrideUpdateChannelToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            // no path selected? computer says no
            if (string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                MessageBox.Show("Please browse and select your Yuzu.exe folder location first", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // otherwise, update UI
            cboUpdateChannel.Enabled = overrideUpdateChannelToolStripMenuItem.Checked;
            if (!overrideUpdateChannelToolStripMenuItem.Checked)
                await RefreshYuzuInstallationStateAsync();
        }

        // METHODS 
        private void ToggleControls(bool value)
        {
            txtYuzuLocation.Enabled = value;
            btnBrowse.Enabled = value;
            optionsToolStripMenuItem.Enabled = value;
        }

        private void CheckAppVersion(bool manualCheck = false)
        {
            var currentAppVersion = AppUpdater.CheckVersion();
            switch (currentAppVersion)
            {
                case AppUpdater.CurrentVersion.LatestVersion when manualCheck:
                    MessageBox.Show("You currently have the latest version of EzYuzu", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case AppUpdater.CurrentVersion.UpdateAvailable:
                    MessageBox.Show("New version of EzYuzu available, please download from https://github.com/amakvana/EzYuzu", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start("https://github.com/amakvana/EzYuzu");
                    break;
                case AppUpdater.CurrentVersion.NotSupported:
                    MessageBox.Show("This version of EzYuzu is no longer supported, please download the latest version from https://github.com/amakvana/EzYuzu", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.Start("https://github.com/amakvana/EzYuzu");
                    Application.Exit();
                    break;
            }
        }

        private async Task RefreshYuzuInstallationStateAsync()
        {
            string yuzuLocation = txtYuzuLocation.Text;

            // get current branch of Yuzu and set the current channel dropdown 
            var branchDetector = new YuzuBranchDetector(yuzuLocation);
            var currentYuzuBranch = branchDetector.GetCurrentlyInstalledBranch();
            lblUpdateChannel.Text = "Update Channel (Auto-Detected):";

            // get installation state of detected Yuzu install 
            var stateDetector = new YuzuInstallationStateDetector(yuzuLocation, currentYuzuBranch);
            var installationState = await stateDetector.GetYuzuInstallationStateAsync();

            // alter UI based on installation state 
            switch (installationState)
            {
                case YuzuInstallationStateDetector.YuzuInstallationState.LatestVersionInstalled:
                    btnProcess.Text = "Yuzu is currently Up-To-Date!";
                    toolTip1.SetToolTip(btnProcess, "The latest version of Yuzu is currently installed");
                    btnProcess.Enabled = false;
                    cboUpdateChannel.SelectedIndex = (int)currentYuzuBranch;
                    break;
                case YuzuInstallationStateDetector.YuzuInstallationState.UpdateAvailable:
                    btnProcess.Text = "Update Yuzu";
                    toolTip1.SetToolTip(btnProcess, "Click to download the latest version of Yuzu");
                    btnProcess.Enabled = true;
                    cboUpdateChannel.SelectedIndex = (int)currentYuzuBranch;
                    break;
                case YuzuInstallationStateDetector.YuzuInstallationState.NoInstallDetected:
                default:
                    btnProcess.Text = "New Install";
                    toolTip1.SetToolTip(btnProcess, "Yuzu not detected, click to download a fresh copy of Yuzu");
                    btnProcess.Enabled = true;

                    // default options
                    lblUpdateChannel.Text = "Update Channel:";
                    cboUpdateChannel.SelectedIndex = 0;     // mainline 
                    break;
            }
        }
    }
}
