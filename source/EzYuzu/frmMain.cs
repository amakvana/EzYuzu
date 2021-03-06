﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EzYuzu
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            // set defaults 
            lblProgress.Text = "";
            cboOptions.SelectedIndex = 0;   // option: dependencies
            txtYuzuLocation.Cursor = Cursors.Arrow;
            toolTip1.SetToolTip(lblYuzuLocation, "Click and Select your Yuzu Root directory");
            toolTip1.SetToolTip(txtYuzuLocation, "Click and Select your Yuzu Root directory");
            toolTip1.SetToolTip(btnBrowse, "Browse to your Yuzu Root directory");
            toolTip1.SetToolTip(pbarProgress, "Progress completed of current action");
            toolTip1.SetToolTip(cboOptions, "Select Download option - view README.txt for details");
            toolTip1.SetToolTip(btnCheck, "Check if Yuzu update is available");
        }

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
            using (var f = new frmAbout())
            {
                f.ShowDialog();
            }
        }

        private void LblYuzuLocation_Click(object sender, EventArgs e)
        {
            // invoke browse
            BtnBrowse_Click(sender, e);
        }

        private void TxtYuzuLocation_Click(object sender, EventArgs e)
        {
            // invoke browse
            BtnBrowse_Click(sender, e);
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Browse to Yuzu root folder";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtYuzuLocation.Text = fbd.SelectedPath;
                }
            }
        }
        
        private void CboOptions_SelectedIndexChanged(object sender, EventArgs e)
        {

            // default button properties
            btnProcess.Enabled = true;
            btnProcess.Text = "Update";

            string message;
            // update button text depending on certain selected option 
            switch (cboOptions.SelectedIndex)
            {
                case 0:     // dependencies
                    if (DependenciesInstalled())
                    {
                        btnProcess.Enabled = false;
                        btnProcess.Text = "Installed";
                    }
                    break;
                case 1:     // new install
                    message = string.Format("This will install a fresh copy of Yuzu and its Dependencies{0}{0}It will overwrite any existing files, reset configs and set optimised GPU defaults in the Yuzu root folder selected{0}{0}Do not use this to Upgrade Yuzu", Environment.NewLine);
                    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnProcess.Text = "Install";
                    break;
                case 2:     // upgrade
                    message = string.Format("This will install the latest copy of Yuzu and its Dependencies{0}{0}This will not modify any existing configurations", Environment.NewLine);
                    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnProcess.Text = "Upgrade";
                    break;
                case 3:     // yuzu
                    message = string.Format("This will only update Yuzu (FASTEST){0}{0}This will not modify any existing configurations", Environment.NewLine);
                    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            // read in current version 
            // get latest version 
            // compare 

            btnCheck.Enabled = false;
            if (!string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                string yuzuLocation = txtYuzuLocation.Text;

                if (File.Exists(yuzuLocation + "\\version"))
                {
                    // get current version 
                    string currentVersion = "";
                    string latestVersion = "";
                    using (StreamReader sr = File.OpenText(yuzuLocation + "\\version"))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            currentVersion = s;
                        }
                    }

                    // get latest version from repo
                    using (var wc = new WebClient())
                    {
                        // fetch latest Yuzu release 
                        string repo = "https://github.com/yuzu-emu/yuzu-mainline/releases/latest";
                        string repoHtml = wc.DownloadString(repo);
                        Regex r = new Regex(@"(?:\/yuzu-emu\/yuzu-mainline\/releases\/download\/[^""]+)");
                        foreach (Match m in r.Matches(repoHtml))
                        {
                            string url = m.Value.Trim();
                            if (url.Contains(".zip") && !url.Contains("debugsymbols"))
                            {
                                latestVersion = url.Split('/')[5].Trim().Remove(0, 11);
                            }
                        }
                    }

                    // compare & tell user 
                    string message = (currentVersion == latestVersion) ? "You have the latest version of Yuzu!" : "Update available, please select Yuzu from the dropdown and click Update";
                    MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to get version, please select Yuzu from the dropdown and click Update", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select your Yuzu root folder", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnCheck.Enabled = true;
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            // Processes action based on cboOptions choice 
            if (!string.IsNullOrWhiteSpace(txtYuzuLocation.Text))
            {
                string yuzuLocation = txtYuzuLocation.Text;

                switch (cboOptions.SelectedIndex)
                {
                    case 0:
                        _ = ProcessDependencies(yuzuLocation);
                        break;
                    case 1:     // new install
                        ProcessNewInstall(yuzuLocation);
                        break;
                    case 2:     // upgrade
                        ProcessUpgrade(yuzuLocation);
                        break;
                    case 3:     // yuzu
                        _ = ProcessYuzu(yuzuLocation);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Please select your Yuzu root folder", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ProcessDependencies(string yuzuLocation)
        {
            btnProcess.Enabled = false;
            // create temp directory for downloads 
            string tempDir = yuzuLocation + "\\TempUpdate";
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            // download vc++
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += (s, e) =>
                {
                    // install 
                    SetProgressLabelStatus("Installing Visual C++ 2019 ...");
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = tempDir + "\\vc_redist.x64.exe";
                        p.StartInfo.Arguments = "/install /quiet";
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        p.Start();
                        p.WaitForExit();
                    }

                    // cleanup 
                    try
                    {
                        Directory.Delete(tempDir, true);
                    }
                    catch { }
                    SetProgressLabelStatus("Installed!");
                    btnProcess.Enabled = true;
                    pbarProgress.Value = 0;
                };
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                SetProgressLabelStatus("Downloading Visual C++ 2019 ...");
                await wc.DownloadFileTaskAsync(new Uri("https://aka.ms/vs/16/release/vc_redist.x64.exe"), tempDir + "\\vc_redist.x64.exe");
            }
        }

        private void ProcessNewInstall(string yuzuLocation)
        {
            // clean out the old yuzu directory & configs
            var configDirs = new List<string>()
            {
                { yuzuLocation + "\\User\\config" },
                { Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\yuzu\\config" }
            };
            foreach (string dir in configDirs)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }

            // download vc++, yuzu and setup User folder 
            Directory.CreateDirectory(yuzuLocation + "\\User");
            Directory.CreateDirectory(yuzuLocation + "\\User\\keys");
            Directory.CreateDirectory(yuzuLocation + "\\User\\config");
            ProcessUpgrade(yuzuLocation, true);
        }

        private async void ProcessUpgrade(string yuzuLocation, bool newInstall = false)
        {
            // Update dependencies and Yuzu but DO NOT touch configs
            await Task.WhenAll(ProcessDependencies(yuzuLocation));
            await Task.WhenAll(ProcessYuzu(yuzuLocation));
            if (newInstall)
            {
                await Task.WhenAll(GetGPUConfig(yuzuLocation));
            }
        }

        /// <summary>
        /// Download latest Yuzu & unpack
        /// </summary>
        /// <param name="yuzuLocation">Path to store Yuzu</param>
        private async Task ProcessYuzu(string yuzuLocation)
        {
            btnProcess.Enabled = false;
            // create temp directory for downloads 
            string tempDir = yuzuLocation + "\\TempUpdate";
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            // get latest yuzu version & download 
            using (var wc = new WebClient())
            {
                // fetch latest Yuzu release 
                string latestYuzu = "https://github.com";
                string repo = "https://github.com/yuzu-emu/yuzu-mainline/releases/latest";
                string repoHtml = wc.DownloadString(repo);
                string version = "";
                Regex r = new Regex(@"(?:\/yuzu-emu\/yuzu-mainline\/releases\/download\/[^""]+)");
                foreach (Match m in r.Matches(repoHtml))
                {
                    string url = m.Value.Trim();
                    if (url.Contains(".zip") && !url.Contains("debugsymbols"))
                    {
                        latestYuzu += url;
                        version = url.Split('/')[5].Trim().Remove(0, 11);
                    }
                }

                // download it 
                wc.DownloadFileCompleted += (s, e) =>
                {
                    // unpack 
                    SetProgressLabelStatus("Unpacking Yuzu ...");
                    ZipFile.ExtractToDirectory(tempDir + "\\yuzu.zip", yuzuLocation);

                    // update version number 
                    using (StreamWriter sw = File.CreateText(yuzuLocation + "\\version"))
                    {
                        sw.Write(version);
                    }

                    // cleanup 
                    DirectoryUtilities.Copy(yuzuLocation + "\\yuzu-windows-msvc", yuzuLocation, true);
                    Directory.Delete(yuzuLocation + "\\yuzu-windows-msvc", true);
                    Directory.Delete(tempDir, true);
                    Directory.EnumerateFiles(yuzuLocation, "*.xz").ToList().ForEach(item => File.Delete(item));
                    SetProgressLabelStatus("Done!");
                    btnProcess.Enabled = true;
                    pbarProgress.Value = 0;
                };
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                SetProgressLabelStatus("Downloading Yuzu ...");
                await wc.DownloadFileTaskAsync(new Uri(latestYuzu), tempDir + "\\yuzu.zip");
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbarProgress.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Checks if VC++ is installed
        /// </summary>
        /// <returns>true if VC++ is installed, else returns false</returns>
        private bool DependenciesInstalled()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false);
            return (key != null) ? key.GetValue("Version").ToString().StartsWith("14") : false;
        }

        private async Task GetGPUConfig(string yuzuLocation)
        {
            btnProcess.Enabled = false;
            // detect current GPU
            bool useOpenGL = false;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Name"].ToString().ToLower().Contains("nvidia"))
                    {
                        useOpenGL = true;
                    }
                }
            }

            // fetch optimised config based on installed GPU
            using (var wc = new WebClient())
            {
                // download it 
                wc.DownloadFileCompleted += (s, e) =>
                {
                    SetProgressLabelStatus("Done!");
                    btnProcess.Enabled = true;
                    pbarProgress.Value = 0;
                };
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;

                string gpuConfigUrl = "https://github.com/amakvana/EzYuzu/raw/master/configs/";
                gpuConfigUrl += (useOpenGL) ? "opengl.ini" : "vulkan.ini";
                SetProgressLabelStatus("Downloading Optimised GPU Config ...");
                await wc.DownloadFileTaskAsync(new Uri(gpuConfigUrl), yuzuLocation + "\\User\\config\\qt-config.ini");
            }
        }

        private void SetProgressLabelStatus(string status)
        {
            lblProgress.Text = status;
            lblProgress.Refresh();
        }
    }
}
