﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Models;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public class SlackService : ISlackService
    {
        private readonly IAppInsightsClient _appInsightsClient;
        private readonly ISlackAlertHelper _slackAlertHelper;
        private readonly ISlackClient _slackClient;
        private readonly IDynamicJsonDeserializer _deserializer;
        private ILogger _logger;

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

        public async Task PostSlackAlert(ILogger logger, string appInsightsAlertPayload, string slackChannelUri)
        {
            _logger = logger;

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

        private async Task PostSlackAlert(Dictionary<string, string> alertVariables,
                                          string slackChannelUri,
                                          string alertDescription,
                                          string alertEmoji,
                                          string appInsightsSearchResultsUiLink,
                                          DateTime timestamp)
        {
            string alertTitle = _slackAlertHelper.GetSlackAlertTitle(alertDescription, alertVariables);
            
            var slackPayload = new SlackPayload
            {
                Blocks = _slackAlertHelper.BuildSlackPayload(alertEmoji,
                                       timestamp,
                                       alertVariables["JobId"],
                                       alertVariables["AcademicYear"],
                                       alertVariables["CollectionPeriod"],
                                       alertVariables["CollectionPeriodPayments"],
                                       alertVariables["YearToDatePayments"],
                                       alertVariables["NumberOfLearners"],
                                       alertVariables["AccountedForPayments"],
                                       alertTitle,
                                       appInsightsSearchResultsUiLink)
            };
            
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var jsonData = JsonSerializer.Serialize(slackPayload, serializeOptions);

            _logger.LogInformation($"JSON payload sending to Slack API: {jsonData} ");

            await _slackClient.PostAsJsonAsync(slackChannelUri, jsonData);
        }
    }
}