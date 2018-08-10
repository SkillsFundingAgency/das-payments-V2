using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace SFA.DAS.Payment.ServiceFabric.Core
{
    public interface IEndpointCommunicationListener<T> : ICommunicationListener
    {
    }
}