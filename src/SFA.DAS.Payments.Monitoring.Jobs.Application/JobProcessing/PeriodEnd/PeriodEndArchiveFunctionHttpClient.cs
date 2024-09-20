using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndArchiveFunctionHttpClient
    {
        Task<HttpResponseMessage> GetArchiveFunctionStatus(long jobId);
    }

    public class PeriodEndArchiveFunctionHttpClient : IPeriodEndArchiveFunctionHttpClient
    {
        private IPeriodEndArchiveConfiguration _configuration;
        private ITelemetry _telemetry;

        public PeriodEndArchiveFunctionHttpClient(IPeriodEndArchiveConfiguration configuration, ITelemetry telemetry)
        {
            _configuration = configuration;
            _telemetry = telemetry;
        }

        public async Task<HttpResponseMessage> GetArchiveFunctionStatus(long jobId)
        {
            var param = new Dictionary<string, string>
            {
                { "jobId", jobId.ToString() }
            };
            var uri = new Uri(QueryHelpers.AddQueryString(_configuration.ArchiveFunctionUrl, param)).ToString();

            _telemetry.TrackEvent(
                $"PeriodEndArchiveStatusService: Checking current archiving status for jobId ${jobId}, Url: {uri}");

            return await new HttpClient { Timeout = TimeSpan.FromSeconds(_configuration.ArchiveTimeout) }.GetAsync(
                    $"{uri}");
        }
    }
}
