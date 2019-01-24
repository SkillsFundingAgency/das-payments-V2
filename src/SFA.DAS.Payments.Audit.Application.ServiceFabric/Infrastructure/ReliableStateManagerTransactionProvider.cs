using Microsoft.ServiceFabric.Data;

namespace SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure
{
    public interface IReliableStateManagerTransactionProvider
    {
        ITransaction Current { get; }
    }

    public class ReliableStateManagerTransactionProvider: IReliableStateManagerTransactionProvider
    {
        public ITransaction Current { get; internal set; }
    }
}