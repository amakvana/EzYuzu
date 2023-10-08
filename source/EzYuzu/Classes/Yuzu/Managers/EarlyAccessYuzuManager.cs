using EzYuzu.Classes.Extensions;
using System.Diagnostics;

namespace EzYuzu.Classes.Yuzu.Managers
{
    public sealed class EarlyAccessYuzuManager : YuzuManager
    {
        private const string Quote = "\"";
        private readonly IHttpClientFactory clientFactory;
        private readonly string tagName;
        private readonly string downloadUrl;

        public EarlyAccessYuzuManager(IHttpClientFactory clientFactory, string tagName, string downloadUrl) : base(clientFactory)
        {
            this.clientFactory = clientFactory;
            this.tagName = tagName;
            this.downloadUrl = downloadUrl;
        }

        public new async Task ProcessYuzuNewInstallationAsync()
        {
            // prepare folders and pull configs 
            await base.ProcessYuzuNewInstallationAsync();

            // pull latest copy of Yuzu 
            await ProcessYuzuUpdateAsync();
        }

        public async Task ProcessYuzuUpdateAsync()
        {
            string versionNumber = tagName[(tagName.LastIndexOf("-") + 1)..].Trim();
            var client = clientFactory.CreateClient();

            // download latest version of EA Yuzu 
            await using (var file = new FileStream(Path.Combine(TempUpdateDirectoryPath, "yuzu.zip"), FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var progressReporter = new Progress<float>(progress =>
                {
                    var progressPercentage = (int)(progress * 100);
                    RaiseUpdateProgress(progressPercentage, $"Downloading Yuzu Early Access {versionNumber} ...");
                });
                await client.DownloadAsync(downloadUrl, file, progressReporter);
            }

            // unpack yuzu
            RaiseUpdateProgress(0, $@"Unpacking Yuzu Early Access {versionNumber} ...");
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = SevenZipExeFilePath,
                Arguments = $@"x {Quote}{Path.Combine(TempUpdateDirectoryPath, "yuzu.zip")}{Quote} -o{Quote}{YuzuDirectoryPath}{Quote} -aoa"
            };
            using (var p = Process.Start(psi)!)
            {
                await p.WaitForExitAsync();
            }

            // move contents from subdirectory up one level 
            CopyFiles(Path.Combine(YuzuDirectoryPath, "yuzu-windows-msvc-early-access"), YuzuDirectoryPath, true);

            // update version number
            var filePath = Path.Combine(YuzuDirectoryPath, "version");
            await File.WriteAllTextAsync(filePath, versionNumber);
            RaiseUpdateProgress(100, $@"Unpacking Yuzu Early Access {versionNumber} ...");

            // cleanup
            RaiseUpdateProgress(0, "Cleaning up ...");
            CleanUpDirectories();
            RaiseUpdateProgress(100, "Cleaning up ...");
        }
    }
}
