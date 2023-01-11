using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Net.Http;
using Azure.Core;

namespace SFA.DAS.Monitoring.Alerts.Function
{
    public class SendSlackAlert
    {
        private readonly IHttpClientFactory _factory;

        public SendSlackAlert(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        [FunctionName("HttpTrigger1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get","post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"Request: {requestBody}.");
            dynamic alert = JsonConvert.DeserializeObject<ExpandoObject>(requestBody);
            log.LogInformation($"Alert name: {alert.data?.essentials?.alertRule}");

            HttpClient apiInsightsClient = GetAppInsightsClient();
            alert.data.alertContext.SearchResults = await GetAppInsightsSearchResults(alert, apiInsightsClient);

            string alertEmoji = GetEmoji(alert);
            var appInsightsSearchResultsUiLink = alert.data.alertContext.condition.allOf[0].linkToSearchResultsUI;

            HttpClient slackApiClient = GetSlackClient();

            foreach (var table in alert.data.alertContext.SearchResults.tables)
            {
                Console.WriteLine($"{table.rows}");
                foreach (var row in table.rows)
                {
                    await PostSlackAlert(slackApiClient, alert, alertEmoji, row, appInsightsSearchResultsUiLink);
                }
            }

            return new OkObjectResult("");
        }

        private static async Task<dynamic> GetAppInsightsSearchResults(dynamic alert, HttpClient client)
        {
            var appInsightsSearchResultsUri = alert.data.alertContext.condition.allOf[0].linkToSearchResultsAPI;


            var appInsightsApiResultJson = await client.GetStringAsync(new Uri(appInsightsSearchResultsUri));
            dynamic appInsightsResult = JsonConvert.DeserializeObject<ExpandoObject>(appInsightsApiResultJson);

            return appInsightsResult;
        }

        private HttpClient GetSlackClient()
        {
            var slackApiClient = _factory.CreateClient("pollyClient");
            slackApiClient.BaseAddress = new Uri(GetEnvironmentVariable("SlackBaseUrl"));
            return slackApiClient;
        }

        private HttpClient GetAppInsightsClient()
        {
            var apiInsightsClient = _factory.CreateClient("pollyClient");
            var appInsightsAPIKeyHeader = GetEnvironmentVariable("AppInsightsAuthHeader");
            var appInsightsAPIKeyValue = GetEnvironmentVariable("AppInsightsAuthValue");

            apiInsightsClient.DefaultRequestHeaders.Accept.Clear();
            apiInsightsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            apiInsightsClient.DefaultRequestHeaders.Add(appInsightsAPIKeyHeader, appInsightsAPIKeyValue);
            return apiInsightsClient;
        }

        private static async Task PostSlackAlert(HttpClient httpClient, dynamic alert, string alertEmoji, dynamic row, string appInsightsSearchResultsUiLink)
        {
            var customDimensions = JsonConvert.DeserializeObject<Dictionary<string, string>>(row[3]);
            var customMeasurements = JsonConvert.DeserializeObject<Dictionary<string, double>>(row[4]);
            DateTime timestamp = row[0];

            var alertVariables = ExtractAlertVariables(customMeasurements, customDimensions, timestamp);

            string alertTitle = alert.data.essentials.description;
            string alertText = alert.data.essentials.description;

            foreach (var alertVariable in alertVariables)
            {
                alertTitle = alertTitle.Replace("{" + alertVariable.Key + "}", alertVariable.Value);
                alertText = alertText.Replace("{" + alertVariable.Key + "}", "*" + alertVariable.Value + "*");
            }

            var slackPayload = new
            {
                text = alertTitle,
                blocks = ExtractFields(alertEmoji,
                                       timestamp,
                                       alertVariables["JobId"],
                                       alertVariables["AcademicYear"],
                                       alertVariables["CollectionPeriod"],
                                       alertTitle,
                                       appInsightsSearchResultsUiLink)
            };

            var payloadText = JsonConvert.SerializeObject(slackPayload);

            var slackChannelResource = GetEnvironmentVariable("SlackChannelUri");

            var response = await httpClient.PostAsJsonAsync(slackChannelResource, slackPayload);

            var x = await response.Content.ReadAsStringAsync();
        }

        private static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process);
        }

