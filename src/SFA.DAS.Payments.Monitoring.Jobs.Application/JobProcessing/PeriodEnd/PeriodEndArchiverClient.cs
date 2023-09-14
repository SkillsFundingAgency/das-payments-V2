using Newtonsoft.Json;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndArchiverClient
    {
        Task<bool> ArchiveStatus();
    }

    public class PeriodEndArchiverClient : IPeriodEndArchiverClient
    {
        private readonly string authCode;
        private readonly Uri functionAddressUri;

        public PeriodEndArchiverClient(string authCode, string functionAddress)
        {
            this.authCode = authCode;
            functionAddressUri = new Uri(functionAddress);
        }

        public async Task<bool> ArchiveStatus()
        {
            var result = await new HttpClient { Timeout = TimeSpan.FromSeconds(270) }.GetAsync(BuildUriFromParameters());

            if (!result.IsSuccessStatusCode) return false;

            var content = await result.Content.ReadAsStringAsync();
            
            // TODO: Talk to Liam about what's coming back to see if we need to parse anything
            return true;
        }

        private string BuildUriFromParameters()
        {
            return $"{new Uri(functionAddressUri, "/orchestrators/PeriodEndArchiveOrchestrator")}";
        }
    }
}