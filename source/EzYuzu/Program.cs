using CommandLine;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows.Forms;

namespace EzYuzu
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			bool hasParseErrors = false;

			Parser.Default.ParseArguments<CheckForUpdateOptions, GuiOptions, InstallOptions>(args)
				.WithParsed<CheckForUpdateOptions>(options => {
					var currentAppVersion = AppUpdater.CheckVersion();
					switch(currentAppVersion) {
						case AppUpdater.CurrentVersion.LatestVersion:
							Console.WriteLine(Properties.strings.UpdateLatestVersion);
							break;
						case AppUpdater.CurrentVersion.UpdateAvailable:
							Console.WriteLine(Properties.strings.UpdateAvailable);
							break;
						case AppUpdater.CurrentVersion.NotSupported:
							Console.WriteLine(Properties.strings.UpdateNotSupported);
							break;
					}
				})
				.WithParsed<GuiOptions>(options => {
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new FrmMain(options));
				})
				.WithParsed<InstallOptions>(options => {
					YuzuBranchEnum overrideBranch = YuzuBranchEnum.None;

					if(options.Mainline) {
						overrideBranch = YuzuBranchEnum.Mainline;
					} else if(options.EarlyAccess) {
						overrideBranch = YuzuBranchEnum.EarlyAccess;
					}

					YuzuUpdater.Process(options.YuzuLocation, overrideBranch, options.VisualC, options.Reinstall, options.PreventNew);
				})
				.WithNotParsed(errors => { hasParseErrors = true; });

			if(hasParseErrors)
				return;
        }
    }
}
