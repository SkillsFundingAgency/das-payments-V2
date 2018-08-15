using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IEndpointCommunicationListener<T> : ICommunicationListener
    {
    }
}