using System.IO;
using System.Threading.Tasks;

namespace EzYuzu
{
    public sealed class YuzuInstallationStateDetector
    {
        public async static Task<YuzuInstallationState> GetYuzuInstallationStateAsync(string yuzuDirectoryPath, YuzuBranchEnum yuzuBranch)
        {
            // if yuzu.exe doesn't exist in the current directory, it's not setup 
            if (yuzuBranch == YuzuBranchEnum.None || !File.Exists(Path.Combine(yuzuDirectoryPath, "yuzu.exe")))
                return YuzuInstallationState.NoInstallDetected;

            // yuzu.exe exists 

            // if no version file, assume out of date  
            if (!File.Exists(Path.Combine(yuzuDirectoryPath, "version")))
                return YuzuInstallationState.UpdateAvailable;

            // yuzu.exe and version file exists 

            // check current installed version against latest online version 
            string repo = "";
            switch (yuzuBranch)
            {
                case YuzuBranchEnum.Mainline:
                    repo = "https://github.com/yuzu-emu/yuzu-mainline/releases/latest";
                    break;
                case YuzuBranchEnum.EarlyAccess:
                    repo = "https://github.com/pineappleEA/pineapple-src/releases/latest";
                    break;
                case YuzuBranchEnum.None:
                default:
                    break;
            }

            using (var f = File.OpenRead(Path.Combine(yuzuDirectoryPath, "version")))
            using (var reader = new StreamReader(f))
            {
                string line;
                int latestVersion = YuzuUtilities.FetchLatestYuzuVersionNumber(repo);
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    int currentVersion = int.Parse(line.Trim());
                    return (currentVersion == latestVersion)
                        ? YuzuInstallationState.LatestVersionInstalled
                        : YuzuInstallationState.UpdateAvailable;
                }
            }

            // fallback 
            return YuzuInstallationState.NoInstallDetected;
        }

    }
}
