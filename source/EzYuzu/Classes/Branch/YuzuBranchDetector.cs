using System.Diagnostics;
using System.IO;

namespace EzYuzu
{
    public sealed class YuzuBranchDetector
    {
		/// <summary>
		/// Gets the current branch from yuzu-cmd
		/// </summary>
		/// <returns>Branch</returns>
		public static YuzuBranchEnum GetCurrentlyInstalledBranch(string yuzuDirectoryPath)
        {
            string yuzuCmdPath = Path.Combine(yuzuDirectoryPath, "yuzu-cmd.exe");

            // if yuzu-cmd not detected, return no branch 
            if (!File.Exists(yuzuCmdPath))
                return YuzuBranchEnum.None;

            // if yuzu-cmd exists, run it with --version switch and get branch 
            var currBranch = YuzuBranchEnum.None;
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
                            currBranch = YuzuBranchEnum.EarlyAccess;

                        if (e.Data.Contains("mainline"))
                            currBranch = YuzuBranchEnum.Mainline;
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
        public static YuzuBranchEnum GetCurrentlyInstalledBranch(int overriddenUpdateChannelIndex)
        {
            return overriddenUpdateChannelIndex switch
            {
                (int)YuzuBranchEnum.Mainline => YuzuBranchEnum.Mainline,
                (int)YuzuBranchEnum.EarlyAccess => YuzuBranchEnum.EarlyAccess,
                _ => YuzuBranchEnum.Mainline,   // fallback
            };
        }

        public static int GetCurrentlyInstalledBranch(YuzuBranch overriddenUpdateChannelBranch) {
			return overriddenUpdateChannelBranch.branch switch {
				YuzuBranchEnum.Mainline => 0,
				YuzuBranchEnum.EarlyAccess => 1,
				_ => 0,   // fallback
			};
		}
	}
}
