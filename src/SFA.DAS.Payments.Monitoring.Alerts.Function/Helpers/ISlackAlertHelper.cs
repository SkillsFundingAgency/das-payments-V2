using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public interface ISlackAlertHelper
    {
        public string GetEmoji(string severity);

        public List<Block> BuildSlackPayload(string alertEmoji,
                                              DateTime timestamp,
                                              string jobId,
                                              string academicYear,
                                              string collectionPeriod,
                                              string collectionPeriodPayments,
                                              string yearToDatePayments,
                                              string numberOfLearners,
                                              string alertTitle,
                                              string appInsightsSearchResultsUiLink);

        public Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp);

        public string GetSlackAlertTitle(string alertTitleFormat, Dictionary<string, string> alertVariables);

        public string GetSlackAlertText(string alertTextFormat, Dictionary<string, string> alertVariables);
    }
}
