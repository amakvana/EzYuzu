using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EzYuzu
{
    public sealed class EarlyAccessYuzuManager : YuzuManager
    {
        private const string EarlyAccessYuzuRepoJsonUrl = "https://api.github.com/repos/pineappleEA/pineapple-src/releases/latest";
        private const string Quote = "\"";

        public EarlyAccessYuzuManager() : base() { }

        public new async Task ProcessYuzuNewInstallationAsync()
        {
            // prepare folders and pull configs 
            await base.ProcessYuzuNewInstallationAsync();

            // pull latest copy of Yuzu 
            await this.ProcessYuzuUpdateAsync();
        }

        public new async Task DownloadInstallVisualCppRedistAsync()
        {
            await base.DownloadInstallVisualCppRedistAsync();
        }

        public async Task ProcessYuzuUpdateAsync()
        {
            Repo repoData = await GetEarlyAccessGitHubRepoJsonData();

            string tagName = repoData.TagName;
            string versionNumber = tagName.Substring(tagName.LastIndexOf("-") + 1).Trim();

            // loop through the assets, find the .zip asset and process it 
            foreach (var asset in repoData?.Assets)
            {
                if (!asset.BrowserDownloadUrl.EndsWith(".zip", StringComparison.Ordinal))
                    continue;

                // process latest version of EA Yuzu 
                using var client = new WebClient();
                client.DownloadFileCompleted += (s, e) =>
                {
                    // unpack 
                    base.RaiseUpdateProgressDelegate(0, $@"Unpacking Yuzu Early Access {versionNumber} ...");

                    // unzip downloaded zip using 7zip 
                    var psi = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = base.SevenZipExeFilePath,
                        Arguments = $@"x {Quote}{Path.Combine(TempUpdateDirectoryPath, "yuzu.zip")}{Quote} -o{Quote}{YuzuDirectoryPath}{Quote} -aoa"
                    };
                    using (var p = Process.Start(psi))
                    {
                        p.WaitForExit();
                    }

                    // move archive contents from yuzu-windows-msvc-early-access into YuzuDirectoryPath
                    DirectoryUtilities.Copy(Path.Combine(YuzuDirectoryPath, "yuzu-windows-msvc-early-access"), YuzuDirectoryPath, true);

                    // update version number 
                    var filePath = Path.Combine(YuzuDirectoryPath, "version");
                    File.WriteAllText(filePath, versionNumber);
                    base.RaiseUpdateProgressDelegate(100, "Done");

                    // cleanup 
                    base.RaiseUpdateProgressDelegate(0, "Cleaning up ...");
                    base.CleanUpDirectories();
                    base.RaiseUpdateProgressDelegate(100, "Done");
                };
                client.DownloadProgressChanged += (s, e) => base.RaiseUpdateProgressDelegate(e.ProgressPercentage, $@"Downloading Yuzu Early Access {versionNumber} ...");
                await client.DownloadFileTaskAsync(new Uri(asset.BrowserDownloadUrl), Path.Combine(TempUpdateDirectoryPath, "yuzu.zip"));
            }
        }

        private async Task<Repo> GetEarlyAccessGitHubRepoJsonData()
        {
            // get the latest json manifest from repo 
            // convert it back into Repo object 
            using var client = new WebClient();
            client.Headers.Add("User-Agent", "request");
            string json = await client.DownloadStringTaskAsync(new Uri(EarlyAccessYuzuRepoJsonUrl));
            using var stream = new MemoryStream(Encoding.Default.GetBytes(json));
            return await JsonSerializer.DeserializeAsync<Repo>(stream).ConfigureAwait(false);
        }
    }
}
