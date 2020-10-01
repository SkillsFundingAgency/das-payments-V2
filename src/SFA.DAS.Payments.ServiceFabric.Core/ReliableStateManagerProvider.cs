using Microsoft.ServiceFabric.Data;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IReliableStateManagerProvider
    {
        IReliableStateManager Current { get; }
    }

    public class ReliableStateManagerProvider : IReliableStateManagerProvider
    {
        public IReliableStateManager Current { get; set; }
    }
}