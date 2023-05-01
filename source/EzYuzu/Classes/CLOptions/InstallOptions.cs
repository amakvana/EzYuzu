using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzYuzu {
	[Verb("install", HelpText = "Install or update Yuzu.")]
	public class InstallOptions {
		[Option('l', "location", Required = true, HelpText = "Yuzu install/update folder location.")]
		public string YuzuLocation { get; set; }

		[Option('m', "mainline", Default = false, SetName = "mainline", HelpText = "Force Yuzu install/update channel to mainline.")]
		public bool Mainline { get; set; }

		[Option('e', "early-access", Default = false, SetName = "earlyaccess", HelpText = "Force Yuzu install/update channel to early access.")]
		public bool EarlyAccess { get; set; }

		[Option('v', "visual-c", Default = false, HelpText = "Reinstall Visual C++.")]
		public bool VisualC { get; set; }

		[Option('r', "reinstall", Default = false, HelpText = "Reinstall Yuzu even if it's already up-to-date")]
		public bool Reinstall { get; set; }

		[Option('p', "prevent-new", Default = false, HelpText = "Prevent installing Yuzu where it isn't already installed")]
		public bool PreventNew { get; set; }
	}
}
