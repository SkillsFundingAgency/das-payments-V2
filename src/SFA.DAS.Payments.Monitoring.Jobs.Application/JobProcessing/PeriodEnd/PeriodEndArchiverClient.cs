using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndArchiverClient
    {
        Task<bool> ArchiveStatus();
    }

    public class EntityState
    {
        public string JobId { get; set; }
        public string Status { get; set; }
    }

    public class PeriodEndArchiverStatusSummary
    {
        public bool EntityExists { get; set; }
        public EntityState EntityState { get; set; }
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

            var periodEndArchiverStatusSummary = JsonConvert.DeserializeObject<PeriodEndArchiverStatusSummary>(content);
            return periodEndArchiverStatusSummary.EntityState.Status == "Succeeded";
        }

        private string BuildUriFromParameters()
        {
            return $"{new Uri(functionAddressUri, "/orchestrators/PeriodEndArchiveOrchestrator")}";
        }
    }
}