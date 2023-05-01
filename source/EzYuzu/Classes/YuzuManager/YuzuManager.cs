using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Threading.Tasks;

namespace EzYuzu
{
    public class YuzuManager
    {
        protected YuzuManager() { }

        protected internal delegate void UpdateProgressDelegate(int progressPercentage, string progressText);

        protected internal event UpdateProgressDelegate UpdateProgress;

        protected void RaiseUpdateProgressDelegate(int progressPercentage, string progressText)
        {
            UpdateProgress?.Invoke(progressPercentage, progressText);
        }

        protected string SevenZipExeFilePath { get; private set; } = "";

        protected internal string YuzuDirectoryPath { get; set; } = "";

        protected internal string TempUpdateDirectoryPath { get; set; } = "";

        /// <summary>
        /// Downloads all prequisites for EzYuzu to function. Includes 7zip and Visual C++
        /// </summary>
        /// <returns></returns>
        protected internal async Task DownloadPrerequisitesAsync()
        {
            CloseYuzu();
            PrepareTempUpdateFolder();

            if (!IsSevenZipInstalled())
                await DownloadSevenZipAsync();

            if (!IsVisualCppRedistInstalled())
                await DownloadInstallVisualCppRedistAsync();
        }

        /// <summary>
        /// Installs Yuzu from scratch, set up User directory and pulls optimised GPU configurations
        /// </summary>
        /// <returns></returns>
        protected async Task ProcessYuzuNewInstallationAsync()
        {
            // Clean out the old yuzu directory & configs
            var configDirs = new List<string>
            {
                Path.Combine(YuzuDirectoryPath, "User", "config"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "yuzu", "config")
            };
            foreach (string dir in configDirs)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }

            // Setup User folders
            Directory.CreateDirectory(Path.Combine(YuzuDirectoryPath, "User", "keys"));
            Directory.CreateDirectory(Path.Combine(YuzuDirectoryPath, "User", "config"));

            // Install latest vc++, yuzu and pull gpu configs
            await Task.WhenAll(
                DownloadInstallVisualCppRedistAsync(),
                GetGPUConfigAsync());
        }

        /// <summary>
        /// Downloads and Installs Visual C++
        /// </summary>
        /// <returns></returns>
        protected async Task DownloadInstallVisualCppRedistAsync()
        {
            string vcRedistPath = Path.Combine(TempUpdateDirectoryPath, "vc_redist.x64.exe");
            using var client = new WebClient();
            client.DownloadFileCompleted += (s, e) =>
            {
                // install visual c++
                UpdateProgress(0, "Installing Visual C++ ...");
                var psi = new ProcessStartInfo
                {
                    FileName = vcRedistPath,
                    Arguments = "/install /quiet /norestart",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                using (var p = Process.Start(psi))
                {
                    p.WaitForExit();
                }
                UpdateProgress(100, "Done");
            };
            client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, "Downloading Visual C++ ...");
            await client.DownloadFileTaskAsync(new Uri("https://aka.ms/vs/16/release/vc_redist.x64.exe"), vcRedistPath);
        }

        /// <summary>
        /// Clears all Temporary directories used during the Updating process
        /// </summary>
        protected void CleanUpDirectories()
        {
            //Directory.Delete($@"{YuzuDirectoryPath}\yuzu-windows-msvc", true);
            Directory.Delete(TempUpdateDirectoryPath, true);
            Directory.EnumerateDirectories(YuzuDirectoryPath, "yuzu-windows-msvc*").ToList().ForEach(item => Directory.Delete(item, true));
            Directory.EnumerateFiles(YuzuDirectoryPath, "*.xz").ToList().ForEach(item => File.Delete(item));
        }

        /// <summary>
        /// Detects which GPU Manufacturer is installed and downloads appropriate configurations
        /// </summary>
        /// <returns></returns>
        private async Task GetGPUConfigAsync()
        {
            // detect current GPU
            bool useOpenGL = false;
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                {
                    if (obj["Name"].ToString().ToLower().Contains("nvidia"))
                    {
                        useOpenGL = true;
                    }
                }
            }

            // fetch optimised config based on installed GPU
            using var client = new WebClient();
            client.DownloadFileCompleted += (s, e) =>
            {
                UpdateProgress(100, "Done");
            };
            client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, "Downloading Optimised GPU Config ...");
            string gpuConfigUrl = "https://github.com/amakvana/EzYuzu/raw/master/configs/";
            gpuConfigUrl += useOpenGL ? "opengl.ini" : "vulkan.ini";
            await client.DownloadFileTaskAsync(new Uri(gpuConfigUrl), Path.Combine(YuzuDirectoryPath, "User", "config", "qt-config.ini"));
        }

        /// <summary>
        /// Close all running Yuzu processes 
        /// </summary>
        /// <returns></returns>
        private void CloseYuzu()
        {
            // get all running yuzu processes 
            var procs = Process.GetProcessesByName("yuzu");

            // if none, return 
            if (procs == null)
                return;
            
            // otherwise, terminate each running yuzu process
            foreach (var proc in procs)
            {
                if (!proc.HasExited)
                {
                    proc.Kill();
                }
                proc.Dispose();
            }
        }

        /// <summary>
        /// Prepares the Temporary folders if they don't exist
        /// </summary>
        private void PrepareTempUpdateFolder()
        {
            if (!Directory.Exists(TempUpdateDirectoryPath))
            {
                Directory.CreateDirectory(TempUpdateDirectoryPath);
            }
        }

        /// <summary>
        /// Checks if VC++ is installed
        /// </summary>
        /// <returns><c>true</c> if installed; otherwise <c>false</c></returns>.
        private bool IsVisualCppRedistInstalled()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false);
            return (key != null) && key.GetValue("Version").ToString().StartsWith("14");
        }

        /// <summary>
        /// Checks if 7-Zip is currently installed on this machine.
        /// </summary>
        /// <returns>
        /// <c>true</c> if installed; otherwise <c>false</c>.
        /// </returns>
        private bool IsSevenZipInstalled()
        {
            string[] possibleLocations = {
                @"%ProgramW6432%\7-Zip\7z.exe",
                @"%ProgramFiles(x86)%\7-Zip\7z.exe",
                @"%ProgramFiles%\7-Zip\7z.exe"
            };

            foreach (string location in possibleLocations)
            {
                string fullPath = Environment.ExpandEnvironmentVariables(location);
                if (File.Exists(fullPath))
                {
                    SevenZipExeFilePath = fullPath;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Downloads 7-Zip and extracts it inside the Yuzu User Directory path.
        /// </summary>
        /// <returns></returns>
        private async Task DownloadSevenZipAsync()
        {
            string prerequisitesLocation = Path.Combine(TempUpdateDirectoryPath, "7z");
            string sevenZLocation = Path.Combine(prerequisitesLocation, "7z.zip");

            if (Directory.Exists(prerequisitesLocation))
            {
                Directory.Delete(prerequisitesLocation, true);
            }
            Directory.CreateDirectory(prerequisitesLocation);

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, "Downloading Prerequisites ...");
                await client.DownloadFileTaskAsync("https://github.com/amakvana/EzYuzu/raw/master/assets/7z/22.01/7z.zip", sevenZLocation);
            }

            UpdateProgress(0, "Unpacking prerequisites ...");
            using (var archive = ZipFile.OpenRead(sevenZLocation))
            {
                foreach (var entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine($"{prerequisitesLocation}", entry.FullName), true);
                }
            }
            SevenZipExeFilePath = Path.Combine(prerequisitesLocation, "7z.exe");
            UpdateProgress(100, "Done");
        }
    }
}
