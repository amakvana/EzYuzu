using EzYuzu.Classes.Yuzu.Detectors;
using EzYuzu.Classes.Yuzu.Managers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Security.Principal;
using static EzYuzu.Classes.Yuzu.Detectors.YuzuBranchDetector;
using static EzYuzu.Classes.Yuzu.Detectors.YuzuInstallationStateDetector;

namespace EzYuzu.Classes.CLOptions
{
    public sealed class YuzuCommandLineUpdater
    {
        private readonly IHttpClientFactory? clientFactory;

        public YuzuCommandLineUpdater(IServiceProvider serviceProvider)
        {
            clientFactory = serviceProvider.GetService<IHttpClientFactory>();
        }

        public async Task ProcessYuzuDirectory(string? yuzuLocationPath, YuzuBranch branch, int updateVersion, bool launchYuzu)
        {
            // if no path passed, exit method 
            if (yuzuLocationPath is null)
                return;

            // if no branch passed in, auto detect branch
            var branchDetector = new YuzuBranchDetector(clientFactory!)
            {
                YuzuDirectoryPath = yuzuLocationPath
            };
            if (branch == YuzuBranch.None)
            {
                branch = await branchDetector.GetCurrentlyInstalledBranchAsync();
            }

            // get available update versions based on branch passed in 
            var availableBranchVersions = await branchDetector.GetDetectedBranchAvailableUpdateVersionsAsync(branch);

            // if no version parameter has been passed in or the value is less than 0, get latest version info
            var latestVersionTagNameAndDownloadUrl = availableBranchVersions.First();
            switch (updateVersion)
            {
                case > 0:
                    // if version parameter has been passed in, loop through availableBranchVersions and extract data from chosen version
                    foreach (var version in availableBranchVersions)
                    {
                        int currentVersionNumber = int.Parse(version.Key[(version.Key.LastIndexOf("-") + 1)..]);

                        // if the current version isn't the version we want, go again
                        if (currentVersionNumber != updateVersion)
                            continue;

                        // current version iterated is a match, store details and break the loop
                        latestVersionTagNameAndDownloadUrl = version;
                        break;
                    }
                    break;
            }

            int latestVersionAvailableFromBranch = int.Parse(latestVersionTagNameAndDownloadUrl.Key[(latestVersionTagNameAndDownloadUrl.Key.LastIndexOf("-") + 1)..]);

            // detect whether current install is up-to-date
            // if not, process update or new install
            var stateDetector = new YuzuInstallationStateDetector(yuzuLocationPath, branch);
            var installationState = await stateDetector.GetYuzuInstallationStateAsync(latestVersionAvailableFromBranch);

            // prepare YuzuManager
            YuzuManager yuzuManager = branch switch
            {
                YuzuBranch.EarlyAccess => new EarlyAccessYuzuManager(clientFactory!, latestVersionTagNameAndDownloadUrl.Key, latestVersionTagNameAndDownloadUrl.Value),
                _ => new MainlineYuzuManager(clientFactory!, latestVersionTagNameAndDownloadUrl.Key, latestVersionTagNameAndDownloadUrl.Value)
            };

            yuzuManager.YuzuDirectoryPath = yuzuLocationPath;
            yuzuManager.TempUpdateDirectoryPath = Path.Combine(yuzuLocationPath, "TempUpdate");
            yuzuManager.UpdateVisualCppRedistributables = AppIsRunningAsAdministrator();
            await yuzuManager.DownloadPrerequisitesAsync();

            // pull Yuzu depending on branch and destination paths 
            switch (yuzuManager)
            {
                case EarlyAccessYuzuManager eaym when installationState == YuzuInstallationState.NoInstallDetected:
                    await eaym.ProcessYuzuNewInstallationAsync();
                    break;

                case EarlyAccessYuzuManager eaym when installationState == YuzuInstallationState.UpdateAvailable:
                    await eaym.ProcessYuzuUpdateAsync();
                    break;

                case MainlineYuzuManager mym when installationState == YuzuInstallationState.NoInstallDetected:
                    await mym.ProcessYuzuNewInstallationAsync();
                    break;

                case MainlineYuzuManager mym when installationState == YuzuInstallationState.UpdateAvailable:
                    await mym.ProcessYuzuUpdateAsync();
                    break;
            }

            // launch Yuzu if option passed in
            if (launchYuzu)
            {
                Process.Start(new ProcessStartInfo(Path.Combine(yuzuLocationPath, "yuzu.exe"))
                {
                    UseShellExecute = true
                })?.Dispose();
            }
        }

        private static bool AppIsRunningAsAdministrator()
        {
            try
            {
                using WindowsIdentity identity = WindowsIdentity.GetCurrent();
                return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}
