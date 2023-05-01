using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzYuzu {
	[Verb("gui", HelpText = "Run EzYuzu graphically.")]
	public class GuiOptions {
		[Option('l', "location", HelpText = "Set Yuzu folder location.")]
		public string YuzuLocation { get; set; }

		[Option('m', "mainline", Default = false, SetName = "mainline", HelpText = "Set the Yuzu update channel override to mainline.")]
		public bool Mainline { get; set; }

		[Option('e', "early-access", Default = false, SetName = "earlyaccess", HelpText = "Set the Yuzu update channel override to early access.")]
		public bool EarlyAccess { get; set; }

		[Option('v', "visual-c", Default = false, HelpText = "Set reinstall Visual C++.")]
		public bool VisualC { get; set; }
	}
}
