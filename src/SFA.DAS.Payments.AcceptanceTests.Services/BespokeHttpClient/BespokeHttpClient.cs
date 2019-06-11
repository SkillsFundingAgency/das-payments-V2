using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using Polly;
using Polly.Registry;

namespace SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient
{
    public class BespokeHttpClient : IBespokeHttpClient
    {
        private readonly IJsonSerializationService jsonSerializationService;
        private readonly string urlBase;
        private readonly HttpClient httpClient = new HttpClient();
        private bool disposed = false;
        private readonly IAsyncPolicy pollyPolicy;

        public BespokeHttpClient(IJsonSerializationService jsonSerializationService, IReadOnlyPolicyRegistry<string> pollyRegistry)
        {
            pollyPolicy = pollyRegistry.Get<IAsyncPolicy>("HttpRetryPolicy");
            this.jsonSerializationService = jsonSerializationService;
            urlBase = ConfigurationManager.AppSettings["apiBaseUrl"];
        }

        public async Task<string> SendDataAsync(string url, object data)
        {
            var json = jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await pollyPolicy.ExecuteAsync(() => httpClient.PostAsync(FullUrl(url), content));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendAsync(string url)
        {
            var response = await pollyPolicy.ExecuteAsync(() => httpClient.PostAsync(FullUrl(url), null));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDataAsync(string url)
        {
            var response = await pollyPolicy.ExecuteAsync(() => httpClient.GetAsync(FullUrl(url)));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task DeleteAsync(string url)
        {
            var response = await pollyPolicy.ExecuteAsync(() => httpClient.DeleteAsync(FullUrl(url)));
            response.EnsureSuccessStatusCode();
        }

        private Uri FullUrl(string url) => new Uri($"{urlBase}/{url}");

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    httpClient.Dispose();
                }
            }

            disposed = true;
        }
    }
}
