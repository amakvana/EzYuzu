using CommandLine;
using EzYuzu.Classes.CLOptions;
using EzYuzu.Classes.Updaters;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using static EzYuzu.Classes.Yuzu.Detectors.YuzuBranchDetector;

namespace EzYuzu
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();

            // no args passed, load gui
            if (args is null || args.Length <= 0)
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new FrmMain(serviceProvider));
                return;
            }

            // otherwise, process commandline args 
            await ProcessCommandLineArgsAsync(serviceProvider, args);
        }

        private static IServiceProvider BuildServiceProvider()
        {
            // prepare services for Depedency injection 
            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient();
            services.AddHttpClient("GitHub-Api", client =>
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.Add("accept", "application/vnd.github+json");
                client.DefaultRequestHeaders.Add("user-agent", "request");
            });

            services.AddHttpClient("GitHub-EzYuzu", client =>
            {
                client.BaseAddress = new Uri("https://raw.githubusercontent.com/amakvana/EzYuzu/master/");
                client.DefaultRequestHeaders.Add("accept", "application/vnd.github.raw");
                client.DefaultRequestHeaders.Add("user-agent", "request");
            });
            return services.BuildServiceProvider();
        }

        private static async Task ProcessCommandLineArgsAsync(IServiceProvider serviceProvider, string[] args)
        {
            // if app isn't latest version, display error and exit out
            var updater = new AppUpdater(serviceProvider);
            var currentAppVersion = await updater.CheckVersionAsync();
            switch (currentAppVersion)
            {
                case AppUpdater.CurrentVersion.UpdateAvailable:
                    MessageBox.Show("New version of EzYuzu available, please download from https://github.com/amakvana/EzYuzu", "EzYuzu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(new ProcessStartInfo("https://github.com/amakvana/EzYuzu")
                    {
                        UseShellExecute = true,
                        Verb = "Open"
                    })?.Dispose();
                    return;
                case AppUpdater.CurrentVersion.NotSupported:
                    MessageBox.Show("This version of EzYuzu is no longer supported, please download the latest version from https://github.com/amakvana/EzYuzu", "EzYuzu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Process.Start(new ProcessStartInfo("https://github.com/amakvana/EzYuzu")
                    {
                        UseShellExecute = true,
                        Verb = "Open"
                    })?.Dispose();
                    return;
                case AppUpdater.CurrentVersion.Undetectable:
                    MessageBox.Show("Network Connection Error! Please check your internet connection and try again.", "EzYuzu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            // current version of EzYuzu is latest, process command line args 

            // prepare Parser and redirect HelpWriter into SW 
            using var helpWriter = new StringWriter();
            var parser = new Parser(config => config.HelpWriter = helpWriter);

            // parse arguments
            var parserResults = parser.ParseArguments<CommandLineInstallOptions>(args);
            await parserResults.WithParsedAsync(async options =>
            {
                var branch = YuzuBranch.None;   // default if no branch params have been passed
                if (options.EarlyAccess)
                {
                    branch = YuzuBranch.EarlyAccess;
                }
                else if (options.Mainline)
                {
                    branch = YuzuBranch.Mainline;
                }

                var updater = new YuzuCommandLineUpdater(serviceProvider);
                await updater.ProcessYuzuDirectory(options.YuzuLocationPath, branch, options.UpdateVersion, options.LaunchYuzuAfterUpdate, options.EnableHDR);
            });
            parserResults.WithNotParsed(options =>
            {
                if (options.IsVersion() || options.IsHelp())
                {
                    MessageBox.Show(helpWriter.ToString(), "EzYuzu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }
    }
}