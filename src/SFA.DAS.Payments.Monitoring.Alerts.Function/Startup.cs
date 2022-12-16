using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(SFA.DAS.Monitoring.Alerts.Function.Startup))]


namespace SFA.DAS.Monitoring.Alerts.Function
{
    public class Startup : FunctionsStartup
    {
        private static int _numberOfRetries = 4;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddHttpClient("pollyClient").AddPolicyHandler(GetRetryPolicy());    
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
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
    }
}
