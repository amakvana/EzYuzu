using static EzYuzu.Classes.Yuzu.Detectors.YuzuBranchDetector;

namespace EzYuzu.Classes.Yuzu.Detectors
{
    public sealed class YuzuInstallationStateDetector
    {
        private readonly YuzuBranch yuzuBranch;
        private readonly string yuzuDirectoryPath;

        public YuzuInstallationStateDetector(string yuzuDirectoryPath, YuzuBranch yuzuBranch)
        {
            this.yuzuBranch = yuzuBranch;
            this.yuzuDirectoryPath = yuzuDirectoryPath;
        }

        public enum YuzuInstallationState
        {
            LatestVersionInstalled,
            UpdateAvailable,
            NoInstallDetected
        }

        public async Task<YuzuInstallationState> GetYuzuInstallationStateAsync(int latestVersionAvailable)
        {
            // if yuzu.exe doesn't exist in the current directory, it's not setup 
            if (yuzuBranch == YuzuBranch.None || !File.Exists(Path.Combine(yuzuDirectoryPath, "yuzu.exe")))
                return YuzuInstallationState.NoInstallDetected;

            // yuzu.exe exists 

            // if no version file, assume out of date  
            if (!File.Exists(Path.Combine(yuzuDirectoryPath, "version")))
                return YuzuInstallationState.UpdateAvailable;

            // yuzu.exe and version file exists 

            // check current installed version against latest version 
            using (var reader = new StreamReader(Path.Combine(yuzuDirectoryPath, "version")))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) is not null)
                {
                    int currentVersion = int.Parse(line.Trim());
                    return currentVersion == latestVersionAvailable
                        ? YuzuInstallationState.LatestVersionInstalled
                        : YuzuInstallationState.UpdateAvailable;
                }
            }

            // fallback 
            return YuzuInstallationState.NoInstallDetected;
        }
    }
}
