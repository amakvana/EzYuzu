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

        public async Task ProcessYuzuDirectory(string? yuzuLocationPath, YuzuBranch branch, int updateVersion, bool launchYuzu, bool enableHdr)
        {
            // if no path passed, exit method 
            if (yuzuLocationPath is null)
                return;

            // if yuzuLocationPath passed in is something like D:\path\to\yuzu\yuzu.exe
            // modify the path so it strips the file from the path and return the absolute directory only 
            if (yuzuLocationPath.EndsWith("yuzu.exe", StringComparison.OrdinalIgnoreCase))
                yuzuLocationPath = Path.GetDirectoryName(yuzuLocationPath);

            // rename old cemu.exe to yuzu.exe to avoid naming conflicts
            if (!enableHdr && File.Exists(Path.Combine(yuzuLocationPath!, "cemu.exe")))
                RenameFile(yuzuLocationPath!, "cemu.exe", "yuzu.exe");

            // if no branch passed in, auto detect branch
            var branchDetector = new YuzuBranchDetector(clientFactory!)
            {
                YuzuDirectoryPath = yuzuLocationPath!
            };
            if (branch == YuzuBranch.None)
                branch = await branchDetector.GetCurrentlyInstalledBranchAsync();

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
            var stateDetector = new YuzuInstallationStateDetector(yuzuLocationPath!, branch);
            var installationState = await stateDetector.GetYuzuInstallationStateAsync(latestVersionAvailableFromBranch);

            // if we have the latest version of Yuzu installed and Launch Yuzu is selected 
            // no point parsing the info, just launch Yuzu
            // speeds up processing 
            if (installationState == YuzuInstallationState.LatestVersionInstalled && launchYuzu)
            {
                string exeToLaunch = File.Exists(Path.Combine(yuzuLocationPath!, "cemu.exe")) ? "cemu.exe" : "yuzu.exe";
                Process.Start(new ProcessStartInfo(Path.Combine(yuzuLocationPath!, exeToLaunch))
                {
                    UseShellExecute = true
                })?.Dispose();
                return;
            }

            // if Yuzu detected is not the latest version, process new install / update

            // prepare YuzuManager
            YuzuManager yuzuManager = branch switch
            {
                YuzuBranch.EarlyAccess => new EarlyAccessYuzuManager(clientFactory!, latestVersionTagNameAndDownloadUrl.Key, latestVersionTagNameAndDownloadUrl.Value),
                _ => new MainlineYuzuManager(clientFactory!, latestVersionTagNameAndDownloadUrl.Key, latestVersionTagNameAndDownloadUrl.Value)
            };

            yuzuManager.YuzuDirectoryPath = yuzuLocationPath!;
            yuzuManager.TempUpdateDirectoryPath = Path.Combine(yuzuLocationPath!, "TempUpdate");
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

            // if EnableHDR option is passed in 
            if (enableHdr)
                RenameFile(yuzuLocationPath!, "yuzu.exe", "cemu.exe");

            // launch Yuzu if option passed in
            if (launchYuzu)
            {
                string exeToLaunch = enableHdr ? "cemu.exe" : "yuzu.exe";
                Process.Start(new ProcessStartInfo(Path.Combine(yuzuLocationPath!, exeToLaunch))
                {
                    UseShellExecute = true
                })?.Dispose();
            }

            Directory.Delete(yuzuManager.TempUpdateDirectoryPath, true);
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

        private static void RenameFile(string path, string oldFileName, string newFileName)
        {
            // ensure renaming doesn't happen while file is being written to filesystem 
            bool renamed = false;
            while (!renamed)
            {
                if (!File.Exists(Path.Combine(path, oldFileName)))
                    continue;

                File.Move(Path.Combine(path, oldFileName), Path.Combine(path, newFileName), true);
                renamed = true;
            }
        }
    }
}
