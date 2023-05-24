namespace EzYuzu
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            lblYuzuLocation = new Label();
            txtYuzuLocation = new TextBox();
            btnBrowse = new Button();
            pbarCurrentProgress = new ProgressBar();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            generalToolStripMenuItem = new ToolStripMenuItem();
            updateYuzuToolStripMenuItem = new ToolStripMenuItem();
            launchYuzuAfterUpdateToolStripMenuItem = new ToolStripMenuItem();
            reinstallVisualCToolStripMenuItem = new ToolStripMenuItem();
            exitAfterUpdateToolStripMenuItem = new ToolStripMenuItem();
            autoupdateOnEzYuzuStartToolStripMenuItem = new ToolStripMenuItem();
            advancedToolStripMenuItem = new ToolStripMenuItem();
            overrideUpdateChannelToolStripMenuItem = new ToolStripMenuItem();
            overrideUpdateVersionToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            yuzuWebsiteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            btnProcess = new Button();
            lblProgress = new Label();
            toolTip1 = new ToolTip(components);
            lblUpdateChannel = new Label();
            cboUpdateChannel = new ComboBox();
            lblUpdateVersion = new Label();
            cboUpdateVersion = new ComboBox();
            grpOptions = new GroupBox();
            menuStrip1.SuspendLayout();
            grpOptions.SuspendLayout();
            SuspendLayout();
            // 
            // lblYuzuLocation
            // 
            lblYuzuLocation.AutoSize = true;
            lblYuzuLocation.Location = new Point(13, 37);
            lblYuzuLocation.Margin = new Padding(4, 0, 4, 0);
            lblYuzuLocation.Name = "lblYuzuLocation";
            lblYuzuLocation.Size = new Size(196, 15);
            lblYuzuLocation.TabIndex = 0;
            lblYuzuLocation.Text = "Browse to Yuzu.exe Folder Location:";
            toolTip1.SetToolTip(lblYuzuLocation, "Click and browse to your Yuzu.exe Folder Location");
            lblYuzuLocation.Click += LblYuzuLocation_Click;
            // 
            // txtYuzuLocation
            // 
            txtYuzuLocation.Location = new Point(14, 55);
            txtYuzuLocation.Margin = new Padding(4, 3, 4, 3);
            txtYuzuLocation.Name = "txtYuzuLocation";
            txtYuzuLocation.ReadOnly = true;
            txtYuzuLocation.Size = new Size(363, 23);
            txtYuzuLocation.TabIndex = 1;
            toolTip1.SetToolTip(txtYuzuLocation, "Click and browse to your Yuzu.exe Folder Location");
            txtYuzuLocation.Click += TxtYuzuLocation_Click;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(385, 53);
            btnBrowse.Margin = new Padding(4, 3, 4, 3);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(28, 27);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "...";
            toolTip1.SetToolTip(btnBrowse, "Click and browse to your Yuzu.exe Folder Location");
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += BtnBrowse_ClickAsync;
            // 
            // pbarCurrentProgress
            // 
            pbarCurrentProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbarCurrentProgress.Location = new Point(14, 231);
            pbarCurrentProgress.Margin = new Padding(4, 3, 4, 3);
            pbarCurrentProgress.Name = "pbarCurrentProgress";
            pbarCurrentProgress.Size = new Size(399, 27);
            pbarCurrentProgress.TabIndex = 3;
            toolTip1.SetToolTip(pbarCurrentProgress, "Progress completed of current action");
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 2, 0, 2);
            menuStrip1.Size = new Size(427, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(92, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { generalToolStripMenuItem, advancedToolStripMenuItem });
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(61, 20);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // generalToolStripMenuItem
            // 
            generalToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { updateYuzuToolStripMenuItem });
            generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            generalToolStripMenuItem.Size = new Size(180, 22);
            generalToolStripMenuItem.Text = "General";
            // 
            // updateYuzuToolStripMenuItem
            // 
            updateYuzuToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { launchYuzuAfterUpdateToolStripMenuItem, reinstallVisualCToolStripMenuItem, exitAfterUpdateToolStripMenuItem, autoupdateOnEzYuzuStartToolStripMenuItem });
            updateYuzuToolStripMenuItem.Name = "updateYuzuToolStripMenuItem";
            updateYuzuToolStripMenuItem.Size = new Size(180, 22);
            updateYuzuToolStripMenuItem.Text = "Update Yuzu";
            // 
            // launchYuzuAfterUpdateToolStripMenuItem
            // 
            launchYuzuAfterUpdateToolStripMenuItem.CheckOnClick = true;
            launchYuzuAfterUpdateToolStripMenuItem.Name = "launchYuzuAfterUpdateToolStripMenuItem";
            launchYuzuAfterUpdateToolStripMenuItem.Size = new Size(225, 22);
            launchYuzuAfterUpdateToolStripMenuItem.Text = "Launch Yuzu after update";
            launchYuzuAfterUpdateToolStripMenuItem.ToolTipText = "Launch Yuzu after Update or New Install is complete";
            // 
            // reinstallVisualCToolStripMenuItem
            // 
            reinstallVisualCToolStripMenuItem.CheckOnClick = true;
            reinstallVisualCToolStripMenuItem.Enabled = false;
            reinstallVisualCToolStripMenuItem.Name = "reinstallVisualCToolStripMenuItem";
            reinstallVisualCToolStripMenuItem.Size = new Size(225, 22);
            reinstallVisualCToolStripMenuItem.Text = "Reinstall Visual C++";
            reinstallVisualCToolStripMenuItem.ToolTipText = "Automatically reinstall Visual C++ Redistrbutables when EzYuzu is run as Administrator";
            // 
            // exitAfterUpdateToolStripMenuItem
            // 
            exitAfterUpdateToolStripMenuItem.CheckOnClick = true;
            exitAfterUpdateToolStripMenuItem.Name = "exitAfterUpdateToolStripMenuItem";
            exitAfterUpdateToolStripMenuItem.Size = new Size(225, 22);
            exitAfterUpdateToolStripMenuItem.Text = "Exit after update";
            exitAfterUpdateToolStripMenuItem.ToolTipText = "Exit EzYuzu after Yuzu has been updated";
            // 
            // autoupdateOnEzYuzuStartToolStripMenuItem
            // 
            autoupdateOnEzYuzuStartToolStripMenuItem.CheckOnClick = true;
            autoupdateOnEzYuzuStartToolStripMenuItem.Name = "autoupdateOnEzYuzuStartToolStripMenuItem";
            autoupdateOnEzYuzuStartToolStripMenuItem.Size = new Size(225, 22);
            autoupdateOnEzYuzuStartToolStripMenuItem.Text = "Auto-update on EzYuzu start";
            autoupdateOnEzYuzuStartToolStripMenuItem.ToolTipText = "Automatically update Yuzu when EzYuzu is launched";
            // 
            // advancedToolStripMenuItem
            // 
            advancedToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { overrideUpdateChannelToolStripMenuItem, overrideUpdateVersionToolStripMenuItem });
            advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            advancedToolStripMenuItem.Size = new Size(180, 22);
            advancedToolStripMenuItem.Text = "Advanced";
            // 
            // overrideUpdateChannelToolStripMenuItem
            // 
            overrideUpdateChannelToolStripMenuItem.CheckOnClick = true;
            overrideUpdateChannelToolStripMenuItem.Name = "overrideUpdateChannelToolStripMenuItem";
            overrideUpdateChannelToolStripMenuItem.Size = new Size(207, 22);
            overrideUpdateChannelToolStripMenuItem.Text = "Override Update Channel";
            overrideUpdateChannelToolStripMenuItem.ToolTipText = "Toggle to allow changing between Mainline and Early Access update channels";
            overrideUpdateChannelToolStripMenuItem.Click += OverrideUpdateChannelToolStripMenuItem_ClickAsync;
            // 
            // overrideUpdateVersionToolStripMenuItem
            // 
            overrideUpdateVersionToolStripMenuItem.CheckOnClick = true;
            overrideUpdateVersionToolStripMenuItem.Name = "overrideUpdateVersionToolStripMenuItem";
            overrideUpdateVersionToolStripMenuItem.Size = new Size(207, 22);
            overrideUpdateVersionToolStripMenuItem.Text = "Override Update Version";
            overrideUpdateVersionToolStripMenuItem.ToolTipText = "Toggle to select specific versions of a build. Useful when rolling back to previous versions";
            overrideUpdateVersionToolStripMenuItem.Click += OverrideUpdateVersionToolStripMenuItem_ClickAsync;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { yuzuWebsiteToolStripMenuItem, toolStripSeparator1, aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // yuzuWebsiteToolStripMenuItem
            // 
            yuzuWebsiteToolStripMenuItem.Name = "yuzuWebsiteToolStripMenuItem";
            yuzuWebsiteToolStripMenuItem.Size = new Size(145, 22);
            yuzuWebsiteToolStripMenuItem.Text = "Yuzu Website";
            yuzuWebsiteToolStripMenuItem.Click += YuzuWebsiteToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(142, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(145, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // btnProcess
            // 
            btnProcess.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnProcess.Enabled = false;
            btnProcess.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnProcess.Location = new Point(14, 264);
            btnProcess.Margin = new Padding(4, 3, 4, 3);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(399, 73);
            btnProcess.TabIndex = 7;
            btnProcess.Text = "Select the Directory containing Yuzu.exe";
            toolTip1.SetToolTip(btnProcess, "Click to download the latest version of Yuzu");
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += BtnProcess_ClickAsync;
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblProgress.AutoSize = true;
            lblProgress.Location = new Point(13, 213);
            lblProgress.Margin = new Padding(4, 0, 4, 0);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(0, 15);
            lblProgress.TabIndex = 9;
            // 
            // lblUpdateChannel
            // 
            lblUpdateChannel.AutoSize = true;
            lblUpdateChannel.Location = new Point(7, 19);
            lblUpdateChannel.Margin = new Padding(4, 0, 4, 0);
            lblUpdateChannel.Name = "lblUpdateChannel";
            lblUpdateChannel.Size = new Size(95, 15);
            lblUpdateChannel.TabIndex = 12;
            lblUpdateChannel.Text = "Update Channel:";
            toolTip1.SetToolTip(lblUpdateChannel, "The current Update Channel of Yuzu. Auto-detected when the Yuzu Path is selected. \\n\" +");
            // 
            // cboUpdateChannel
            // 
            cboUpdateChannel.DropDownStyle = ComboBoxStyle.DropDownList;
            cboUpdateChannel.Enabled = false;
            cboUpdateChannel.FormattingEnabled = true;
            cboUpdateChannel.Items.AddRange(new object[] { "Mainline", "Early Access" });
            cboUpdateChannel.Location = new Point(7, 37);
            cboUpdateChannel.Margin = new Padding(4, 3, 4, 3);
            cboUpdateChannel.Name = "cboUpdateChannel";
            cboUpdateChannel.Size = new Size(389, 23);
            cboUpdateChannel.TabIndex = 13;
            toolTip1.SetToolTip(cboUpdateChannel, "The current Update Channel of Yuzu. Auto-detected when the Yuzu Path is selected. \\n\" +");
            cboUpdateChannel.SelectedIndexChanged += CboUpdateChannel_SelectedIndexChangedAsync;
            // 
            // lblUpdateVersion
            // 
            lblUpdateVersion.AutoSize = true;
            lblUpdateVersion.Location = new Point(7, 73);
            lblUpdateVersion.Name = "lblUpdateVersion";
            lblUpdateVersion.Size = new Size(89, 15);
            lblUpdateVersion.TabIndex = 14;
            lblUpdateVersion.Text = "Update Version:";
            // 
            // cboUpdateVersion
            // 
            cboUpdateVersion.DropDownHeight = 75;
            cboUpdateVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            cboUpdateVersion.Enabled = false;
            cboUpdateVersion.FormattingEnabled = true;
            cboUpdateVersion.IntegralHeight = false;
            cboUpdateVersion.Location = new Point(6, 91);
            cboUpdateVersion.MaxDropDownItems = 10;
            cboUpdateVersion.Name = "cboUpdateVersion";
            cboUpdateVersion.Size = new Size(389, 23);
            cboUpdateVersion.TabIndex = 15;
            cboUpdateVersion.SelectedIndexChanged += CboUpdateVersion_SelectedIndexChangedAsync;
            // 
            // grpOptions
            // 
            grpOptions.Controls.Add(cboUpdateVersion);
            grpOptions.Controls.Add(lblUpdateVersion);
            grpOptions.Controls.Add(cboUpdateChannel);
            grpOptions.Controls.Add(lblUpdateChannel);
            grpOptions.Location = new Point(12, 86);
            grpOptions.Name = "grpOptions";
            grpOptions.Size = new Size(403, 120);
            grpOptions.TabIndex = 12;
            grpOptions.TabStop = false;
            grpOptions.Text = "Options";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(427, 351);
            Controls.Add(grpOptions);
            Controls.Add(lblProgress);
            Controls.Add(btnProcess);
            Controls.Add(pbarCurrentProgress);
            Controls.Add(btnBrowse);
            Controls.Add(txtYuzuLocation);
            Controls.Add(lblYuzuLocation);
            Controls.Add(menuStrip1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "EzYuzu - Yuzu Portable Updater";
            FormClosed += FrmMain_FormClosed;
            Load += FrmMain_LoadAsync;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            grpOptions.ResumeLayout(false);
            grpOptions.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblYuzuLocation;
        private System.Windows.Forms.TextBox txtYuzuLocation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ProgressBar pbarCurrentProgress;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yuzuWebsiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateYuzuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reinstallVisualCToolStripMenuItem;
        private ToolStripMenuItem launchYuzuAfterUpdateToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem overrideUpdateChannelToolStripMenuItem;
        private ToolStripMenuItem overrideUpdateVersionToolStripMenuItem;
        private Label lblUpdateChannel;
        private ComboBox cboUpdateChannel;
        private Label lblUpdateVersion;
        private ComboBox cboUpdateVersion;
        private GroupBox grpOptions;
        private ToolStripMenuItem exitAfterUpdateToolStripMenuItem;
        private ToolStripMenuItem autoupdateOnEzYuzuStartToolStripMenuItem;
    }
}