        private static List<object> ExtractFields(string alertEmoji, DateTime timestamp, string jobId, string academicYear, string collectionPeriod, string alertTitle, string appInsightsSearchResultsUiLink)
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
                        new { type = "plain_text", text = jobId.ToString() },
                        new { type = "mrkdwn", text = "*Academic Year*" },
                        new { type = "mrkdwn", text = "*Collection Period*" },
                        new { type = "plain_text", text = academicYear.ToString() },
                        new { type = "plain_text", text = collectionPeriod.ToString() },
                    }
                }
            };
        }

        private static string GetEmoji(dynamic alert)
        {
            return alert.data.essentials.severity switch
            {
                "Sev0" or "Sev1" => ":alert:",
                "Sev2" => ":warning:",
                "Sev3" => ":information_source:",
                _ => string.Empty,
            };
        }

        private static Dictionary<string, string> ExtractAlertVariables(dynamic customMeasurements, dynamic customDimensions, DateTime timestamp)
        {
            var percentage = customMeasurements.ContainsKey("Percentage") ? customMeasurements["Percentage"] : 0;
            var duration = customMeasurements.ContainsKey("Duration") ? customMeasurements["Duration"] : 0;
            var ukprn = customDimensions.ContainsKey("Ukprn") ? customDimensions["Ukprn"] : string.Empty;
            var jobId = customDimensions.ContainsKey("JobId") ? customDimensions["JobId"] : string.Empty;
            var academicYear = customDimensions.ContainsKey("AcademicYear") ? customDimensions["AcademicYear"] : string.Empty;
            var collectionPeriod = customDimensions.ContainsKey("CollectionPeriod") ? customDimensions["CollectionPeriod"] : string.Empty;

            dynamic dcEarningsTotal = customMeasurements.ContainsKey("DcEarningsTotal") ?
                                      customMeasurements["DcEarningsTotal"].ToString() :
                                        customMeasurements.ContainsKey("EarningsDCTotal") ?
                                        customMeasurements["EarningsDCTotal"].ToString() :
                                        string.Empty;

            dynamic dasEarningsTotal = customMeasurements.ContainsKey("DasEarningsTotal") ?
                                       customMeasurements["DasEarningsTotal"].ToString() :
                                       string.Empty;

            dynamic adjustedDataLockedEarnings = customMeasurements.ContainsKey("DataLockedEarningsAmount") ?
                                                 customMeasurements["DataLockedEarningsAmount"].ToString() :
                                                    customMeasurements.ContainsKey("DataLockedEarnings") ?
                                                    customMeasurements["DataLockedEarnings"].ToString() :
                                                    string.Empty;

            dynamic differenceTotal = customMeasurements.ContainsKey("DifferenceTotal") ?
                                      customMeasurements["DifferenceTotal"].ToString() :
                                      string.Empty;

            dynamic heldBackCompletionPayments = customMeasurements.ContainsKey("HeldBackCompletionPayments") ?
                                                 customMeasurements["HeldBackCompletionPayments"].ToString() :
                                                 string.Empty;

            dynamic requiredPayments = customMeasurements.ContainsKey("RequiredPaymentsTotal") ?
                                       customMeasurements["RequiredPaymentsTotal"].ToString() :
                                       string.Empty;

            dynamic colectionPeriodPayments = customMeasurements.ContainsKey("PaymentsTotal") ?
                                              customMeasurements["PaymentsTotal"].ToString() :
                                              string.Empty;

            dynamic yearToDatePayments = customMeasurements.ContainsKey("YearToDatePaymentsTotal") ?
                                         customMeasurements["YearToDatePaymentsTotal"].ToString() :
                                            customMeasurements.ContainsKey("PaymentsYearToDateTotal") ?
                                            customMeasurements["PaymentsYearToDateTotal"].ToString() :
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
                { "CollectionPeriodPayments", colectionPeriodPayments },
                { "YearToDatePayments", yearToDatePayments }
            };
        }
    }
}