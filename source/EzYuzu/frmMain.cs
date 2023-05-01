using Ookii.Dialogs.WinForms;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EzYuzu
{
    public partial class FrmMain : Form
    {
        private GuiOptions guiOptions;
		private bool saveYuzuLocation = true;

        public FrmMain(GuiOptions guiOptions)
        {
            this.guiOptions = guiOptions;

			InitializeComponent();
		}

        private async void FrmMain_Load(object sender, EventArgs e)
        {
            // check if software is latest version 
            CheckAppVersion();

            // set UI defaults 
            btnProcess.Enabled = false;
            Text = Properties.strings.FormMainTitle;
            lblYuzuLocation.Text = Properties.strings.LabelYuzuLocation;
            lblUpdateChannel.Text = Properties.strings.LabelUpdateChannel;
            fileToolStripMenuItem.Text = Properties.strings.ToolStripFile;
            exitToolStripMenuItem.Text = Properties.strings.ToolStripExit;
            optionsToolStripMenuItem.Text = Properties.strings.ToolStripOptions;
            generalToolStripMenuItem.Text = Properties.strings.ToolStripGeneral;
            updateYuzuToolStripMenuItem.Text = Properties.strings.ToolStripUpdateYuzu;
            reinstallVisualCToolStripMenuItem.Text = Properties.strings.ToolStripReinstallVisualC;
            channelToolStripMenuItem.Text = Properties.strings.ToolStripUpdateChannel;
            overrideUpdateChannelToolStripMenuItem.Text = Properties.strings.ToolStripOverrideUpdateChannel;
            helpToolStripMenuItem.Text = Properties.strings.ToolStripHelp;
            yuzuWebsiteToolStripMenuItem.Text = Properties.strings.ToolStripYuzuWebsite;
            checkForUpdatesToolStripMenuItem.Text = Properties.strings.ToolStripCheckForUpdates;
            aboutToolStripMenuItem.Text = Properties.strings.ToolStripAbout;
            cboUpdateChannel.Items.AddRange(YuzuBranch.Branches.Select(b => b.branchString).ToArray());
            btnProcess.Text = Properties.strings.ButtonProcessSelectYuzuLocation;
            lblProgress.Text = "";
            toolTip1.SetToolTip(lblYuzuLocation, Properties.strings.ToolTipYuzuLocation);
            toolTip1.SetToolTip(txtYuzuLocation, Properties.strings.ToolTipYuzuLocation);
            toolTip1.SetToolTip(btnBrowse, Properties.strings.ToolTipYuzuLocation);
            toolTip1.SetToolTip(pbarCurrentProgress, Properties.strings.ToolTipCurrentProgress);
            toolTip1.SetToolTip(cboUpdateChannel, Properties.strings.ToolTipUpdateChannel);
            toolTip1.SetToolTip(lblUpdateChannel, Properties.strings.ToolTipUpdateChannel);

            if(guiOptions.YuzuLocation == null) {
				// if directory saved does not exist, clear it
				txtYuzuLocation.Text = Properties.Settings.Default?.YuzuLocation;

				if(!Directory.Exists(txtYuzuLocation.Text)) {
					txtYuzuLocation.Clear();
					Properties.Settings.Default.YuzuLocation = "";
					Properties.Settings.Default.Save();
				}
			} else {
				txtYuzuLocation.Text = guiOptions.YuzuLocation;

				if(!Directory.Exists(txtYuzuLocation.Text)) {
					txtYuzuLocation.Clear();
				} else {
					saveYuzuLocation = false;
				}
			}

            reinstallVisualCToolStripMenuItem.Checked = guiOptions.VisualC;

			// check path and refresh yuzu installation state 
			if (!string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                await RefreshYuzuInstallationStateAsync();
			}

			if(guiOptions.Mainline) {
				cboUpdateChannel.SelectedIndex = YuzuBranchDetector.GetCurrentlyInstalledBranch(YuzuBranch.Mainline);
				lblUpdateChannel.Text = Properties.strings.LabelUpdateChannel;
			} else if(guiOptions.EarlyAccess) {
				cboUpdateChannel.SelectedIndex = YuzuBranchDetector.GetCurrentlyInstalledBranch(YuzuBranch.EarlyAccess);
				lblUpdateChannel.Text = Properties.strings.LabelUpdateChannel;
			}
		}

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // save selected yuzu location 
            if(saveYuzuLocation) {
                Properties.Settings.Default.YuzuLocation = txtYuzuLocation.Text;
                Properties.Settings.Default.Save();
            }
        }

        private async void BtnBrowse_ClickAsync(object sender, EventArgs e)
        {
            // browse to Yuzu folder and select it 
            using var fbd = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true,
                Description = Properties.strings.FolderBrowserTitle
            };

            // if folder has been selected
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string yuzuLocationPath = fbd.SelectedPath.Trim();
                txtYuzuLocation.Text = yuzuLocationPath;

                saveYuzuLocation = true;

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
                btnProcess.Text = Properties.strings.ButtonProcessUpdateYuzu;
                lblUpdateChannel.Text = Properties.strings.LabelUpdateChannel;
                toolTip1.SetToolTip(btnProcess, Properties.strings.ToolTipButtonProcessUpdateYuzu);
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
                MessageBox.Show(Properties.strings.MessageBoxNoPathSelected, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Disable UI controls 
            ToggleControls(false);
            btnProcess.Enabled = false;

			string yuzuLocation = txtYuzuLocation.Text;

			if(overrideUpdateChannelToolStripMenuItem.Checked) {
				cboUpdateChannel.Enabled = false;
			}

			YuzuBranchEnum useBranch = YuzuBranchDetector.GetCurrentlyInstalledBranch(cboUpdateChannel.SelectedIndex);

			YuzuManager yuzuManager = useBranch switch
            {
                YuzuBranchEnum.EarlyAccess => new EarlyAccessYuzuManager(),
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
                    case var value when value == Properties.strings.ButtonProcessNewInstall:
                        await eaym.ProcessYuzuNewInstallationAsync();
                        break;

                    case var value when value == Properties.strings.ButtonProcessUpdateYuzu:
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
                    case var value when value == Properties.strings.ButtonProcessNewInstall:
                        await mym.ProcessYuzuNewInstallationAsync();
                        break;

                    case var value when value == Properties.strings.ButtonProcessUpdateYuzu:
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
                MessageBox.Show(Properties.strings.MessageBoxNoPathSelected, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show(Properties.strings.UpdateLatestVersion, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case AppUpdater.CurrentVersion.UpdateAvailable:
                    MessageBox.Show(Properties.strings.UpdateAvailable, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start("https://github.com/amakvana/EzYuzu");
                    break;
                case AppUpdater.CurrentVersion.NotSupported:
                    MessageBox.Show(Properties.strings.UpdateNotSupported, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.Start("https://github.com/amakvana/EzYuzu");
                    Application.Exit();
                    break;
            }
        }

        private async Task RefreshYuzuInstallationStateAsync()
        {
            string yuzuLocation = txtYuzuLocation.Text;

            // get current branch of Yuzu and set the current channel dropdown
            var currentYuzuBranch = YuzuBranchDetector.GetCurrentlyInstalledBranch(yuzuLocation);

            // get installation state of detected Yuzu install
            var installationState = await YuzuInstallationStateDetector.GetYuzuInstallationStateAsync(yuzuLocation, currentYuzuBranch);

			btnProcess.Text = installationState.installationStateShortString;
			toolTip1.SetToolTip(btnProcess, installationState.installationStateLongString);

			// alter UI based on installation state 
			switch (installationState)
            {
                case var value when value == YuzuInstallationState.LatestVersionInstalled:
                    btnProcess.Enabled = false;
                    cboUpdateChannel.SelectedIndex = (int)currentYuzuBranch;
					lblUpdateChannel.Text = Properties.strings.LabelUpdateChannelAutoDetected;
					break;
				case var value when value == YuzuInstallationState.UpdateAvailable:
                    btnProcess.Enabled = true;
                    cboUpdateChannel.SelectedIndex = (int)currentYuzuBranch;
					lblUpdateChannel.Text = Properties.strings.LabelUpdateChannelAutoDetected;
					break;
				case var value when value == YuzuInstallationState.NoInstallDetected:
                default:
                    btnProcess.Enabled = true;

                    // default options
                    cboUpdateChannel.SelectedIndex = 0;     // mainline 
					lblUpdateChannel.Text = Properties.strings.LabelUpdateChannel;
					break;
            }
        }
    }
}
