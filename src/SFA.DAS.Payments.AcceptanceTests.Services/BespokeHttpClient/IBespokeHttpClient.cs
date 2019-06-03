using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient
{
    public interface IBespokeHttpClient
    {
        Task<string> SendDataAsync(string url, object job);

        Task<string> SendAsync(string url);

        Task<string> GetDataAsync(string url);

        Task DeleteAsync(string url);
    }
}
