using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public partial class ConfigUpdater : Form
    {
        private ConfigurationManager _configurationManager;

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
        }

        private void BindGridsToConfigurationEntries()
        {
            AllParameterValues.DataSource = _configurationManager.ConfigurationEntries.OrderBy(x => x.FileName).ThenBy(x => x.ParameterName).ToList();
            MissingParameters.DataSource = _configurationManager.ConfigurationEntries.Where(x => string.IsNullOrEmpty(x.CurrentValue)).OrderBy(x => x.FileName).ThenBy(x => x.ParameterName).ToList();
            ConsolidatedParameters.DataSource = _configurationManager.GetUniqueConfigEntries().OrderBy(x => x.ParameterName).ToList();
            MismatchedParameters.DataSource = _configurationManager.GetMismatchedParameters().OrderBy(x => x.ParameterName).ToList();
            ConsolidatedParameters.Columns["FileName"].Visible = false;
        }

        private void SaveUserParameters()
        {
            Properties.Settings.Default.SourceFolderPath = SourceFolderPath.Text;
            Properties.Settings.Default.ConfigToUpdate = ConfigToUpdate.Text;
            Properties.Settings.Default.Save();
        }

        private void ConsolidatedParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var fileConfigEntry = (FileConfigEntry)ConsolidatedParameters.Rows[e.RowIndex].DataBoundItem;
            _configurationManager.UpdateParametersByName(fileConfigEntry.ParameterName, fileConfigEntry.NewValue);
        }

        private void UpdateConfig_Click(object sender, EventArgs e)
        {
            _configurationManager.UpdateConfig(ConfigToUpdate.Text);

            MessageBox.Show("Config updated");
        }

        private void PublishServices_Click(object sender, EventArgs e)
        {
            var allServiceConfigs = _configurationManager.ConfigurationEntries.Select(x => x.FileName).Distinct();
            foreach (var serviceConfig in allServiceConfigs)
            {
                var packagePublisher = new PackagePublisher();
                packagePublisher.PublishPackage(Directory.GetParent(serviceConfig).Parent.FullName, ConfigToUpdate.Text);
            }

            MessageBox.Show("Packages published");
        }
    }
}
