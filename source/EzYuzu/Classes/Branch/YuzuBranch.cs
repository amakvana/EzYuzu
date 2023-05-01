using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzYuzu {
	public class YuzuBranch {
		public static readonly YuzuBranch Mainline = new YuzuBranch(YuzuBranchEnum.Mainline, Properties.strings.ComboBoxUpdateChannelMainline);

		public static readonly YuzuBranch EarlyAccess = new YuzuBranch(YuzuBranchEnum.EarlyAccess, Properties.strings.ComboBoxUpdateChannelEarlyAccess);

		//Not included in Branches as it's not valid and its purpose is functional
		public static readonly YuzuBranch None = new YuzuBranch(YuzuBranchEnum.None, "");

		public static readonly YuzuBranch[] Branches = new YuzuBranch[] { Mainline, EarlyAccess };

		public YuzuBranchEnum branch { get; private set; }
		public string branchString { get; private set; }

		private YuzuBranch(YuzuBranchEnum branch, string branchString) {
			this.branch = branch;
			this.branchString = branchString;
		}
	}
}
