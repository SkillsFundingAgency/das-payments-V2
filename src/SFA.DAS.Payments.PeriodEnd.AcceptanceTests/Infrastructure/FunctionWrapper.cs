using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.PeriodEnd.Model.SubmissionJobs;

namespace SFA.DAS.Payments.PeriodEnd.AcceptanceTests.Infrastructure
{
    public class FunctionWrapper
    {
        private readonly HttpClient httpClient;
        private readonly string baseUri;

        public FunctionWrapper(HttpClient httpClient)
        {
            baseUri = ConfigurationManager.AppSettings["FunctionUri"];
            this.httpClient = httpClient;
        }

        public async Task<SubmissionJobs> SuccessfulSubmissions(short academicYear, byte collectionPeriod)
        {
            var response = await httpClient.GetStringAsync($"{baseUri}?academicYear={academicYear}&collectionperiod={collectionPeriod}");
            var results = JsonConvert.DeserializeObject<SubmissionJobs>(response);

            return results;
        }
    }
}
