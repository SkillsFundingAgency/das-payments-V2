using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Payments.Monitoring.Alerts.Function;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Services;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

[assembly: FunctionsStartup(typeof(SFA.DAS.Monitoring.Alerts.Function.Startup))]
namespace SFA.DAS.Monitoring.Alerts.Function
{
    public class Startup : FunctionsStartup
    {
        private static readonly int _numberOfRetries = 4;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            #if DEBUG
            SetupDevelopmentEnvironmentVariables(builder);
            #endif

            builder.Services.AddLogging();

            AddAppInsightsClient(builder);

            builder.Services
                   .AddHttpClient<ISlackClient, SlackClient>(x =>
                   {
                       x.BaseAddress = new Uri(GetEnvironmentVariable("SlackBaseUrl"));
                   });

            builder.Services.AddTransient<IDynamicJsonDeserializer, DynamicJsonDeserializer>();
            builder.Services.AddTransient<ISlackAlertHelper, SlackAlertHelper>();
            builder.Services.AddTransient<ISlackService, SlackService>();
        }

        private static void AddAppInsightsClient(IFunctionsHostBuilder builder)
        {
            builder.Services
                   .AddHttpClient<IAppInsightsClient, AppInsightsClient>(x =>
                   {
                       var appInsightsAPIKeyHeader = GetEnvironmentVariable("AppInsightsAuthHeader");
                       var appInsightsAPIKeyValue = GetEnvironmentVariable("AppInsightsAuthValue");

                       x.DefaultRequestHeaders.Accept.Clear();
                       x.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                       x.DefaultRequestHeaders.Add(appInsightsAPIKeyHeader, appInsightsAPIKeyValue);
                   })
                   .AddPolicyHandler(GetDefaultRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetDefaultRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                    _numberOfRetries, 
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        var log = context.GetLogger();
                        log?.LogInformation($"Request failed with status code {outcome.Result.StatusCode} delaying for {timespan.TotalMilliseconds} milliseconds then retry {retryAttempt}");
                    });
        }

        private static string GetEnvironmentVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process);
        }
        
        private void SetupDevelopmentEnvironmentVariables(IFunctionsHostBuilder builder)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}