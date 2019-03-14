namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    using System.Threading.Tasks;
    using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
    using ESFA.DC.Serialization.Interfaces;
    using SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient;
    using System.Configuration;
    using ESFA.DC.CollectionsManagement.Models;

    public class CollectionManagementService : ICollectionManagementService
    {
        private readonly IBespokeHttpClient _httpClient;
        private string _baseUrl;
        private readonly IJsonSerializationService _serializationService;

        public CollectionManagementService(IBespokeHttpClient httpClient,
            IJsonSerializationService serializationService)
        {
            _httpClient = httpClient;
            _serializationService = serializationService;
        }

        public async Task<ReturnPeriodModel> GetCurrentPeriodAsync(string collectionName)
        {
            _baseUrl = ConfigurationManager.AppSettings["apiBaseUrl"];

            var data = await _httpClient.GetDataAsync($"{_baseUrl}/returns-calendar/{collectionName}/current");
            ReturnPeriodModel result = null;

            if (!string.IsNullOrEmpty(data))
            {
                var returnPeriod = _serializationService.Deserialize<ReturnPeriod>(data);
                result = new ReturnPeriodModel(returnPeriod.PeriodNumber);
            }

            return result;
        }
    }
}
