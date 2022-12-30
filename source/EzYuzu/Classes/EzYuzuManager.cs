using Microsoft.Win32;
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

namespace EzYuzu
{
    public class EzYuzuManager
    {
        private const string BaseYuzuRepoUrl = "https://github.com/yuzu-emu/yuzu-mainline/releases/latest";
        private const string Quote = "\"";
        private string _sevenZipExePath = "";

        public EzYuzuManager() { }

        public delegate void UpdateProgressDelegate(int progressPercentage, string progressText);

        public event UpdateProgressDelegate UpdateProgress;

        /// <summary>
        /// States of Yuzu that's currently installed 
        /// </summary>
        public enum YuzuInstallationState
        {
            LatestVersionInstalled,
            UpdateAvailable,
            NoInstallDetected
        }

        public string YuzuDirectoryPath { get; set; } = "";

        public string TempUpdateDirectoryPath { get; set; } = "";

        /// <summary>
        /// Downloads all prequisites for EzYuzu to function. Includes 7zip and Visual C++
        /// </summary>
        /// <returns></returns>
        public async Task DownloadPrerequisitesAsync()
        {
            CloseYuzu();
            PrepareTempUpdateFolder();

            if (!IsSevenZipInstalled())
                await DownloadSevenZipAsync();

            if (!IsVisualCppRedistInstalled())
                await DownloadInstallVisualCppRedistAsync();
        }

        /// <summary>
        /// Checks to see whether the current version of Yuzu within the specified Yuzu Directory Path is up-to-date
        /// </summary>
        /// <returns>The <c>YuzuInstallationState</c> state</returns>
        public async Task<YuzuInstallationState> GetYuzuInstallationStateAsync()
        {
            // if yuzu.exe doesn't exist in the current directory, it's not setup 
            if (!File.Exists($@"{YuzuDirectoryPath}\yuzu.exe"))
            {
                return YuzuInstallationState.NoInstallDetected;
            }

            // yuzu.exe exists 

            // if no version file, assume out of date  
            if (!File.Exists($@"{YuzuDirectoryPath}\version"))
            {
                return YuzuInstallationState.UpdateAvailable;
            }

            // yuzu.exe and version file exists 

            // check current installed version against latest online version 
            using (var f = File.OpenRead($@"{YuzuDirectoryPath}\version"))
            using (var reader = new StreamReader(f))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    int currentVersion = int.Parse(line.Trim());
                    int latestVersion = FetchLatestYuzuVersionNumber();

                    return (currentVersion == latestVersion)
                        ? YuzuInstallationState.LatestVersionInstalled
                        : YuzuInstallationState.UpdateAvailable;
                }
            }

