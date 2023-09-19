using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public interface IPeriodEndArchiverClient
    {
        Task StartArchive();
        Task<string> ArchiveStatus();
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
        private readonly ITelemetry telemetry;
        private readonly Uri functionAddressUri;

        public PeriodEndArchiverClient(string authCode, string functionAddress)
        {
            this.authCode = authCode;
            this.telemetry = telemetry;
            functionAddressUri = new Uri(functionAddress);
        }

        public async Task StartArchive()
        {
            await new HttpClient { Timeout = TimeSpan.FromSeconds(270) }.PostAsync(BuildUriFromParameters(), null);
        }

        public async Task<string> ArchiveStatus()
        {
            var result = await new HttpClient { Timeout = TimeSpan.FromSeconds(270) }.GetAsync(BuildUriFromParameters());

            if (!result.IsSuccessStatusCode) return "Failed";

            var content = await result.Content.ReadAsStringAsync();

            var periodEndArchiverStatusSummary = JsonConvert.DeserializeObject<PeriodEndArchiverStatusSummary>(content);
            return periodEndArchiverStatusSummary.EntityState.Status;
        }

        private string BuildUriFromParameters()
        {
            return $"{new Uri(functionAddressUri, "/orchestrators/PeriodEndArchiveOrchestrator")}";
        }

    }
}