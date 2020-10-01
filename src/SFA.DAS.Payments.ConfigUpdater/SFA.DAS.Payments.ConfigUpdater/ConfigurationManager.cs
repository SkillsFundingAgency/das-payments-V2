using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SFA.DAS.Payments.ConfigUpdater
{
    internal class ConfigurationManager
    {
        public List<FileConfigEntry> ConfigurationEntries { get; private set; }

        private readonly XNamespace _ns = "http://schemas.microsoft.com/2011/01/fabric";

        public void GetCurrentConfiguration(string sourceDirectory, string configToUpdate)
        {
            var configFiles = GetAllCloudConfigFiles(sourceDirectory, configToUpdate);
            var allConfigEntries = GetAllConfigEntries(configFiles, configToUpdate);
            ConfigurationEntries = allConfigEntries.ToList();
        }

        public List<FileConfigEntry> GetUniqueConfigEntries()
        {
            var groupedParameters = ConfigurationEntries.GroupBy(x => x.ParameterName);

            var uniqueConfigEntries = new List<FileConfigEntry>();
            foreach (var groupedParameter in groupedParameters)
            {
                uniqueConfigEntries.Add(groupedParameter.First());
            }

            return uniqueConfigEntries;
        }

        public void UpdateParametersByName(string parameterName, string newValue)
        {
            var matchingEntries = ConfigurationEntries.Where(x => x.ParameterName == parameterName);
            foreach (var matchingEntry in matchingEntries)
            {
                matchingEntry.NewValue = newValue;
            }
        }

        public IEnumerable<FileConfigEntry> GetMismatchedParameters()
        {
            var mismatchedParameters = new List<FileConfigEntry>();
            var groupedEntries = ConfigurationEntries.GroupBy(x => x.ParameterName);
            foreach (var configGroup in groupedEntries)
            {
                if (configGroup.Any(x => x.CurrentValue != configGroup.First().CurrentValue))
                {
                    mismatchedParameters.AddRange(configGroup);
                }
            }

            return mismatchedParameters;
        }

        public void UpdateConfig(string configToUpdate)
        {
            var configByFiles = ConfigurationEntries.GroupBy(x => x.FileName);
            foreach (var configFile in configByFiles)
            {
                UpdateConfigFile(configFile.Key, configFile.Select(x => x), configToUpdate);
            }
        }

        private IEnumerable<FileConfigEntry> GetAllConfigEntries(IEnumerable<string> configFiles, string configToUpdate)
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
                             select new { Name = (string)parameter.Attribute("Name"), Value = (string)parameter.Attribute("Value") };

            var currentConfigValues = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                if (!currentConfigValues.ContainsKey(parameter.Name))
                {
                    currentConfigValues.Add(parameter.Name, parameter.Value);
                }
                else
                {
                    MessageBox.Show($"Paramenter: {parameter.Name} is repeated in file: {configFileToUpdate}");
                }
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

        private IEnumerable<string> GetConfigEntriesFromFile(string configFile)
        {
            var file = XElement.Load(configFile);
            var parameters = from parameter in file.Descendants(_ns + "Parameter")
                             select (string)parameter.Attribute("Name");

            return parameters.ToList();
        }

        private static List<string> GetAllCloudConfigFiles(string sourcePath, string configToUpdate)
        {
            var appParametersDirectories = Directory.GetDirectories(sourcePath, "ApplicationParameters", SearchOption.AllDirectories);
            var allConfigFiles = new List<string>();

            foreach (var directory in appParametersDirectories)
            {
                var configFiles = Directory.GetFiles(directory, configToUpdate, SearchOption.TopDirectoryOnly);
                allConfigFiles.AddRange(configFiles);
            }

            return allConfigFiles.ToList();
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

            var newFileName = Directory.GetParent(configFile) + @"\" + configToUpdate;
            cloudConfigFile.Save(newFileName);
        }
    }
}
