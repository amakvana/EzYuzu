using CommandLine;

namespace EzYuzu.Classes.CLOptions
{
    public sealed class CommandLineInstallOptions
    {

        [Option('p', "path", Default = "", Required = true, HelpText = "Set Yuzu location directory path for new install/update.")]
        public string? YuzuLocationPath { get; set; }

        [Option('m', "mainline", Default = false, SetName = "mainline", HelpText = "Force Yuzu install/update channel to mainline.")]
        public bool Mainline { get; set; }

        [Option('e', "early-access", Default = false, SetName = "earlyaccess", HelpText = "Force Yuzu install/update channel to early access.")]
        public bool EarlyAccess { get; set; }

        [Option('v', Default = 0, HelpText = "Set a specific version number to update/rollback to.")]
        public int UpdateVersion { get; set; }

        [Option('l', "launch-yuzu", Default = false, HelpText = "Launch Yuzu after new install/update.")]
        public bool LaunchYuzuAfterUpdate { get; set; }

        [Option("enable-hdr", Default = false, HelpText = "Enables HDR by renaming yuzu.exe to cemu.exe.")]
        public bool EnableHDR { get; set; }

    }
}
