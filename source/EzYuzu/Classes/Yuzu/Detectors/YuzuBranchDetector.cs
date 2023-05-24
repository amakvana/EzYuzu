using EzYuzu.Classes.Entities;
using System.Diagnostics;
using System.Text.Json;

namespace EzYuzu.Classes.Yuzu.Detectors
{
    public sealed class YuzuBranchDetector
    {
        private readonly IHttpClientFactory clientFactory;

        public YuzuBranchDetector(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public enum YuzuBranch
        {
            Mainline,
            EarlyAccess,
            None
        }

        public string YuzuDirectoryPath { get; set; } = "";

        /// <summary>
        /// Gets the current branch from yuzu-cmd
        /// </summary>
        /// <returns>Branch</returns>
        public async Task<YuzuBranch> GetCurrentlyInstalledBranchAsync()
        {
            string yuzuCmdPath = Path.Combine(this.YuzuDirectoryPath, "yuzu-cmd.exe");

            // if yuzu-cmd not detected, return no branch 
            if (!File.Exists(yuzuCmdPath))
                return YuzuBranch.None;

            // if yuzu-cmd exists, run it with --version switch and get branch 
            var currBranch = YuzuBranch.None;
            var psi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = yuzuCmdPath,
                Arguments = "--version"
            };

            using (var p = new Process())
            {
                p.OutputDataReceived += (s, e) =>
                {
                    if (e.Data is not null)
                    {
                        if (e.Data.Contains("early-access", StringComparison.OrdinalIgnoreCase))
                            currBranch = YuzuBranch.EarlyAccess;

                        if (e.Data.Contains("mainline", StringComparison.OrdinalIgnoreCase))
                            currBranch = YuzuBranch.Mainline;
                    }
                };

                p.StartInfo = psi;
                p.Start();
                p.BeginOutputReadLine();
                await p.WaitForExitAsync().ConfigureAwait(false);
            }
            return currBranch;
        }

        public async Task<List<KeyValuePair<string, string>>> GetDetectedBranchAvailableUpdateVersionsAsync(YuzuBranch detectedYuzuBranch)
        {
            // detect branch & update repo url 
            string repoUrl = detectedYuzuBranch switch
            {
                YuzuBranch.Mainline => "repos/yuzu-emu/yuzu-mainline/releases",
                YuzuBranch.EarlyAccess => "repos/pineappleEA/pineapple-src/releases",
                _ => "repos/yuzu-emu/yuzu-mainline/releases"    // mainline fallback
            };

            string repoUrlFileExt = detectedYuzuBranch switch
            {
                YuzuBranch.Mainline => ".7z",
                YuzuBranch.EarlyAccess => ".zip",
                _ => ".7z"    // mainline fallback
            };

            // pull releases from github json 
            using var client = clientFactory.CreateClient("GitHub-Api");
            using var json = await client.GetStreamAsync(repoUrl);
            var repoData = await JsonSerializer.DeserializeAsync<IEnumerable<Repo>>(json)!;

            // parse json and return tagname and download url 
            var tagNameDownloadUrls = new List<KeyValuePair<string, string>>();
            foreach (var repo in repoData!)
            {
                if (repo.Assets is null)
                    continue;

                foreach (var asset in repo.Assets)
                {
                    if (!asset.BrowserDownloadUrl!.EndsWith(repoUrlFileExt, StringComparison.Ordinal))
                        continue;

                    // add tagname and download url into list 
                    tagNameDownloadUrls.Add(new(repo.TagName!, asset.BrowserDownloadUrl));
                }
            }

            return tagNameDownloadUrls;
        }
    }
}
