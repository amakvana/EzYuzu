using EzYuzu.Classes.Extensions;
using System.Diagnostics;

namespace EzYuzu.Classes.Yuzu.Managers
{
    public sealed class MainlineYuzuManager : YuzuManager
    {
        private const string Quote = "\"";
        private readonly IHttpClientFactory clientFactory;
        private readonly string tagName;
        private readonly string downloadUrl;

        public MainlineYuzuManager(IHttpClientFactory clientFactory, string tagName, string downloadUrl) : base(clientFactory)
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

            // download latest version of Mainline Yuzu 
            await using (var file = new FileStream(Path.Combine(TempUpdateDirectoryPath, "yuzu.zip"), FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var progressReporter = new Progress<float>(progress =>
                {
                    var progressPercentage = (int)(progress * 100);
                    RaiseUpdateProgressDelegate(progressPercentage, $"Downloading Yuzu Mainline {versionNumber} ...");
                });
                await client.DownloadAsync(downloadUrl, file, progressReporter);
            }

            // unpack yuzu
            RaiseUpdateProgressDelegate(0, $@"Unpacking Yuzu Mainline {versionNumber} ...");
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
            CopyFiles(Path.Combine(YuzuDirectoryPath, "yuzu-windows-msvc"), YuzuDirectoryPath, true);

            // update version number
            var filePath = Path.Combine(YuzuDirectoryPath, "version");
            await File.WriteAllTextAsync(filePath, versionNumber);
            RaiseUpdateProgressDelegate(100, $@"Unpacking Yuzu Mainline {versionNumber} ...");

            // cleanup
            RaiseUpdateProgressDelegate(0, "Cleaning up ...");
            CleanUpDirectories();
            RaiseUpdateProgressDelegate(100, "Cleaning up ...");
        }
    }
}
