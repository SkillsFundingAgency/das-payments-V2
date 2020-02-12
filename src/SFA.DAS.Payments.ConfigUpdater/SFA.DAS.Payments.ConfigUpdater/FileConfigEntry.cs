namespace SFA.DAS.Payments.ConfigUpdater
{
    internal class FileConfigEntry
    {
        public FileConfigEntry(string fileName, string parameterName, string defaultValue, string currentValue)
        {
            FileName = fileName;
            ParameterName = parameterName;
            DefaultValue = defaultValue;
            CurrentValue = currentValue;
            NewValue = string.IsNullOrEmpty(currentValue) ? defaultValue : currentValue;
        }

        public string FileName { get; }
        public string ParameterName { get; }
        public string DefaultValue { get; }
        public string CurrentValue { get; }
        public string NewValue { get; set; }
    }
}
