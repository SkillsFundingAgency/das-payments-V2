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

        public ConfigUpdater()
        {
            InitializeComponent();
            _configurationManager = new ConfigurationManager();
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
            PublishServices.Enabled = true;
            UninstallServices.Enabled = true;

            var list = _configurationManager.ConfigurationEntries.Select(x => Directory.GetParent(x.FileName).Parent.Name.Replace("SFA.DAS.Payments.", string.Empty).Replace(".ServiceFabric", string.Empty)).Distinct().ToList();
            list.Insert(0, "Publish All");
            PublishProjectComboBox.DataSource = list;
        }

        private void BindGridsToConfigurationEntries()
        {
            AllParameterValues.DataSource = _configurationManager.ConfigurationEntries.OrderBy(x => x.FileName).ThenBy(x => x.ParameterName).ToSortableBindingList();
            MissingParameters.DataSource = _configurationManager.ConfigurationEntries.Where(x => string.IsNullOrEmpty(x.CurrentValue)).OrderBy(x => x.FileName).ThenBy(x => x.ParameterName).ToSortableBindingList();
            ConsolidatedParameters.DataSource = _configurationManager.GetUniqueConfigEntries().OrderBy(x => x.ParameterName).ToSortableBindingList();
            MismatchedParameters.DataSource = _configurationManager.GetMismatchedParameters().OrderBy(x => x.ParameterName).ToSortableBindingList();

            if (ConsolidatedParameters.Columns["FileName"] != null)
                ConsolidatedParameters.Columns["FileName"].Visible = false;
        }

        private static void AutoResizeColumnWidthsYetAllowUserResizing(DataGridView grid)
        {
            if (grid.Columns.Count == 0) return;

            for (var i = 0; i < grid.Columns.Count - 1; i++)
            {
                grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            grid.Columns[grid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            for (var i = 0; i < grid.Columns.Count; i++)
            {
                var colw = grid.Columns[i].Width;
                grid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                grid.Columns[i].Width = colw;
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void SaveUserParameters()
        {
            Properties.Settings.Default.SourceFolderPath = SourceFolderPath.Text;
            Properties.Settings.Default.ConfigToUpdate = ConfigToUpdate.Text;
            Properties.Settings.Default.VSCommandLinePath = VSCommandLinePath.Text;
            Properties.Settings.Default.Save();
        }

        private void ConsolidatedParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var fileConfigEntry = (FileConfigEntry)ConsolidatedParameters.Rows[e.RowIndex].DataBoundItem;
            _configurationManager.UpdateParametersByName(fileConfigEntry.ParameterName, fileConfigEntry.NewValue);
        }

        private void UpdateConfig_Click(object sender, EventArgs e)
        {
            SaveUserParameters();

            _configurationManager.UpdateConfig(ConfigToUpdate.Text);

            MessageBox.Show("Config updated");
        }

        private void PublishServices_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(VSCommandLinePath.Text) || !File.Exists(VSCommandLinePath.Text))
            {
                MessageBox.Show("Enter a valid path to VsDevCmd.bat (include the file name)");
            }

            var allServiceConfigs = _configurationManager.ConfigurationEntries.Select(x => x.FileName).Distinct().ToList();

            PublishProgress.Visible = true;
            PublishProgress.Value = 0;
            var publishAll = PublishProjectComboBox.SelectedIndex == 0;
            PublishProgress.Maximum = publishAll ? allServiceConfigs.Count : 1;

            if (!publishAll) allServiceConfigs = allServiceConfigs.Where(proj => proj.Contains(PublishProjectComboBox.Text)).ToList();

            tabControl1.SelectedTab = PublishLogPage;

            var args = new List<object> { allServiceConfigs, "Install" };

            backgroundWorker1.RunWorkerAsync(args);

            PublishServices.Enabled = false;
            UninstallServices.Enabled = false;

            PublishOutputText.Text = string.Empty;
        }

        private void UninstallServices_Click(object sender, EventArgs e)
        {
            var allServiceConfigs = _configurationManager.ConfigurationEntries.Select(x => x.FileName).Distinct().ToList();

            PublishProgress.Visible = true;
            PublishProgress.Value = 0;
            var publishAll = PublishProjectComboBox.SelectedIndex == 0;
            PublishProgress.Maximum = publishAll ? allServiceConfigs.Count : 1;

            if (!publishAll) allServiceConfigs = allServiceConfigs.Where(proj => proj.Contains(PublishProjectComboBox.Text)).ToList();

            tabControl1.SelectedTab = PublishLogPage;

            var args = new List<object> { allServiceConfigs, "Uninstall" };

            backgroundWorker1.RunWorkerAsync(args);

            PublishServices.Enabled = false;
            UninstallServices.Enabled = false;

            PublishOutputText.Text = string.Empty;
        }

        /// <summary>
        /// For this method to work you need to run either the compiled exe or visual studio as administrator.
        /// </summary>
        private static void StartServiceFabric()
        {
            var runspaceConfiguration = RunspaceConfiguration.Create();
            var runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
            runspace.Open();
            var pipeline = runspace.CreatePipeline();

            pipeline.Commands.AddScript("Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted");
            pipeline.Commands.AddScript("Start-Service FabricHostSvc");
            pipeline.Invoke();
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Do not access the form's BackgroundWorker reference directly.
            // Instead, use the reference provided by the sender parameter.
            var bw = sender as BackgroundWorker;

            var args = e.Argument as IEnumerable<object>;

            var allServiceConfigs = args.First() as IEnumerable<string>;
            var operationName = args.Last() as string;

            StartServiceFabric();

            foreach (var serviceConfig in allServiceConfigs)
            {
                var packagePublisher = new PackagePublisher();
                var solutionPath = Directory.GetParent(serviceConfig).Parent.FullName;

                if (operationName == "Install")
                {
                    packagePublisher.PublishPackage(
                        solutionPath,
                        ConfigToUpdate.Text,
                        VSCommandLinePath.Text,
                        update =>
                        {
                            bw.ReportProgress(0, update);
                        });
                }
                else if (operationName == "Uninstall")
                {
                    packagePublisher.UnInstallServiceFabricApp(
                        solutionPath,
                        update =>
                        {
                            bw.ReportProgress(0, update);
                        });
                }

                bw.ReportProgress(1);
            }

            AzureStorageEmulatorManager.StartStorageEmulator();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // There was an error during the operation.
                var msg = $"An error occurred: {e.Error.Message}";
                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("Packages published");
            }

            PublishServices.Enabled = true;
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
