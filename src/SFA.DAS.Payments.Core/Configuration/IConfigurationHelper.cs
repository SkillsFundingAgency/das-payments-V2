namespace SFA.DAS.Payments.Core.Configuration
{
    public interface IConfigurationHelper
    {
        string GetSetting(string sectionName, string settingName);
    }
}