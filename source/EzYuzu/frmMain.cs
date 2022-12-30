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
        private EzYuzuManager yuzuManager;

        public FrmMain()
        {
            InitializeComponent();
        }

        private async void FrmMain_Load(object sender, EventArgs e)
        {
            // check if software is latest version 
            CheckAppVersion();

            // initialise EzYuzuManager
            yuzuManager = new EzYuzuManager();
            yuzuManager.UpdateProgress += EzYuzuDownloader_UpdateProgress;

            // set UI defaults 
            btnProcess.Enabled = false;
            lblProgress.Text = "";
            toolTip1.SetToolTip(lblYuzuLocation, "Click and browse to Yuzu.exe Folder Location");
            toolTip1.SetToolTip(txtYuzuLocation, "Click and browse to Yuzu.exe Folder Location");
            toolTip1.SetToolTip(btnBrowse, "Browse to Yuzu.exe Folder Location");
            toolTip1.SetToolTip(pbarProgress, "Progress completed of current action");

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
            // disable UI controls 
            ToggleControls(false);
            btnProcess.Enabled = false;

            // download all prerequisites
            await yuzuManager.DownloadPrerequisitesAsync();

            // process yuzu depending on directory selected 
            switch (btnProcess.Text)
            {
                case "New Install":
                    await yuzuManager.ProcessYuzuNewInstallationAsync();
                    break;

                case "Update Yuzu":
                    if (reinstallVisualCToolStripMenuItem.Checked)
                    {
                        await yuzuManager.DownloadInstallVisualCppRedistAsync();
                    }
                    await yuzuManager.ProcessYuzuUpdateAsync();
                    break;

                default:
                    // do nothing
                    break;
            }

            // refresh directory installation status 
            await RefreshYuzuInstallationStateAsync();

            // enable UI controls 
            ToggleControls(true);
        }

        private void EzYuzuDownloader_UpdateProgress(int progressPercentage, string progressText)
        {
            pbarProgress.Value = progressPercentage;
            pbarProgress.Refresh();
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

        // METHODS 
        private void ToggleControls(bool value)
        {
            txtYuzuLocation.Enabled = value;
            //btnProcess.Enabled = value;
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
            yuzuManager.YuzuDirectoryPath = txtYuzuLocation.Text;
            yuzuManager.TempUpdateDirectoryPath = $@"{txtYuzuLocation.Text}\TempUpdate";

            var installationState = await yuzuManager.GetYuzuInstallationStateAsync();
            switch (installationState)
            {
                case EzYuzuManager.YuzuInstallationState.LatestVersionInstalled:
                    btnProcess.Text = "Yuzu is currently Up-To-Date!";
                    toolTip1.SetToolTip(btnProcess, "The latest version of Yuzu is currently installed");
                    btnProcess.Enabled = false;
                    break;
                case EzYuzuManager.YuzuInstallationState.UpdateAvailable:
                    btnProcess.Text = "Update Yuzu";
                    toolTip1.SetToolTip(btnProcess, "Click to download the latest version of Yuzu");
                    btnProcess.Enabled = true;
                    break;
                case EzYuzuManager.YuzuInstallationState.NoInstallDetected:
                    btnProcess.Text = "New Install";
                    toolTip1.SetToolTip(btnProcess, "Yuzu not detected, click to download a fresh copy of Yuzu");
                    btnProcess.Enabled = true;
                    break;
                default:
                    // do nothing 
                    break;
            }
        }
    }
}
