namespace SFA.DAS.Payments.Core.Configuration
{
    public interface IConfigurationHelper
    {
        bool HasSetting(string sectionName, string settingName);
        string GetSetting(string sectionName, string settingName);
    }
}