using EzYuzu.Classes.Extensions;
using System.Diagnostics;
using System.IO.Compression;
using System.Management;

namespace EzYuzu.Classes.Yuzu.Managers
{
    public class YuzuManager
    {
        private readonly IHttpClientFactory clientFactory;

        protected YuzuManager(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        protected internal delegate void UpdateProgressDelegate(int progressPercentage, string progressText);

        protected internal event UpdateProgressDelegate? UpdateProgress;

        protected void RaiseUpdateProgressDelegate(int progressPercentage, string progressText)
        {
            UpdateProgress?.Invoke(progressPercentage, progressText);
        }

        protected string SevenZipExeFilePath { get; private set; } = "";

        protected internal string YuzuDirectoryPath { get; set; } = "";

        protected internal string TempUpdateDirectoryPath { get; set; } = "";

        protected internal bool UpdateVisualCppRedistributables { get; set; } = false;

        /// <summary>
        /// Downloads all prequisites for EzYuzu to function. Includes 7zip and Visual C++
        /// </summary>
        /// <returns></returns>
        protected internal async Task DownloadPrerequisitesAsync()
        {
            // closes yuzu & creates tempupdate folders
            CloseYuzu();
            PrepareTempUpdateFolder();

            // downloads 7zip if not installed
            if (!IsSevenZipInstalled())
                await DownloadSevenZipAsync();

            // download and install visual c++, 
            if (UpdateVisualCppRedistributables)
                await DownloadInstallVisualCppRedistAsync();
        }

        /// <summary>
        /// Installs Yuzu from scratch, set up User directory and pulls optimised GPU configurations
        /// </summary>
        /// <returns></returns>
        protected async Task ProcessYuzuNewInstallationAsync()
        {
            // Clean out the old yuzu directory & configs incase of previous broken yuzu install 
            string[] configDirs = {
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

            // pull gpu configs
            await GetGPUConfigAsync();
        }

        /// <summary>
        /// Clears all Temporary directories used during the Updating process
        /// </summary>
        protected void CleanUpDirectories()
        {
            foreach (string directoryPath in Directory.EnumerateDirectories(YuzuDirectoryPath, "yuzu-windows-msvc*"))
            {
                Directory.Delete(directoryPath, true);
            }
            foreach (string filePath in Directory.EnumerateFiles(YuzuDirectoryPath, "*.xz"))
            {
                File.Delete(filePath);
            }
            Directory.Delete(TempUpdateDirectoryPath, true);
        }

        /// <summary>
        /// Copies all files and folders within the source path provided to the destination path provided 
        /// </summary>
        /// <param name="fromFolder">Source folder path</param>
        /// <param name="toFolder">Destination folder path</param>
        /// <param name="overwrite">Overwrite existing files in destination</param>
        protected static void CopyFiles(string fromFolder, string toFolder, bool overwrite = false)
        {
            // https://stackoverflow.com/a/49570235
            Directory
                .EnumerateFiles(fromFolder, "*.*", SearchOption.AllDirectories)
                .Where(file => (File.GetAttributes(file) & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                .AsParallel()
                .ForAll(from =>
                {
                    var to = from.Replace(fromFolder, toFolder);

                    // Create directories if required
                    var toSubFolder = Path.GetDirectoryName(to);
                    if (!string.IsNullOrWhiteSpace(toSubFolder))
                    {
                        Directory.CreateDirectory(toSubFolder);
                    }

                    File.Copy(from, to, overwrite);
                });
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
                foreach (var obj in searcher.Get())
                {
                    if (obj["Name"] is not null && obj["Name"].ToString()!.Contains("nvidia", StringComparison.OrdinalIgnoreCase))
                    {
                        useOpenGL = true;
                        break;
                    }
                }
            }

            // fetch optimised config based on installed GPU
            string gpuConfigIni = useOpenGL ? "opengl.ini" : "vulkan.ini";
            var client = clientFactory.CreateClient("GitHub-EzYuzu");
            await using var file = new FileStream(Path.Combine(YuzuDirectoryPath, "User", "config", "qt-config.ini"), FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            var progressReporter = new Progress<float>(progress =>
            {
                var progressPercentage = (int)(progress * 100);
                RaiseUpdateProgressDelegate(progressPercentage, $@"Downloading Optimised GPU Config ...");
            });
            await client.DownloadAsync($"configs/{gpuConfigIni}", file, progressReporter);
        }

        /// <summary>
        /// Downloads and Installs Visual C++
        /// </summary>
        /// <returns></returns>
        private async Task DownloadInstallVisualCppRedistAsync()
        {
            string fileName = "vc_redist.x64.exe";
            string vcRedistPath = Path.Combine(TempUpdateDirectoryPath, fileName);

            // download visual c++
            var client = clientFactory.CreateClient();
            await using (var file = new FileStream(vcRedistPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var progressReporter = new Progress<float>(progress =>
                {
                    var progressPercentage = (int)(progress * 100);
                    RaiseUpdateProgressDelegate(progressPercentage, $@"Downloading {fileName} ...");
                });
                await client.DownloadAsync("https://aka.ms/vs/16/release/vc_redist.x64.exe", file, progressReporter);
            }

            // install visual c++
            RaiseUpdateProgressDelegate(0, $"Installing {fileName} ...");
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = vcRedistPath,
                Arguments = "/install /quiet /norestart",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            using (var p = Process.Start(psi)!)
            {
                await p.WaitForExitAsync();
            }
            RaiseUpdateProgressDelegate(100, $"Installing {fileName} ...");
        }

        /// <summary>
        /// Close all running Yuzu processes 
        /// </summary>
        /// <returns></returns>
        private static void CloseYuzu()
        {
            // get all running yuzu processes 
            var procs = Process.GetProcessesByName("yuzu");

            // if none, return 
            if (procs is null)
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
            Directory.CreateDirectory(TempUpdateDirectoryPath);
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
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "7-Zip", "7z.exe")
            };

            foreach (string location in possibleLocations)
            {
                if (File.Exists(location))
                {
                    SevenZipExeFilePath = location;
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
            SevenZipExeFilePath = Path.Combine(prerequisitesLocation, "7z.exe");

            // if 7z was already downloaded before, use that 
            if (File.Exists(SevenZipExeFilePath))
                return;

            // downloading prerequisities
            Directory.CreateDirectory(prerequisitesLocation);
            var client = clientFactory.CreateClient("GitHub-EzYuzu");
            await using (var file = new FileStream(sevenZLocation, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var progressReporter = new Progress<float>(progress =>
                {
                    var progressPercentage = (int)(progress * 100);
                    RaiseUpdateProgressDelegate(progressPercentage, $@"Downloading 7-Zip ...");
                });
                await client.DownloadAsync("assets/7z/22.01/7z.zip", file, progressReporter);
            }

            // unpacking prerequisites
            using (var archive = ZipFile.OpenRead(sevenZLocation))
            {
                int totalFiles = archive.Entries.Count;
                int copiedFiles = 0;
                foreach (var entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine($"{prerequisitesLocation}", entry.FullName), true);
                    copiedFiles++;
                    int progressPercentage = (int)((double)copiedFiles / totalFiles * 100);
                    RaiseUpdateProgressDelegate(progressPercentage, "Unpacking 7-Zip ...");
                }
            }
        }
    }
}
