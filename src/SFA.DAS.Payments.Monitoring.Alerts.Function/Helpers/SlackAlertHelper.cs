using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public class SlackAlertHelper : ISlackAlertHelper
    {
        public List<Block> BuildSlackPayload(string alertEmoji,
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
            var blocks = new List<Block>
            {
                new Block
                {
                    Type = "header",
                    Text = new BlockData
                    {
                        Type = "plain_text",
                        Text = $"{alertEmoji} {alertTitle}."
                    }
                },
                new Block
                {
                    Type = "section",
                    Text = new BlockData
                    {
                        Type = "mrkdwn",
                        Text = $"<{appInsightsSearchResultsUiLink}|View in Azure App Insights>"
                    },
                    Fields = new List<BlockData>
                    {
                        new BlockData { Type = "mrkdwn", Text = "*Timestamp*" },
                        new BlockData { Type = "mrkdwn", Text = "*Job*" },
                        new BlockData { Type = "plain_text", Text = timestamp.ToString("f") },
                        new BlockData { Type = "plain_text", Text = jobId },
                        new BlockData { Type = "mrkdwn", Text = "*Academic Year*" },
                        new BlockData { Type = "mrkdwn", Text = "*Collection Period*" },
                        new BlockData { Type = "plain_text", Text = academicYear },
                        new BlockData { Type = "plain_text", Text = collectionPeriod }
                    }
                }
            };

            if (!string.IsNullOrWhiteSpace(yearToDatePayments) || !string.IsNullOrWhiteSpace(collectionPeriodPayments) || !string.IsNullOrWhiteSpace(numberOfLearners))
            {
                var optionalBlock = AddOptionalBlockFields(collectionPeriodPayments, yearToDatePayments, numberOfLearners);

                blocks.Add(optionalBlock);
            }


            return blocks;
        }

        private static Block AddOptionalBlockFields(string collectionPeriodPayments, string yearToDatePayments, string numberOfLearners)
        {
            var optionalBlock = new Block
            {
                Type = "section",
                Text = new BlockData
                {
                    Type = "mrkdwn",
                    Text = " "
                },
                Fields = new List<BlockData>()
            };

            if (!string.IsNullOrWhiteSpace(yearToDatePayments))
            {
                optionalBlock.Fields.Add(new BlockData { Type = "mrkdwn", Text = "*Previous Payments Year To Date*" });
            }

            if (!string.IsNullOrWhiteSpace(collectionPeriodPayments))
            {
                optionalBlock.Fields.Add(new BlockData { Type = "mrkdwn", Text = "*Collection Period Payments*" });
            }

            if (!string.IsNullOrWhiteSpace(yearToDatePayments))
            {
                var yearTodatePaymentsText = string.Empty;
                try
                {
                    var yearToDatePaymentsValue = Convert.ToDecimal(RemoveInvalidCharacters(yearToDatePayments));
                    yearTodatePaymentsText = yearToDatePaymentsValue.ToString("N2");
                }
                catch (FormatException)
                {
                    yearTodatePaymentsText = RemoveInvalidCharacters(yearToDatePayments);
                }
                optionalBlock.Fields.Add(new BlockData { Type = "plain_text", Text = $"£{yearTodatePaymentsText}" });
            }

            if (!string.IsNullOrWhiteSpace(collectionPeriodPayments))
            {
                var collectionPeriodPaymentsText = string.Empty;
                try
                {
                    var collectionPeriodPaymentsValue = Convert.ToDecimal(RemoveInvalidCharacters(collectionPeriodPayments));
                    collectionPeriodPaymentsText = collectionPeriodPaymentsValue.ToString("N2");
                }
                catch (FormatException)
                {
                    collectionPeriodPaymentsText = RemoveInvalidCharacters(collectionPeriodPayments);
                }
                optionalBlock.Fields.Add(new BlockData { Type = "plain_text", Text = $"£{collectionPeriodPaymentsText}" });
            }

            if (!string.IsNullOrEmpty(numberOfLearners))
            {
                optionalBlock.Fields.Add(new BlockData { Type = "mrkdwn", Text = "*In Learning*" });
                optionalBlock.Fields.Add(new BlockData { Type = "mrkdwn", Text = " " });
                optionalBlock.Fields.Add(new BlockData { Type = "plain_text", Text = RemoveInvalidCharacters(numberOfLearners) });
            }

            return optionalBlock;
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

        private static string RemoveInvalidCharacters(string text)
        {
            return text.Replace("\"", "");
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
                                              "n/a";

            string yearToDatePayments = customMeasurements.ContainsKey("YearToDatePaymentsTotal") ?
                                         customMeasurements["YearToDatePaymentsTotal"].ToString() :
                                            customMeasurements.ContainsKey("PaymentsYearToDateTotal") ?
                                            customMeasurements["PaymentsYearToDateTotal"].ToString() :
                                            "n/a";

            string numberOfLearners = customMeasurements.ContainsKey("InLearning") ?
                                       customMeasurements["InLearning"].ToString() :
                                       "n/a";

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