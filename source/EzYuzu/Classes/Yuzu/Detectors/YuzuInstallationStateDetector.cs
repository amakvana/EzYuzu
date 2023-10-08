using static EzYuzu.Classes.Yuzu.Detectors.YuzuBranchDetector;

namespace EzYuzu.Classes.Yuzu.Detectors
{
    public sealed class YuzuInstallationStateDetector
    {
        private readonly YuzuBranch yuzuBranch;
        private readonly string yuzuDirectoryPath;
        private readonly string versionFilePath;

        public YuzuInstallationStateDetector(string yuzuDirectoryPath, YuzuBranch yuzuBranch)
        {
            this.yuzuBranch = yuzuBranch;
            this.yuzuDirectoryPath = yuzuDirectoryPath;
            this.versionFilePath = Path.Combine(yuzuDirectoryPath, "version");
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
            if (yuzuBranch == YuzuBranch.None || !YuzuExists())
                return YuzuInstallationState.NoInstallDetected;

            // yuzu.exe exists 

            // if no version file, assume out of date  
            if (!File.Exists(versionFilePath))
                return YuzuInstallationState.UpdateAvailable;

            // yuzu.exe and version file exists 

            // check current installed version against latest version 
            return await GetCurrentYuzuInstalledVersion() == latestVersionAvailable
                ? YuzuInstallationState.LatestVersionInstalled
                : YuzuInstallationState.UpdateAvailable;
        }

        public async Task<int> GetCurrentYuzuInstalledVersion()
        {
            // read version file 
            // parse it and return version number 
            // malformed version numbers will return int.MinValue
            if (!File.Exists(versionFilePath))
                return int.MinValue;

            // version file exists 
            string? line;
            using var reader = new StreamReader(versionFilePath);
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                return int.TryParse(line.Trim(), out int tmp) ? tmp : int.MinValue;
            }

            //fallback 
            return int.MinValue;
        }

        private bool YuzuExists()
        {
            return File.Exists(Path.Combine(yuzuDirectoryPath, "yuzu.exe")) || File.Exists(Path.Combine(yuzuDirectoryPath, "cemu.exe"));
        }
    }
}
