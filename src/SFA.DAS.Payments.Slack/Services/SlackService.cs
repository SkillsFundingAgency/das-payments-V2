using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.Payments.Slack.Helpers;
using SFA.DAS.Payments.Slack.JsonHelpers;
using SFA.DAS.Payments.Slack.TypedClients;

namespace SFA.DAS.Payments.Slack.Services
{
    public class SlackService : ISlackService
    {
        private readonly IAppInsightsClient _appInsightsClient;
        private readonly ISlackAlertHelper _slackAlertHelper;
        private readonly ISlackClient _slackClient;
        private readonly IDynamicJsonDeserializer _deserializer;

        public SlackService(IDynamicJsonDeserializer deserializer,
                            ISlackAlertHelper slackAlertHelper,
                            ISlackClient slackClient,
                            IAppInsightsClient appInsightsClient)
        {
            _deserializer = deserializer;
            _slackAlertHelper = slackAlertHelper;
            _appInsightsClient = appInsightsClient;
            _slackClient = slackClient; 
        }

        public async Task PostSlackAlert(string appInsightsAlertPayload, string slackChannelUri)
        {
            dynamic alert = _deserializer.Deserialize(appInsightsAlertPayload);

            string searchResultApiUrl = alert.data.alertContext.condition.allOf[0].linkToSearchResultsAPI;
            alert.data.alertContext.SearchResults = await _appInsightsClient.GetSearchResultsAsync(searchResultApiUrl);

            var severity = alert.data.essentials.severity;
            string alertEmoji = _slackAlertHelper.GetEmoji(severity);

            var appInsightsSearchResultsUiLink = alert.data.alertContext.condition.allOf[0].linkToSearchResultsUI;

            foreach (var table in alert.data.alertContext.SearchResults.tables)
            {
                foreach (var row in table.rows)
                {
                    var customDimensions = JsonSerializer.Deserialize<Dictionary<string, string>>(row[3]);
                    var customMeasurements = JsonSerializer.Deserialize<Dictionary<string, double>>(row[4]);
                    DateTime timestamp = DateTime.Parse(row[0]);
                    Dictionary<string,string> alertVariables = _slackAlertHelper.ExtractAlertVariables(customMeasurements, customDimensions, timestamp);
                    string alertDescription = alert.data.essentials.description;
                    await PostSlackAlert(alertVariables, slackChannelUri, alertDescription, alertEmoji, appInsightsSearchResultsUiLink, timestamp);
                }
            }
        }

        public async Task PostSlackAlert(Dictionary<string, string> alertVariables,
                                          string slackChannelUri,
                                          string alertDescription,
                                          string alertEmoji,
                                          DateTime timestamp,
                                          string alertTitle) 
        {
            var slackPayload = new
            {
                text = alertTitle,
                blocks = _slackAlertHelper.BuildSlackPayload(alertEmoji,
                    timestamp,
                    alertVariables["JobId"],
                    alertVariables["AcademicYear"],
                    alertVariables["CollectionPeriod"],
                    alertTitle)
            };

            await _slackClient.PostAsJsonAsync(slackChannelUri, slackPayload);
        }

        private async Task PostSlackAlert(Dictionary<string, string> alertVariables,
                                          string slackChannelUri,
                                          string alertDescription,
                                          string alertEmoji,
                                          string appInsightsSearchResultsUiLink,
                                          DateTime timestamp)
        {
            string alertTitle = _slackAlertHelper.GetSlackAlertTitle(alertDescription, alertVariables);
            
            var slackPayload = new
            {
                text = alertTitle,
                blocks = _slackAlertHelper.BuildSlackPayload(alertEmoji,
                                       timestamp,
                                       alertVariables["JobId"],
                                       alertVariables["AcademicYear"],
                                       alertVariables["CollectionPeriod"],
                                       alertTitle,
                                       appInsightsSearchResultsUiLink)
            };

            await _slackClient.PostAsJsonAsync(slackChannelUri, slackPayload);
        }
    }
}