namespace SFA.DAS.Payments.AcceptanceTests.Services.BespokeHttpClient
{
    using System.Threading.Tasks;

    public interface IBespokeHttpClient
    {
        Task<string> SendDataAsync(string url, object job);

        Task<string> SendAsync(string url);

        Task<string> GetDataAsync(string url);
    }
}
