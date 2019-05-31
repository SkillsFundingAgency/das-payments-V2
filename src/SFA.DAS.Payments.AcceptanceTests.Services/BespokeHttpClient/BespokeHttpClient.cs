namespace SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using ESFA.DC.Serialization.Interfaces;
    using Newtonsoft.Json;
    using Polly;
    using Polly.Registry;

    public class BespokeHttpClient : IBespokeHttpClient
    {
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly string _urlBase;
        private readonly HttpClient _httpClient = new HttpClient();
        private bool _disposed = false;
        private readonly IAsyncPolicy _pollyPolicy;

        public BespokeHttpClient(IJsonSerializationService jsonSerializationService, IReadOnlyPolicyRegistry<string> pollyRegistry)
        {
            _pollyPolicy = pollyRegistry.Get<IAsyncPolicy>("HttpRetryPolicy");
            _jsonSerializationService = jsonSerializationService;
            _urlBase = ConfigurationManager.AppSettings["apiBaseUrl"];
        }

        public async Task<string> SendDataAsync(string url, object data)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _pollyPolicy.ExecuteAsync(() => _httpClient.PostAsync(FullUrl(url), content));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> SendAsync(string url)
        {
            var response = await _pollyPolicy.ExecuteAsync(() => _httpClient.PostAsync(FullUrl(url), null));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDataAsync(string url)
        {
            var response = await _pollyPolicy.ExecuteAsync(() => _httpClient.GetAsync(FullUrl(url)));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private Uri FullUrl(string url) => new Uri($"{_urlBase}/{url}");

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
