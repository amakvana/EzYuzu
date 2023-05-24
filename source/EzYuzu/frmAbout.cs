using System.Reflection;

namespace EzYuzu
{
    partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            //this.textBoxDescription.Text = AssemblyDescription;
            this.textBoxDescription.Text = String.Format("EzYuzu {0}" +
                "Yuzu Portable Updater (Unofficial) by amakvana {0}{0}" +
                "https://github.com/amakvana/EzYuzu {0}{0}" +
                "This software is licensed under GNU GPL-3.0 {0}" +
                "Source code is available in repository above {0}{0}" +
                "Credits: https://github.com/amakvana/EzYuzu#acknowledgements {0}{0}" +
                "Disclaimer: {0}" +
                "This software comes with no warranty, express or implied nor does the author makes no representation of warranties. The author claims no responsibility for damages resulting from any use or misuse of the software.", Environment.NewLine);

        }

        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                //return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
                return Path.GetFileNameWithoutExtension(System.AppContext.BaseDirectory);
            }
        }

        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
