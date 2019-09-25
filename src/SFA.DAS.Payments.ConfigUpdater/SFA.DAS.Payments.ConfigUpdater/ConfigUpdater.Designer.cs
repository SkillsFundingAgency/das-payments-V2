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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ConsolidatedParameters = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.AllParameterValues = new System.Windows.Forms.DataGridView();
            this.UpdateConfig = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MissingParameters)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConsolidatedParameters)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AllParameterValues)).BeginInit();
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
            this.label2.Location = new System.Drawing.Point(14, 46);
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1212, 394);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.MissingParameters);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1204, 368);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Missing Parameters";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // MissingParameters
            // 
            this.MissingParameters.AllowUserToAddRows = false;
            this.MissingParameters.AllowUserToDeleteRows = false;
            this.MissingParameters.AllowUserToOrderColumns = true;
            this.MissingParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.MissingParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MissingParameters.Location = new System.Drawing.Point(6, 6);
            this.MissingParameters.Name = "MissingParameters";
            this.MissingParameters.Size = new System.Drawing.Size(1195, 356);
            this.MissingParameters.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ConsolidatedParameters);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1204, 368);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Consolidated Parameters";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ConsolidatedParameters
            // 
            this.ConsolidatedParameters.AllowUserToAddRows = false;
            this.ConsolidatedParameters.AllowUserToDeleteRows = false;
            this.ConsolidatedParameters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ConsolidatedParameters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConsolidatedParameters.Location = new System.Drawing.Point(3, 3);
            this.ConsolidatedParameters.Name = "ConsolidatedParameters";
            this.ConsolidatedParameters.Size = new System.Drawing.Size(1195, 362);
            this.ConsolidatedParameters.TabIndex = 0;
            this.ConsolidatedParameters.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ConsolidatedParameters_CellValueChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.AllParameterValues);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1204, 368);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "All Values";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // AllParameterValues
            // 
            this.AllParameterValues.AllowUserToAddRows = false;
            this.AllParameterValues.AllowUserToDeleteRows = false;
            this.AllParameterValues.AllowUserToOrderColumns = true;
            this.AllParameterValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.AllParameterValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AllParameterValues.Location = new System.Drawing.Point(3, 3);
            this.AllParameterValues.Name = "AllParameterValues";
            this.AllParameterValues.Size = new System.Drawing.Size(1195, 362);
            this.AllParameterValues.TabIndex = 0;
            // 
            // UpdateConfig
            // 
            this.UpdateConfig.Enabled = false;
            this.UpdateConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateConfig.Location = new System.Drawing.Point(1108, 501);
            this.UpdateConfig.Name = "UpdateConfig";
            this.UpdateConfig.Size = new System.Drawing.Size(97, 40);
            this.UpdateConfig.TabIndex = 7;
            this.UpdateConfig.Text = "Update Config";
            this.UpdateConfig.UseVisualStyleBackColor = true;
            this.UpdateConfig.Click += new System.EventHandler(this.UpdateConfig_Click);
            // 
            // ConfigUpdater
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 553);
            this.Controls.Add(this.UpdateConfig);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.DetermineConfig);
            this.Controls.Add(this.ConfigToUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SetSourceFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SourceFolderPath);
            this.Name = "ConfigUpdater";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ConfigUpdater_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MissingParameters)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConsolidatedParameters)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AllParameterValues)).EndInit();
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
    }
}

