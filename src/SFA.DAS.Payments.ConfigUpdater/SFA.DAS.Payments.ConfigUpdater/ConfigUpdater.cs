using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SFA.DAS.Payments.ConfigUpdater
{
    public partial class ConfigUpdater : Form
    {
        private List<FileConfigEntry> _configurationEntries;
        private XNamespace _ns = "http://schemas.microsoft.com/2011/01/fabric";

        public ConfigUpdater()
        {
            InitializeComponent();
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

        private List<FileConfigEntry> GetCurrentConfiguration(string sourceDirectory, string configToUpdate)
        {
            var configFiles = GetAllCloudConfigFiles(sourceDirectory);
            var allConfigEntries = GetAllConfigEntries(configFiles, configToUpdate);
            return allConfigEntries.ToList();
        }

        private List<FileConfigEntry> GetUniqueConfigEntries(IEnumerable<FileConfigEntry> allConfigEntries)
        {
            var groupedParameters = allConfigEntries.GroupBy(x => x.ParameterName);

            var uniqueConfigEntries = new List<FileConfigEntry>();
            foreach (var groupedParameter in groupedParameters)
            {
                uniqueConfigEntries.Add(groupedParameter.First());
            }

            return uniqueConfigEntries;
        }

        private IEnumerable<FileConfigEntry> GetAllConfigEntries(List<string> configFiles, string configToUpdate)
        {
            var allConfigEntries = new List<FileConfigEntry>();
            foreach (var configFile in configFiles)
            {
                var fileConfigEntries = GetConfigEntriesFromFile(configFile);
                var defaultConfigValues = GetDefaultValuesFromManifest(configFile);
                var currentConfigValues = GetCurrentConfigValues(configFile, configToUpdate);

                foreach (var configEntry in fileConfigEntries)
                {
                    allConfigEntries.Add(
                        new FileConfigEntry(
                            configFile,
                        configEntry,
                        defaultConfigValues.ContainsKey(configEntry) ? defaultConfigValues[configEntry] : string.Empty,
                        currentConfigValues.ContainsKey(configEntry) ? currentConfigValues[configEntry] : string.Empty
                        ));
                }
            }

            return allConfigEntries;
        }

        private Dictionary<string, string> GetCurrentConfigValues(string configFile, string configToUpdate)
        {
            var baseFolder = Directory.GetParent(configFile).FullName;
            var configFileToUpdate = baseFolder + @"\" + configToUpdate;

            if (!File.Exists(configFileToUpdate))
            {
                return new Dictionary<string, string>();
            }

            var existingConfig = XElement.Load(configFileToUpdate);
            
            var parameters = from parameter in existingConfig.Descendants(_ns + "Parameter")
                where !string.IsNullOrEmpty(parameter.Attribute("Value").Value)
                select new {Name = (string) parameter.Attribute("Name"), Value = (string) parameter.Attribute("Value")};

            var currentConfigValues = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                currentConfigValues.Add(parameter.Name, parameter.Value);
            }

            return currentConfigValues;
        }

        private Dictionary<string, string> GetDefaultValuesFromManifest(string configFile)
        {
            var baseFolder = Directory.GetParent(configFile).Parent.FullName;
            var manifestPath = baseFolder + @"\ApplicationPackageRoot\ApplicationManifest.xml";
            var manifest = XElement.Load(manifestPath);

            var defaults = from parameters in manifest.Element(_ns + "Parameters").Descendants(_ns + "Parameter")
                           where !string.IsNullOrEmpty(parameters.Attribute("DefaultValue").Value)
                           select new
                           {
                               Name = (string)parameters.Attribute("Name"),
                               DefaultValue = (string)parameters.Attribute("DefaultValue")
                           };

            var defaultValues = new Dictionary<string, string>();
            foreach (var parameterDefault in defaults)
            {
                defaultValues.Add(parameterDefault.Name, parameterDefault.DefaultValue);
            }

            return defaultValues;
        }

        private List<string> GetConfigEntriesFromFile(string configFile)
        {
            var file = XElement.Load(configFile);
            var parameters = from parameter in file.Descendants(_ns + "Parameter")
                             select (string)parameter.Attribute("Name");

            return parameters.ToList();
        }

        private List<string> GetAllCloudConfigFiles(string sourcePath)
        {
            var appParametersDirectories = Directory.GetDirectories(sourcePath, "ApplicationParameters", SearchOption.AllDirectories);
            var allConfigFiles = new List<string>();

            foreach (var directory in appParametersDirectories)
            {
                var configFiles = Directory.GetFiles(directory, @"Cloud.xml", SearchOption.TopDirectoryOnly);
                allConfigFiles.AddRange(configFiles);
            }

            return allConfigFiles.ToList();
        }

        private void DetermineConfig_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConfigToUpdate.Text))
            {
                MessageBox.Show("Enter a config file to update.");
                return;
            }

            SaveUserParameters();

            _configurationEntries = GetCurrentConfiguration(SourceFolderPath.Text, ConfigToUpdate.Text);
            BindGridsToConfigurationEntries();

            UpdateConfig.Enabled = true;
        }

        private void BindGridsToConfigurationEntries()
        {
            AllParameterValues.DataSource = _configurationEntries.OrderBy(x => x.FileName).ThenBy(x => x.ParameterName).ToList();
            MissingParameters.DataSource = _configurationEntries.Where(x => string.IsNullOrEmpty(x.CurrentValue)).OrderBy(x => x.FileName).ThenBy(x => x.ParameterName).ToList();
            ConsolidatedParameters.DataSource = GetUniqueConfigEntries(_configurationEntries).OrderBy(x => x.ParameterName).ToList();
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

            var matchingEntries = _configurationEntries.Where(x => x.ParameterName == fileConfigEntry.ParameterName);
            foreach (var matchingEntry in matchingEntries)
            {
                matchingEntry.NewValue = fileConfigEntry.NewValue;
            }
        }

        private void UpdateConfig_Click(object sender, EventArgs e)
        {
            var configByFiles = _configurationEntries.GroupBy(x => x.FileName);
            foreach (var configFile in configByFiles)
            {
                UpdateConfigFile(configFile.Key, configFile.Select(x => x), ConfigToUpdate.Text);
            }

            MessageBox.Show("Config updated");
        }

        private void UpdateConfigFile(string configFile, IEnumerable<FileConfigEntry> configEntries, string configToUpdate)
        {
            var cloudConfigFile = XElement.Load(configFile);
            var parameterNodes = cloudConfigFile.Descendants(_ns + "Parameter");
            foreach (var configEntry in configEntries)
            {
                var matchingParameter = parameterNodes.Single(x => x.Attribute("Name").Value == configEntry.ParameterName);
                matchingParameter.Attribute("Value").Value = configEntry.NewValue;
            }

            var newFileName = Directory.GetParent(configFile) + @"\test" + configToUpdate;
            cloudConfigFile.Save(newFileName);
        }
    }
}
