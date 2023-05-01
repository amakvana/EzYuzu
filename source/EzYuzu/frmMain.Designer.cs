namespace EzYuzu
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
			this.lblYuzuLocation = new System.Windows.Forms.Label();
			this.txtYuzuLocation = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.pbarCurrentProgress = new System.Windows.Forms.ProgressBar();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateYuzuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reinstallVisualCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.channelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.overrideUpdateChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.yuzuWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.btnProcess = new System.Windows.Forms.Button();
			this.lblProgress = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.lblUpdateChannel = new System.Windows.Forms.Label();
			this.cboUpdateChannel = new System.Windows.Forms.ComboBox();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblYuzuLocation
			// 
			this.lblYuzuLocation.AutoSize = true;
			this.lblYuzuLocation.Location = new System.Drawing.Point(12, 32);
			this.lblYuzuLocation.Name = "lblYuzuLocation";
			this.lblYuzuLocation.Size = new System.Drawing.Size(180, 13);
			this.lblYuzuLocation.TabIndex = 0;
			this.lblYuzuLocation.Text = "Browse to Yuzu.exe Folder Location:";
			this.lblYuzuLocation.Click += new System.EventHandler(this.LblYuzuLocation_Click);
			// 
			// txtYuzuLocation
			// 
			this.txtYuzuLocation.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.txtYuzuLocation.Location = new System.Drawing.Point(12, 48);
			this.txtYuzuLocation.Name = "txtYuzuLocation";
			this.txtYuzuLocation.ReadOnly = true;
			this.txtYuzuLocation.Size = new System.Drawing.Size(312, 20);
			this.txtYuzuLocation.TabIndex = 1;
			this.txtYuzuLocation.Click += new System.EventHandler(this.TxtYuzuLocation_Click);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(330, 46);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(24, 23);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_ClickAsync);
			// 
			// pbarCurrentProgress
			// 
			this.pbarCurrentProgress.Location = new System.Drawing.Point(12, 149);
			this.pbarCurrentProgress.Name = "pbarCurrentProgress";
			this.pbarCurrentProgress.Size = new System.Drawing.Size(342, 23);
			this.pbarCurrentProgress.TabIndex = 3;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(366, 24);
			this.menuStrip1.TabIndex = 4;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripFile;
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
			this.exitToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripExit;
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
			// 
			// optionsToolStripMenuItem
			// 
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.optionsToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripOptions;
			// 
			// generalToolStripMenuItem
			// 
			this.generalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateYuzuToolStripMenuItem,
            this.channelToolStripMenuItem});
			this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
			this.generalToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
			this.generalToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripGeneral;
			// 
			// updateYuzuToolStripMenuItem
			// 
			this.updateYuzuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reinstallVisualCToolStripMenuItem});
			this.updateYuzuToolStripMenuItem.Name = "updateYuzuToolStripMenuItem";
			this.updateYuzuToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.updateYuzuToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripUpdateYuzu;
			// 
			// reinstallVisualCToolStripMenuItem
			// 
			this.reinstallVisualCToolStripMenuItem.CheckOnClick = true;
			this.reinstallVisualCToolStripMenuItem.Name = "reinstallVisualCToolStripMenuItem";
			this.reinstallVisualCToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
			this.reinstallVisualCToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripReinstallVisualC;
			// 
			// channelToolStripMenuItem
			// 
			this.channelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.overrideUpdateChannelToolStripMenuItem});
			this.channelToolStripMenuItem.Name = "channelToolStripMenuItem";
			this.channelToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			this.channelToolStripMenuItem.Text = "Update Channel";
			// 
			// overrideUpdateChannelToolStripMenuItem
			// 
			this.overrideUpdateChannelToolStripMenuItem.CheckOnClick = true;
			this.overrideUpdateChannelToolStripMenuItem.Name = "overrideUpdateChannelToolStripMenuItem";
			this.overrideUpdateChannelToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
			this.overrideUpdateChannelToolStripMenuItem.Text = "Override Update Channel";
			this.overrideUpdateChannelToolStripMenuItem.Click += new System.EventHandler(this.OverrideUpdateChannelToolStripMenuItem_ClickAsync);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yuzuWebsiteToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripSeparator1,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripHelp;
			// 
			// yuzuWebsiteToolStripMenuItem
			// 
			this.yuzuWebsiteToolStripMenuItem.Name = "yuzuWebsiteToolStripMenuItem";
			this.yuzuWebsiteToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.yuzuWebsiteToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripYuzuWebsite;
			this.yuzuWebsiteToolStripMenuItem.Click += new System.EventHandler(this.YuzuWebsiteToolStripMenuItem_Click);
			// 
			// checkForUpdatesToolStripMenuItem
			// 
			this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
			this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.checkForUpdatesToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripCheckForUpdates;
			this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.CheckForUpdatesToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.aboutToolStripMenuItem.Text = global::EzYuzu.Properties.strings.ToolStripAbout;
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
			// 
			// btnProcess
			// 
			this.btnProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnProcess.Location = new System.Drawing.Point(12, 178);
			this.btnProcess.Name = "btnProcess";
			this.btnProcess.Size = new System.Drawing.Size(342, 63);
			this.btnProcess.TabIndex = 7;
			this.btnProcess.Text = global::EzYuzu.Properties.strings.ButtonProcessSelectYuzuLocation;
			this.btnProcess.UseVisualStyleBackColor = true;
			this.btnProcess.Click += new System.EventHandler(this.BtnProcess_ClickAsync);
			// 
			// lblProgress
			// 
			this.lblProgress.AutoSize = true;
			this.lblProgress.Location = new System.Drawing.Point(12, 133);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(35, 13);
			this.lblProgress.TabIndex = 9;
			this.lblProgress.Text = "label2";
			// 
			// lblUpdateChannel
			// 
			this.lblUpdateChannel.AutoSize = true;
			this.lblUpdateChannel.Location = new System.Drawing.Point(12, 81);
			this.lblUpdateChannel.Name = "lblUpdateChannel";
			this.lblUpdateChannel.Size = new System.Drawing.Size(87, 13);
			this.lblUpdateChannel.TabIndex = 10;
			this.lblUpdateChannel.Text = "Update Channel:";
			// 
			// cboUpdateChannel
			// 
			this.cboUpdateChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboUpdateChannel.Enabled = false;
			this.cboUpdateChannel.FormattingEnabled = true;
			this.cboUpdateChannel.Location = new System.Drawing.Point(12, 97);
			this.cboUpdateChannel.Name = "cboUpdateChannel";
			this.cboUpdateChannel.Size = new System.Drawing.Size(342, 21);
			this.cboUpdateChannel.TabIndex = 11;
			this.cboUpdateChannel.SelectedIndexChanged += new System.EventHandler(this.CboUpdateChannel_SelectedIndexChanged);
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(366, 253);
			this.Controls.Add(this.cboUpdateChannel);
			this.Controls.Add(this.lblUpdateChannel);
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.btnProcess);
			this.Controls.Add(this.pbarCurrentProgress);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtYuzuLocation);
			this.Controls.Add(this.lblYuzuLocation);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "FrmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "EzYuzu - Yuzu Portable Updater";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
			this.Load += new System.EventHandler(this.FrmMain_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateYuzuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reinstallVisualCToolStripMenuItem;
        private System.Windows.Forms.Label lblUpdateChannel;
        private System.Windows.Forms.ComboBox cboUpdateChannel;
        private System.Windows.Forms.ToolStripMenuItem channelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem overrideUpdateChannelToolStripMenuItem;
    }
}

