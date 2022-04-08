using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public interface ICommitmentsApiClient
    {
        Task<CommitmentStatsDto> GetStats(int lastNumberOfDays);
    }

    public class CommitmentsApiClient : ICommitmentsApiClient
    {
        private readonly ICommitmentApiConfiguration commitmentApiConfiguration;
        private readonly HttpClient httpClient;

        public CommitmentsApiClient(ICommitmentApiConfiguration commitmentApiConfiguration)
        {
            this.commitmentApiConfiguration = commitmentApiConfiguration;
            this.httpClient = new HttpClient { BaseAddress = new Uri(this.commitmentApiConfiguration.ApiBaseUrl) };
        }

        private async Task<AuthenticationResult> GetAuthenticationResult()
        {
            var authority = "https://login.microsoftonline.com/" + commitmentApiConfiguration.Tenant;

            var clientCredential = new ClientCredential(commitmentApiConfiguration.ClientId, commitmentApiConfiguration.ClientSecret);
            
            return await new AuthenticationContext(authority, true).AcquireTokenAsync(commitmentApiConfiguration.IdentifierUri, clientCredential);
        }

        public async Task<CommitmentStatsDto> GetStats(int lastNumberOfDays)
        {
            var authenticationResult = await GetAuthenticationResult();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

            var httpResponseMessage = await httpClient.GetAsync($"api/apprenticeshipstatistics/stats?{lastNumberOfDays}");

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<CommitmentStatsDto>(response);
        }
    }
}