            // fallback 
            return YuzuInstallationState.NoInstallDetected;
        }

        /// <summary>
        /// Installs Yuzu from scratch, set up User directory and pulls optimised GPU configurations
        /// </summary>
        /// <returns></returns>
        public async Task ProcessYuzuNewInstallationAsync()
        {
            // clean out the old yuzu directory & configs
            var configDirs = new List<string>()
            {
                { $@"{YuzuDirectoryPath}\User\config" },
                { $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\yuzu\config" }
            };
            foreach (string dir in configDirs)
            {
                if (Directory.Exists(dir))
                {
                    Directory.Delete(dir, true);
                }
            }

            // setup User folders
            Directory.CreateDirectory($@"{YuzuDirectoryPath}\User");
            Directory.CreateDirectory($@"{YuzuDirectoryPath}\User\keys");
            Directory.CreateDirectory($@"{YuzuDirectoryPath}\User\config");
            
            // install latest vc++, yuzu and pull gpu configs
            await DownloadInstallVisualCppRedistAsync();
            await ProcessYuzuUpdateAsync();
            await GetGPUConfigAsync();
        }

        /// <summary>
        /// Downloads the latest version of Yuzu and unpacks it in <c>YuzuDirectoryPath</c>
        /// </summary>
        /// <returns></returns>
        public async Task ProcessYuzuUpdateAsync()
        {
            // Downloads the latest version of Yuzu from GitHub
            // Unpacks it into Yuzu Directory Path 
            // Extracts and writes version into 'version' file

            // get latest yuzu version & download 
            using var client = new WebClient();

            // fetch latest Yuzu release 
            string repo = GetRedirectedUrl("https://github.com/yuzu-emu/yuzu-mainline/releases/latest");

            // 21/09/22 - github now uses expanded_assets in the url to show the assets. 
            // we grab the redirected url from /latest/
            // parse the last / as it contains the version url then prepend the word "expanded_assets" to it 
            // join back onto "https://github.com/yuzu-emu/yuzu-mainline/releases"
            string mainLineVersion = repo.Substring(repo.LastIndexOf('/') + 1).Trim();
            repo = $@"https://github.com/yuzu-emu/yuzu-mainline/releases/expanded_assets/{mainLineVersion}";

            // get html from new url structure
            string repoHtml = client.DownloadString(repo);
            string latestYuzu = "https://github.com";
            int version = FetchLatestYuzuVersionNumber();
            Regex r = new Regex(@"(?:\/yuzu-emu\/yuzu-mainline\/releases\/download\/[^""]+)");
            foreach (Match m in r.Matches(repoHtml))
            {
                string url = m.Value.Trim();

                // since 7z is used, prefer the 7z url - faster 
                if (url.EndsWith(".7z", StringComparison.Ordinal))
                {
                    latestYuzu += url;
                }
            }

            // download it 
            client.DownloadFileCompleted += (s, e) =>
            {
                // unpack 
                UpdateProgress(0, $@"Unpacking Yuzu {version} ...");

                // unzip downloaded zip using 7zip 
                var psi = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = _sevenZipExePath,
                    Arguments = $@"x {Quote}{TempUpdateDirectoryPath}\yuzu.7z{Quote} -o{Quote}{YuzuDirectoryPath}{Quote} -aoa"
                };
                using (var p = Process.Start(psi))
                {
                    p.WaitForExit();
                }

                // move archive contents from yuzu-windows-msvc into YuzuDirectoryPath
                DirectoryUtilities.Copy($@"{YuzuDirectoryPath}\yuzu-windows-msvc", YuzuDirectoryPath, true);

                // update version number 
                using (var f = new FileStream($@"{YuzuDirectoryPath}\version", FileMode.OpenOrCreate))
                using (var sw = new StreamWriter(f))
                {
                    sw.Write(version);
                }

                UpdateProgress(100, "Done");

                // cleanup 
                UpdateProgress(0, "Cleaning up ...");
                CleanUpDirectories();
                UpdateProgress(100, "Done");

            };
            client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, $@"Downloading Yuzu {version} ...");
            await client.DownloadFileTaskAsync(new Uri(latestYuzu), $@"{TempUpdateDirectoryPath}\yuzu.7z");
        }

        /// <summary>
        /// Downloads and Installs Visual C++
        /// </summary>
        /// <returns></returns>
        public async Task DownloadInstallVisualCppRedistAsync()
        {
            using var client = new WebClient();
            client.DownloadFileCompleted += (s, e) =>
            {
                // install visual c++
                UpdateProgress(0, "Installing Visual C++ ...");
                var psi = new ProcessStartInfo
                {
                    FileName = $@"{TempUpdateDirectoryPath}\vc_redist.x64.exe",
                    Arguments = "/install /quiet /norestart",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                p.WaitForExit();
                UpdateProgress(100, "Done");
            };
            client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, "Downloading Visual C++ ...");
            await client.DownloadFileTaskAsync(new Uri("https://aka.ms/vs/16/release/vc_redist.x64.exe"), $@"{TempUpdateDirectoryPath}\vc_redist.x64.exe");
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
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Name"].ToString().ToLower().Contains("nvidia"))
                    {
                        useOpenGL = true;
                    }
                }
            }

            // fetch optimised config based on installed GPU
            using var client = new WebClient();
            
            // download it 
            client.DownloadFileCompleted += (s, e) =>
            {
                UpdateProgress(100, "Done");
            };
            client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, "Downloading Optimised GPU Config ...");

            string gpuConfigUrl = "https://github.com/amakvana/EzYuzu/raw/master/configs/";
            gpuConfigUrl += useOpenGL ? "opengl.ini" : "vulkan.ini";
            await client.DownloadFileTaskAsync(new Uri(gpuConfigUrl), $@"{YuzuDirectoryPath}\User\config\qt-config.ini");
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
            
            // terminate each running yuzu process
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
        /// Clears all Temporary directories used during the Updating process
        /// </summary>
        private void CleanUpDirectories()
        {
            Directory.Delete($@"{YuzuDirectoryPath}\yuzu-windows-msvc", true);
            Directory.Delete(TempUpdateDirectoryPath, true);
            Directory.EnumerateFiles(YuzuDirectoryPath, "*.xz").ToList().ForEach(item => File.Delete(item));
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
        /// Fetches the latest version number of Yuzu online
        /// </summary>
        /// <returns>The version number</returns>
        private int FetchLatestYuzuVersionNumber()
        {
            string url = GetRedirectedUrl(BaseYuzuRepoUrl);
            string mainLineVersion = url.Substring(url.LastIndexOf('/') + 1).Trim();
            mainLineVersion = mainLineVersion.Substring(mainLineVersion.LastIndexOf('-') + 1).Trim();
            return int.Parse(mainLineVersion);
        } 

        /// <summary>
        /// Checks if 7-Zip is currently installed on this machine.
        /// </summary>
        /// <returns>
        /// <c>true</c> if installed; otherwise <c>false</c>.
        /// </returns>
        private bool IsSevenZipInstalled()
        {
            string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

            if (programFiles != null && File.Exists($@"{programFiles}\7-Zip\7z.exe"))
            {
                _sevenZipExePath = $@"{programFiles}\7-Zip\7z.exe";
                return true;
            }
            if (programFilesX86 != null && File.Exists($@"{programFilesX86}\7-Zip\7z.exe"))
            {
                _sevenZipExePath = $@"{programFilesX86}\7-Zip\7z.exe";
                return true;
            }
            return false;
        }

        /// <summary>
        /// Downloads 7-Zip and extracts it inside the Yuzu User Directory path.
        /// </summary>
        /// <returns></returns>
        private async Task DownloadSevenZipAsync()
        {
            string prerequisitesLocation = $@"{TempUpdateDirectoryPath}\7z";
            string sevenZLocation = $@"{prerequisitesLocation}\7z.zip";

            _sevenZipExePath = $@"{prerequisitesLocation}\7z.exe";

            if (Directory.Exists(prerequisitesLocation))
            {
                Directory.Delete(prerequisitesLocation, true);
            }
            Directory.CreateDirectory(prerequisitesLocation);

            using var client = new WebClient();
            client.DownloadFileCompleted += (s, e) =>
            {
                UpdateProgress(0, "Unpacking prerequisites ...");
                using var archive = ZipFile.OpenRead(sevenZLocation);
                foreach (var entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine($"{prerequisitesLocation}", entry.FullName), true);
                }
                UpdateProgress(100, "Done");
            };
            client.DownloadProgressChanged += (s, e) => UpdateProgress(e.ProgressPercentage, "Downloading Prerequisites ...");
            await client.DownloadFileTaskAsync("https://github.com/amakvana/EzYuzu/raw/master/assets/7z/22.01/7z.zip", sevenZLocation);
        }

        /// <summary>
        /// Gets a redirected URL from any given URL 
        /// </summary>
        /// <param name="url">The original URL</param>
        /// <returns>The redirected URL</returns>
        // https://stackoverflow.com/a/24445779
        private string GetRedirectedUrl(string url)
        {
            string uriString = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false;  // IMPORTANT
            request.Timeout = 10000;           // timeout 10s
            request.Method = "HEAD";
            // Get the response ...
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                // Now look to see if it's a redirect
                if ((int)response.StatusCode >= 300 && (int)response.StatusCode <= 399)
                {
                    uriString = response.Headers["Location"];
                    response.Close(); // don't forget to close it - or bad things happen
                }
            }
            return uriString;
        }
    }
}
