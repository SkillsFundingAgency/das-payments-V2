using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;
using SFA.DAS.Testing.AzureStorageEmulator;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public partial class ConfigUpdater : Form
    {
        private readonly ConfigurationManager _configurationManager;
        private bool _isServiceFabricStarted;
        
        public ConfigUpdater()
        {
            InitializeComponent();
            _configurationManager = new ConfigurationManager();
            _isServiceFabricStarted = false;
        }

        private void SetSourceFolder_Click(object sender, EventArgs e)
        {
            if (SourceFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                SourceFolderPath.Text = SourceFolderBrowser.SelectedPath;
            }
        }

        private void ConfigUpdater_Load(object sender, EventArgs e)
        {
            SourceFolderPath.Text = Properties.Settings.Default.SourceFolderPath;
            ConfigToUpdate.Text = Properties.Settings.Default.ConfigToUpdate;
            VSCommandLinePath.Text = Properties.Settings.Default.VSCommandLinePath;
        }

        private void DetermineConfig_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConfigToUpdate.Text))
            {
                MessageBox.Show("Enter a config file to update.");
                return;
            }

            SaveUserParameters();

            _configurationManager.GetCurrentConfiguration(SourceFolderPath.Text, ConfigToUpdate.Text);

            BindGridsToConfigurationEntries();

            UpdateConfig.Enabled = true;
            InstallServices.Enabled = true;
            UninstallServices.Enabled = true;

            string GetFileName(FileConfigEntry fileConfig)
            {
                var dirName = Directory.GetParent(fileConfig.FileName)
                                       .Parent;
                return dirName == null
                    ? "Invalid Path"
                    : dirName.Name
                             .Replace("SFA.DAS.Payments.", string.Empty)
                             .Replace(".ServiceFabric", string.Empty);
            }

            var list = _configurationManager.ConfigurationEntries.Select(GetFileName)
                                            .Distinct()
                                            .ToList();

            list.Insert(0, "Select All");
            
            PublishProjectComboBox.DataSource = list;
        }

        private void UpdateConfig_Click(object sender, EventArgs e)
        {
            SaveUserParameters();

            _configurationManager.UpdateConfig(ConfigToUpdate.Text);

            MessageBox.Show("Config updated");
        }

        private void SaveUserParameters()
        {
            Properties.Settings.Default.SourceFolderPath = SourceFolderPath.Text;
            Properties.Settings.Default.ConfigToUpdate = ConfigToUpdate.Text;
            Properties.Settings.Default.VSCommandLinePath = VSCommandLinePath.Text;
            Properties.Settings.Default.Save();
        }

        private void BindGridsToConfigurationEntries()
        {
            AllParameterValues.DataSource = _configurationManager.ConfigurationEntries
                                                                 .OrderBy(x => x.FileName)
                                                                 .ThenBy(x => x.ParameterName)
                                                                 .ToSortableBindingList();

            MissingParameters.DataSource = _configurationManager.ConfigurationEntries
                                                                .Where(x => string.IsNullOrEmpty(x.CurrentValue))
                                                                .OrderBy(x => x.FileName)
                                                                .ThenBy(x => x.ParameterName)
                                                                .ToSortableBindingList();

            ConsolidatedParameters.DataSource = _configurationManager.GetUniqueConfigEntries()
                                                                     .OrderBy(x => x.ParameterName)
                                                                     .ToSortableBindingList();

            MismatchedParameters.DataSource = _configurationManager.GetMismatchedParameters()
                                                                   .OrderBy(x => x.ParameterName)
                                                                   .ToSortableBindingList();

            if (ConsolidatedParameters.Columns["FileName"] != null)
                ConsolidatedParameters.Columns["FileName"].Visible = false;
        }

        private void ConsolidatedParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var fileConfigEntry = (FileConfigEntry) ConsolidatedParameters.Rows[e.RowIndex].DataBoundItem;
            _configurationManager.UpdateParametersByName(fileConfigEntry.ParameterName, fileConfigEntry.NewValue);
        }

        private void tabPage1_Paint(object sender, PaintEventArgs e)
        {
            AutoResizeColumnWidthsYetAllowUserResizing(MissingParameters);
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
            AutoResizeColumnWidthsYetAllowUserResizing(ConsolidatedParameters);
        }

        private void tabPage3_Paint(object sender, PaintEventArgs e)
        {
            AutoResizeColumnWidthsYetAllowUserResizing(AllParameterValues);
        }

        private void tabPage4_Paint(object sender, PaintEventArgs e)
        {
            AutoResizeColumnWidthsYetAllowUserResizing(MismatchedParameters);
        }

        private static void AutoResizeColumnWidthsYetAllowUserResizing(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;

            for (var i = 0; i < grid.Columns.Count; i++)
            {
                var colw = grid.Columns[i].Width;
                grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns[i].Width = colw;
                grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void InstallServices_Click(object sender, EventArgs e)
        {
            RunBackgroundJob("Install");
        }

        private void UninstallServices_Click(object sender, EventArgs e)
        {
            RunBackgroundJob("Uninstall");
        }

        private void RunBackgroundJob(string taskName)
        {
            if (string.IsNullOrEmpty(VSCommandLinePath.Text) || !File.Exists(VSCommandLinePath.Text))
            {
                MessageBox.Show("Enter a valid path to VsDevCmd.bat (include the file name)");
            }

            var allServiceConfigs = _configurationManager.ConfigurationEntries
                                                         .Select(x => x.FileName)
                                                         .Distinct()
                                                         .ToList();

            PublishProgress.Visible = true;

            PublishProgress.Value = 0;

            var publishAll = PublishProjectComboBox.SelectedIndex == 0;

            PublishProgress.Maximum = publishAll ? allServiceConfigs.Count : 1;

            if (!publishAll)
                allServiceConfigs = allServiceConfigs.Where(proj => proj.Contains(PublishProjectComboBox.Text))
                                                     .ToList();

            tabControl1.SelectedTab = PublishLogPage;

            var args = new List<object> { allServiceConfigs, taskName, StartLocalAzureEmulator.Checked };

            backgroundWorker1.RunWorkerAsync(args);

            InstallServices.Enabled = false;

            UninstallServices.Enabled = false;

            PublishOutputText.Text = string.Empty;
        }

        /// <summary>
        /// For this method to work you need to run either the compiled exe or visual studio as administrator.
        /// </summary>
        private void StartServiceFabric()
        {
            var runspaceConfiguration = RunspaceConfiguration.Create();
            var runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            var pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript("Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted");
            pipeline.Commands.AddScript("Start-Service FabricHostSvc");
            pipeline.Invoke();
            _isServiceFabricStarted = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.

            if (!( sender is BackgroundWorker bw )) throw new ArgumentException("Background Worker is Null");
            if (!( e.Argument is IList<object> args ) || args.Count < 3) throw new ArgumentException("Background Worker should have three arguments");

            if (!( args[0] is IEnumerable<string> allServiceConfigs )) throw new ArgumentException("List of Service Config is null");
            if (!( args[1] is string operationName )) throw new ArgumentException("background operation name is null");
            if (!( args[2] is bool startLocalAzureEmulator )) throw new ArgumentException("Start Local Azure Emulator is Null");

            if (!_isServiceFabricStarted)
                StartServiceFabric();

            foreach (var serviceConfig in allServiceConfigs)
            {
                var packagePublisher = new PackagePublisher();
                var solutionPath = Directory.GetParent(serviceConfig)
                                            .Parent?.FullName;

                switch (operationName)
                {
                    case "Install":
                        packagePublisher.PublishPackage(solutionPath, ConfigToUpdate.Text, VSCommandLinePath.Text, update => { bw.ReportProgress(0, update); });
                        break;
                    case "Uninstall":
                        packagePublisher.UnInstallServiceFabricApp(solutionPath, update => { bw.ReportProgress(0, update); });
                        break;
                }

                bw.ReportProgress(1);
            }

            if (startLocalAzureEmulator)
                AzureStorageEmulatorManager.StartStorageEmulator();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PublishOutputText.AppendText(e.Error != null
                                             ? $"An error occurred: {e.Error.Message}"
                                             : "Finished");

            InstallServices.Enabled = true;

            UninstallServices.Enabled = true;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 0)
                PublishProgress.Value += 1;

            if (e.UserState != null && !string.IsNullOrWhiteSpace(e.UserState.ToString()))
                PublishOutputText.AppendText(e.UserState + Environment.NewLine);
        }
    }
}