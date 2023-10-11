using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Slack.Helpers
{
    public interface ISlackAlertHelper
    {
        string GetEmoji(string severity);

        List<object> BuildSlackPayload(string alertEmoji,
                                              DateTime timestamp,
                                              string jobId,
                                              string academicYear,
                                              string collectionPeriod,
                                              string alertTitle,
                                              string appInsightsSearchResultsUiLink);

        List<object> BuildSlackPayload(string alertEmoji,
                                              DateTime timestamp,
                                              string jobId,
                                              string academicYear,
                                              string collectionPeriod,
                                              string alertTitle);

        Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp);

        string GetSlackAlertTitle(string alertTitleFormat, Dictionary<string, string> alertVariables);

        string GetSlackAlertText(string alertTextFormat, Dictionary<string, string> alertVariables);
    }
}
