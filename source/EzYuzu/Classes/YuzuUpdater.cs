using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzYuzu {
	public class YuzuUpdater {
		public async static void Process(string yuzuLocation, YuzuBranchEnum overrideBranch, bool reinstallVisualC, bool reinstallYuzu, bool preventNewYuzuInstall) {
			if(!Directory.Exists(yuzuLocation)) {
				Console.WriteLine(Properties.strings.ProcessYuzuLocationInvalid);

				return;
			}

			var currentYuzuBranch = overrideBranch;

			if(currentYuzuBranch == YuzuBranchEnum.None) {
				currentYuzuBranch = YuzuBranchDetector.GetCurrentlyInstalledBranch(yuzuLocation);
			}

			YuzuManager yuzuManager = currentYuzuBranch switch {
				YuzuBranchEnum.EarlyAccess => new EarlyAccessYuzuManager(),
				_ => new MainlineYuzuManager(),     // default
			};

			// prepare YuzuManager and download all prerequisites
			yuzuManager.YuzuDirectoryPath = yuzuLocation;
			yuzuManager.TempUpdateDirectoryPath = Path.Combine(yuzuLocation, "TempUpdate");
			//yuzuManager.UpdateProgress += EzYuzuDownloader_UpdateCurrentProgress;
			await yuzuManager.DownloadPrerequisitesAsync();

			var installationState = await YuzuInstallationStateDetector.GetYuzuInstallationStateAsync(yuzuLocation, currentYuzuBranch);

			// Process yuzu depending on directory selected and type of YuzuManager
			if(yuzuManager is EarlyAccessYuzuManager eaym) {
				switch(installationState.installationState) {
					case YuzuInstallationStateEnum.LatestVersionInstalled:
						if(reinstallYuzu)
							goto case YuzuInstallationStateEnum.NoInstallDetected;
						else
							Console.WriteLine(Properties.strings.ButtonProcessYuzuUpToDate);
						break;
					case YuzuInstallationStateEnum.NoInstallDetected:
						if(preventNewYuzuInstall) {
							Console.WriteLine(Properties.strings.ProcessPreventNewInstall);
							break;
						}

						await eaym.ProcessYuzuNewInstallationAsync();
						break;

					case YuzuInstallationStateEnum.UpdateAvailable:
						if(reinstallVisualC) {
							await eaym.DownloadInstallVisualCppRedistAsync();
						}
						await eaym.ProcessYuzuUpdateAsync();
						break;
				}
			} else if(yuzuManager is MainlineYuzuManager mym) {
				switch(installationState.installationState) {
					case YuzuInstallationStateEnum.LatestVersionInstalled:
						if(reinstallYuzu)
							goto case YuzuInstallationStateEnum.NoInstallDetected;
						else
							Console.WriteLine(Properties.strings.ButtonProcessYuzuUpToDate);
						break;
					case YuzuInstallationStateEnum.NoInstallDetected:
						if(preventNewYuzuInstall) {
							Console.WriteLine(Properties.strings.ProcessPreventNewInstall);
							break;
						}

						await mym.ProcessYuzuNewInstallationAsync();
						break;

					case YuzuInstallationStateEnum.UpdateAvailable:
						if(reinstallVisualC) {
							await mym.DownloadInstallVisualCppRedistAsync();
						}
						await mym.ProcessYuzuUpdateAsync();
						break;
				}
			}
		}
	}
}
