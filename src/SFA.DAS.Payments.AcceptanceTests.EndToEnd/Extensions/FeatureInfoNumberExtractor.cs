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
            return ExtractFrom(feature?.FeatureInfo?.Title)
                ?? ExtractFrom(scenario?.ScenarioInfo?.Title)
                ?? throw new ArgumentException($"Title `{feature?.FeatureInfo?.Title}` does not contain a PV2 number");
        }

        public static string ExtractFrom(string title)
        {
            var match = Regex.Match(title ?? "", @"PV2[-_](\d+)", RegexOptions.IgnoreCase);

            return match.Success
                ? match.Groups[1].Value
                : null;
        }
    }
}