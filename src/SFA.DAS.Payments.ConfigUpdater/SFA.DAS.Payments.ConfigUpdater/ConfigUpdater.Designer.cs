namespace SFA.DAS.Payments.ConfigUpdater
{
    partial class ConfigUpdater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigUpdater));
            this.SourceFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.SourceFolderPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SetSourceFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ConfigToUpdate = new System.Windows.Forms.TextBox();
            this.DetermineConfig = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.MissingParameters = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.AllParameterValues = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ConsolidatedParameters = new System.Windows.Forms.DataGridView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.MismatchedParameters = new System.Windows.Forms.DataGridView();
            this.PublishLogPage = new System.Windows.Forms.TabPage();
            this.PublishOutputText = new System.Windows.Forms.TextBox();
            this.UpdateConfig = new System.Windows.Forms.Button();
            this.InstallServices = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.VSCommandLinePath = new System.Windows.Forms.TextBox();
            this.PublishProgress = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.PublishProjectComboBox = new System.Windows.Forms.ComboBox();
            this.UninstallServices = new System.Windows.Forms.Button();
            this.StartLocalAzureEmulator = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MissingParameters)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AllParameterValues)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConsolidatedParameters)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MismatchedParameters)).BeginInit();
            this.PublishLogPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // SourceFolderBrowser
            // 
            this.SourceFolderBrowser.Description = "Select Source Folder";
            this.SourceFolderBrowser.ShowNewFolderButton = false;
            // 
            // SourceFolderPath
            // 
            this.SourceFolderPath.Location = new System.Drawing.Point(142, 12);
            this.SourceFolderPath.Name = "SourceFolderPath";
            this.SourceFolderPath.ReadOnly = true;
            this.SourceFolderPath.Size = new System.Drawing.Size(345, 20);
            this.SourceFolderPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Payments Source Folder";
            // 
            // SetSourceFolder
            // 
            this.SetSourceFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetSourceFolder.Location = new System.Drawing.Point(487, 11);
            this.SetSourceFolder.Name = "SetSourceFolder";
            this.SetSourceFolder.Size = new System.Drawing.Size(27, 21);
            this.SetSourceFolder.TabIndex = 2;
            this.SetSourceFolder.Text = "...";
            this.SetSourceFolder.UseVisualStyleBackColor = true;
            this.SetSourceFolder.Click += new System.EventHandler(this.SetSourceFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Config to Update";
            // 
            // ConfigToUpdate
            // 
            this.ConfigToUpdate.Location = new System.Drawing.Point(142, 42);
            this.ConfigToUpdate.Name = "ConfigToUpdate";
            this.ConfigToUpdate.Size = new System.Drawing.Size(345, 20);
            this.ConfigToUpdate.TabIndex = 4;
            // 
            // DetermineConfig
            // 
            this.DetermineConfig.Location = new System.Drawing.Point(142, 71);
            this.DetermineConfig.Name = "DetermineConfig";
            this.DetermineConfig.Size = new System.Drawing.Size(104, 23);
            this.DetermineConfig.TabIndex = 5;
            this.DetermineConfig.Text = "Determine Config";
            this.DetermineConfig.UseVisualStyleBackColor = true;
            this.DetermineConfig.Click += new System.EventHandler(this.DetermineConfig_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.PublishLogPage);
            this.tabControl1.Location = new System.Drawing.Point(3, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1158, 375);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.MissingParameters);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(1150, 349);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Missing Parameters";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage1_Paint);
            // 
            // MissingParameters
            // 
            this.MissingParameters.AllowUserToAddRows = false;
            this.MissingParameters.AllowUserToDeleteRows = false;
            this.MissingParameters.AllowUserToOrderColumns = true;
            this.MissingParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MissingParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.MissingParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MissingParameters.Location = new System.Drawing.Point(1, 1);
            this.MissingParameters.Name = "MissingParameters";
            this.MissingParameters.RowHeadersWidth = 51;
            this.MissingParameters.Size = new System.Drawing.Size(1146, 345);
            this.MissingParameters.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.AllParameterValues);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1150, 349);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "All Values";
            this.tabPage3.UseVisualStyleBackColor = true;
            this.tabPage3.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage3_Paint);
            // 
            // AllParameterValues
            // 
            this.AllParameterValues.AllowUserToAddRows = false;
            this.AllParameterValues.AllowUserToDeleteRows = false;
            this.AllParameterValues.AllowUserToOrderColumns = true;
            this.AllParameterValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AllParameterValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.AllParameterValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AllParameterValues.Location = new System.Drawing.Point(1, 1);
            this.AllParameterValues.Name = "AllParameterValues";
            this.AllParameterValues.RowHeadersWidth = 51;
            this.AllParameterValues.Size = new System.Drawing.Size(1146, 345);
            this.AllParameterValues.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ConsolidatedParameters);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(1150, 349);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Consolidated Parameters";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage2_Paint);
            // 
            // ConsolidatedParameters
            // 
            this.ConsolidatedParameters.AllowUserToAddRows = false;
            this.ConsolidatedParameters.AllowUserToDeleteRows = false;
            this.ConsolidatedParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConsolidatedParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ConsolidatedParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConsolidatedParameters.Location = new System.Drawing.Point(1, 1);
            this.ConsolidatedParameters.Name = "ConsolidatedParameters";
            this.ConsolidatedParameters.RowHeadersWidth = 51;
            this.ConsolidatedParameters.Size = new System.Drawing.Size(1146, 345);
            this.ConsolidatedParameters.TabIndex = 0;
            this.ConsolidatedParameters.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ConsolidatedParameters_CellValueChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.MismatchedParameters);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1150, 349);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Mismatched Parameters";
            this.tabPage4.UseVisualStyleBackColor = true;
            this.tabPage4.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage4_Paint);
            // 
            // MismatchedParameters
            // 
            this.MismatchedParameters.AllowUserToAddRows = false;
            this.MismatchedParameters.AllowUserToDeleteRows = false;
            this.MismatchedParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MismatchedParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.MismatchedParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MismatchedParameters.Location = new System.Drawing.Point(1, 1);
            this.MismatchedParameters.Name = "MismatchedParameters";
            this.MismatchedParameters.RowHeadersWidth = 51;
            this.MismatchedParameters.Size = new System.Drawing.Size(1146, 345);
            this.MismatchedParameters.TabIndex = 1;
            // 
            // PublishLogPage
            // 
            this.PublishLogPage.Controls.Add(this.PublishOutputText);
            this.PublishLogPage.Location = new System.Drawing.Point(4, 22);
            this.PublishLogPage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishLogPage.Name = "PublishLogPage";
            this.PublishLogPage.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishLogPage.Size = new System.Drawing.Size(1150, 349);
            this.PublishLogPage.TabIndex = 4;
            this.PublishLogPage.Text = "Publish Log";
            this.PublishLogPage.UseVisualStyleBackColor = true;
            // 
            // PublishOutputText
            // 
            this.PublishOutputText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PublishOutputText.Location = new System.Drawing.Point(1, 1);
            this.PublishOutputText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishOutputText.Multiline = true;
            this.PublishOutputText.Name = "PublishOutputText";
            this.PublishOutputText.ReadOnly = true;
            this.PublishOutputText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.PublishOutputText.Size = new System.Drawing.Size(1146, 345);
            this.PublishOutputText.TabIndex = 0;
            // 
            // UpdateConfig
            // 
            this.UpdateConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateConfig.Enabled = false;
            this.UpdateConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateConfig.Location = new System.Drawing.Point(848, 479);
            this.UpdateConfig.Name = "UpdateConfig";
            this.UpdateConfig.Size = new System.Drawing.Size(97, 40);
            this.UpdateConfig.TabIndex = 7;
            this.UpdateConfig.Text = "Update Config";
            this.UpdateConfig.UseVisualStyleBackColor = true;
            this.UpdateConfig.Click += new System.EventHandler(this.UpdateConfig_Click);
            // 
            // InstallServices
            // 
            this.InstallServices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.InstallServices.Enabled = false;
            this.InstallServices.Location = new System.Drawing.Point(951, 481);
            this.InstallServices.Name = "InstallServices";
            this.InstallServices.Size = new System.Drawing.Size(97, 40);
            this.InstallServices.TabIndex = 8;
            this.InstallServices.Text = "Install Services";
            this.InstallServices.UseVisualStyleBackColor = true;
            this.InstallServices.Click += new System.EventHandler(this.InstallServices_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(576, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "VS Command Line Path";
            // 
            // VSCommandLinePath
            // 
            this.VSCommandLinePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VSCommandLinePath.Location = new System.Drawing.Point(700, 13);
            this.VSCommandLinePath.Name = "VSCommandLinePath";
            this.VSCommandLinePath.Size = new System.Drawing.Size(451, 20);
            this.VSCommandLinePath.TabIndex = 12;
            // 
            // PublishProgress
            // 
            this.PublishProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PublishProgress.Location = new System.Drawing.Point(9, 481);
            this.PublishProgress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishProgress.Name = "PublishProgress";
            this.PublishProgress.Size = new System.Drawing.Size(823, 39);
            this.PublishProgress.Step = 1;
            this.PublishProgress.TabIndex = 13;
            this.PublishProgress.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(547, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Publish Service Fabric Project";
            // 
            // PublishProjectComboBox
            // 
            this.PublishProjectComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PublishProjectComboBox.FormattingEnabled = true;
            this.PublishProjectComboBox.Location = new System.Drawing.Point(700, 46);
            this.PublishProjectComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishProjectComboBox.Name = "PublishProjectComboBox";
            this.PublishProjectComboBox.Size = new System.Drawing.Size(451, 21);
            this.PublishProjectComboBox.TabIndex = 15;
            // 
            // UninstallServices
            // 
            this.UninstallServices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UninstallServices.Enabled = false;
            this.UninstallServices.Location = new System.Drawing.Point(1054, 482);
            this.UninstallServices.Name = "UninstallServices";
            this.UninstallServices.Size = new System.Drawing.Size(97, 40);
            this.UninstallServices.TabIndex = 16;
            this.UninstallServices.Text = "Uninstall Services";
            this.UninstallServices.UseVisualStyleBackColor = true;
            this.UninstallServices.Click += new System.EventHandler(this.UninstallServices_Click);
            // 
            // StartLocalAzureEmulator
            // 
            this.StartLocalAzureEmulator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartLocalAzureEmulator.Location = new System.Drawing.Point(700, 75);
            this.StartLocalAzureEmulator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.StartLocalAzureEmulator.Name = "StartLocalAzureEmulator";
            this.StartLocalAzureEmulator.Size = new System.Drawing.Size(200, 15);
            this.StartLocalAzureEmulator.TabIndex = 18;
            this.StartLocalAzureEmulator.Text = "Start Local Azure Storage Emulator";
            this.StartLocalAzureEmulator.UseVisualStyleBackColor = true;
            // 
            // ConfigUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1163, 530);
            this.Controls.Add(this.StartLocalAzureEmulator);
            this.Controls.Add(this.UninstallServices);
            this.Controls.Add(this.PublishProjectComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PublishProgress);
            this.Controls.Add(this.VSCommandLinePath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.InstallServices);
            this.Controls.Add(this.UpdateConfig);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.DetermineConfig);
            this.Controls.Add(this.ConfigToUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SetSourceFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SourceFolderPath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConfigUpdater";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Configuration Manager";
            this.Load += new System.EventHandler(this.ConfigUpdater_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MissingParameters)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AllParameterValues)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConsolidatedParameters)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MismatchedParameters)).EndInit();
            this.PublishLogPage.ResumeLayout(false);
            this.PublishLogPage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog SourceFolderBrowser;
        private System.Windows.Forms.TextBox SourceFolderPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SetSourceFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ConfigToUpdate;
        private System.Windows.Forms.Button DetermineConfig;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.DataGridView MissingParameters;
        private System.Windows.Forms.DataGridView AllParameterValues;
        private System.Windows.Forms.DataGridView ConsolidatedParameters;
        private System.Windows.Forms.Button UpdateConfig;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView MismatchedParameters;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox VSCommandLinePath;
        private System.Windows.Forms.ProgressBar PublishProgress;
        private System.Windows.Forms.TabPage PublishLogPage;
        private System.Windows.Forms.TextBox PublishOutputText;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox PublishProjectComboBox;
        private System.Windows.Forms.Button UninstallServices;
        private System.Windows.Forms.Button InstallServices;
        private System.Windows.Forms.CheckBox StartLocalAzureEmulator;
    }
}

