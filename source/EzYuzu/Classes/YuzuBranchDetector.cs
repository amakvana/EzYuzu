using System.Diagnostics;
using System.IO;

namespace EzYuzu
{
    public sealed class YuzuBranchDetector
    {
        private readonly string yuzuDirectoryPath;

        public YuzuBranchDetector(string yuzuDirectoryPath)
        {
            this.yuzuDirectoryPath = yuzuDirectoryPath;
        }

        public enum Branch
        {
            Mainline,
            EarlyAccess,
            None
        }

        /// <summary>
        /// Gets the current branch from yuzu-cmd
        /// </summary>
        /// <returns>Branch</returns>
        public Branch GetCurrentlyInstalledBranch()
        {
            string yuzuCmdPath = Path.Combine(yuzuDirectoryPath, "yuzu-cmd.exe");

            // if yuzu-cmd not detected, return no branch 
            if (!File.Exists(yuzuCmdPath))
                return Branch.None;

            // if yuzu-cmd exists, run it with --version switch and get branch 
            var currBranch = Branch.None;
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
                    if (e.Data != null)
                    {
                        if (e.Data.Contains("early-access"))
                            currBranch = Branch.EarlyAccess;

                        if (e.Data.Contains("mainline"))
                            currBranch = Branch.Mainline;
                    }
                };

                p.StartInfo = psi;
                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }
            return currBranch;
        }

        /// <summary>
        /// Gets the current branch from the manually selected Update Channel
        /// </summary>
        /// <param name="overriddenUpdateChannelIndex">Update Channel Dropdown Index</param>
        /// <returns>Branch</returns>
        public Branch GetCurrentlyInstalledBranch(int overriddenUpdateChannelIndex)
        {
            return overriddenUpdateChannelIndex switch
            {
                (int)Branch.Mainline => Branch.Mainline,
                (int)Branch.EarlyAccess => Branch.EarlyAccess,
                _ => Branch.Mainline,   // fallback
            };
        }
    }
}
