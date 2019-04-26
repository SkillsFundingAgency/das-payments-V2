using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions
{
    public class FeatureNumber
    {
        private readonly ScenarioContext scenario;
        private readonly FeatureContext feature;

        public FeatureNumber(ScenarioContext scenario, FeatureContext feature)
        {
            this.scenario = scenario;
            this.feature = feature;
        }

        public string Extract()
        {
            try
            {
                return ExtractFrom(feature?.FeatureInfo?.Title);
            }
            catch
            {
                return ExtractFrom(scenario?.ScenarioInfo?.Title);
            }
        }

        public static string ExtractFrom(string title)
        {
            var match = Regex.Match(title ?? "", @"PV2[-_](\d+)", RegexOptions.IgnoreCase);

            var number = match.Success 
                ? match.Groups[1].Value 
                : throw new ArgumentException($"Title `{title}` does not contain a PV2 number");

            return number;
        }
    }
}