using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IStatefulEndpointCommunicationListener : ICommunicationListener
    {
        Task RunAsync();
    }
}