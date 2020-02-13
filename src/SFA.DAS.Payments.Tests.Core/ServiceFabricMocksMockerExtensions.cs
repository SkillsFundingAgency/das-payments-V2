using Autofac.Extras.Moq;
using Moq;
using ServiceFabric.Mocks;
using SFA.DAS.Payments.ServiceFabric.Core;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.Tests.Core
{
    public static class ServiceFabricMocksMockerExtensions
    {
        public static MockReliableStateManager SetupReliableStateManagerProvision(this AutoMock mocker)
        {
            var reliableStateManager = new MockReliableStateManager();
            var reliableStateManagerProvider = new Mock<IReliableStateManagerProvider>();
            reliableStateManagerProvider.Setup(x => x.Current).Returns(reliableStateManager);
            mocker.Provide(reliableStateManagerProvider.Object);
            return reliableStateManager;
        }

        public static MockTransaction SetupTransactionProvision(this AutoMock mocker, MockReliableStateManager reliableStateManager, long transactionId = 1)
        {
            var transaction = new MockTransaction(reliableStateManager, transactionId);
            mocker.Provide(transaction);
            return transaction;
        }

        public static void SetupReliableStateManagerTransactionProvision(this AutoMock mocker, MockTransaction transaction)
        {
            var reliableStateManagerTransactionProvider = new Mock<IReliableStateManagerTransactionProvider>();
            reliableStateManagerTransactionProvider.Setup(x => x.Current).Returns(transaction);
            mocker.Provide(reliableStateManagerTransactionProvider.Object);
        }
    }
}
