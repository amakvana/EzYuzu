using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzYuzu {
	public class YuzuInstallationState {
		public static readonly YuzuInstallationState LatestVersionInstalled = new YuzuInstallationState(YuzuInstallationStateEnum.LatestVersionInstalled, Properties.strings.ButtonProcessYuzuUpToDate, Properties.strings.ToolTipButtonProcessYuzuUpToDate);

		public static readonly YuzuInstallationState UpdateAvailable = new YuzuInstallationState(YuzuInstallationStateEnum.UpdateAvailable, Properties.strings.ButtonProcessUpdateYuzu, Properties.strings.ToolTipButtonProcessUpdateYuzu);

		public static readonly YuzuInstallationState NoInstallDetected = new YuzuInstallationState(YuzuInstallationStateEnum.NoInstallDetected, Properties.strings.ButtonProcessNewInstall, Properties.strings.ToolTipButtonProcessNewInstall);

		public YuzuInstallationStateEnum installationState { get; private set; }
		public string installationStateShortString { get; private set; }
		public string installationStateLongString { get; private set; }

		private YuzuInstallationState(YuzuInstallationStateEnum installationState, string installationStateShortString, string installationStateLongString) {
			this.installationState = installationState;
			this.installationStateShortString = installationStateShortString;
			this.installationStateLongString = installationStateLongString;
		}
	}
}
