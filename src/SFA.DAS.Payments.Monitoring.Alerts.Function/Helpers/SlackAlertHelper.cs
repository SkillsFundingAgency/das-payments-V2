using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public class SlackAlertHelper : ISlackAlertHelper
    {
        public List<object> BuildSlackPayload(string alertEmoji,
                                              DateTime timestamp,
                                              string jobId,
                                              string academicYear,
                                              string collectionPeriod,
                                              string collectionPeriodPayments,
                                              string yearToDatePayments,
                                              string numberOfLearners,
                                              string alertTitle,
                                              string appInsightsSearchResultsUiLink)
        {
            return new List<object>
            {
                new
                {
                    type ="header",
                    text = new
                    {
                        type= "plain_text",
                        text = $"{alertEmoji} {alertTitle}."
                    }
                },
                new
                {
                    type = "section",
                    text = new
                    {
                        text = $"<{appInsightsSearchResultsUiLink}|View in Azure App Insights>",
                        type = "mrkdwn"
                    },
                    fields = new List<object>
                    {
                        new { type = "mrkdwn", text = "*Timestamp*" },
                        new { type = "mrkdwn", text = "*Job*" },
                        new { type = "plain_text", text = timestamp.ToString("f") },
                        new { type = "plain_text", text = jobId },
                        new { type = "mrkdwn", text = "*Academic Year*" },
                        new { type = "mrkdwn", text = "*Collection Period*" },
                        new { type = "mrkdwn", text = "*Previous Payments Year To Date*" },
                        new { type = "mrkdwn", text = "*Collection Period Payments*" },
                        new { type = "mrkdwn", text = "*In Learning*" },
                        new { type = "plain_text", text = academicYear },
                        new { type = "plain_text", text = collectionPeriod },
                        new { type = "plain_text", text = yearToDatePayments },
                        new { type = "plain_text", text = collectionPeriodPayments },
                        new { type = "plain_text", text = numberOfLearners },
                    }
                }
            };
        }

        public string GetEmoji(string severity)
        {
            return severity switch
            {
                "Sev0" or "Sev1" => ":alert:",
                "Sev2" => ":warning:",
                "Sev3" => ":+1:",
                _ => string.Empty,
            };
        }

        public Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp)
        {
            double percentage = customMeasurements.ContainsKey("Percentage") ? customMeasurements["Percentage"] : 0;
            double duration = customMeasurements.ContainsKey("Duration") ? customMeasurements["Duration"] : 0;
            string ukprn = customDimensions.ContainsKey("Ukprn") ? customDimensions["Ukprn"] : string.Empty;
            string jobId = customDimensions.ContainsKey("JobId") ? customDimensions["JobId"] : string.Empty;
            string academicYear = customDimensions.ContainsKey("AcademicYear") ? customDimensions["AcademicYear"] : string.Empty;
            string collectionPeriod = customDimensions.ContainsKey("CollectionPeriod") ? customDimensions["CollectionPeriod"] : string.Empty;

            string dcEarningsTotal = customMeasurements.ContainsKey("DcEarningsTotal") ?
                                      customMeasurements["DcEarningsTotal"].ToString() :
                                        customMeasurements.ContainsKey("EarningsDCTotal") ?
                                        customMeasurements["EarningsDCTotal"].ToString() :
                                        string.Empty;

            string dasEarningsTotal = customMeasurements.ContainsKey("DasEarningsTotal") ?
                                       customMeasurements["DasEarningsTotal"].ToString() :
                                       string.Empty;

            string adjustedDataLockedEarnings = customMeasurements.ContainsKey("DataLockedEarningsAmount") ?
                                                 customMeasurements["DataLockedEarningsAmount"].ToString() :
                                                    customMeasurements.ContainsKey("DataLockedEarnings") ?
                                                    customMeasurements["DataLockedEarnings"].ToString() :
                                                    string.Empty;

            string differenceTotal = customMeasurements.ContainsKey("DifferenceTotal") ?
                                      customMeasurements["DifferenceTotal"].ToString() :
                                      string.Empty;

            string heldBackCompletionPayments = customMeasurements.ContainsKey("HeldBackCompletionPayments") ?
                                                 customMeasurements["HeldBackCompletionPayments"].ToString() :
                                                 string.Empty;

            string requiredPayments = customMeasurements.ContainsKey("RequiredPaymentsTotal") ?
                                       customMeasurements["RequiredPaymentsTotal"].ToString() :
                                       string.Empty;

            string collectionPeriodPayments = customMeasurements.ContainsKey("PaymentsTotal") ?
                                              customMeasurements["PaymentsTotal"].ToString() :
                                              string.Empty;

            string yearToDatePayments = customMeasurements.ContainsKey("YearToDatePaymentsTotal") ?
                                         customMeasurements["YearToDatePaymentsTotal"].ToString() :
                                            customMeasurements.ContainsKey("PaymentsYearToDateTotal") ?
                                            customMeasurements["PaymentsYearToDateTotal"].ToString() :
                                            string.Empty;

            string numberOfLearners = customMeasurements.ContainsKey("InLearning") ?
                                       customMeasurements["InLearning"].ToString() :
                                       string.Empty;

            return new Dictionary<string, string>
            {
                { "Ukprn", ukprn },
                { "JobId", jobId },
                { "AcademicYear", academicYear },
                { "CollectionPeriod", collectionPeriod },
                { "Timestamp", timestamp.ToString("F") },
                { "Accuracy", percentage >= 0 ? percentage.ToString() : string.Empty },
                { "Duration", duration > 0 ? duration.ToString() : string.Empty },
                { "DcEarningsTotal", dcEarningsTotal },
                { "DasEarningsTotal", dasEarningsTotal },
                { "AdjustedDataLockedEarnings", adjustedDataLockedEarnings },
                { "DifferenceTotal", differenceTotal },
                { "HeldBackCompletionPayments", heldBackCompletionPayments },
                { "RequiredPayments", requiredPayments },
                { "CollectionPeriodPayments", collectionPeriodPayments },
                { "YearToDatePayments", yearToDatePayments },
                { "NumberOfLearners", numberOfLearners }
            };
        }

        public string GetSlackAlertTitle(string alertTitleFormat, Dictionary<string, string> alertVariables)
        {
            foreach (var alertVariable in alertVariables)
            {
                alertTitleFormat = alertTitleFormat.Replace("{" + alertVariable.Key + "}", alertVariable.Value);
            }

            return alertTitleFormat;
        }

        public string GetSlackAlertText(string alertTextFormat, Dictionary<string, string> alertVariables)
        {
            foreach (var alertVariable in alertVariables)
            {
                alertTextFormat = alertTextFormat.Replace("{" + alertVariable.Key + "}", "*" + alertVariable.Value + "*");
            }

            return alertTextFormat;
        }
    }
}