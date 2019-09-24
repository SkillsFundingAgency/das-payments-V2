using System.Transactions;

namespace SFA.DAS.Payments.Application.Data
{
    public static class TransactionScopeFactory
    {
        public static TransactionScope CreateWriteOnlyTransaction()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted,
            }, TransactionScopeAsyncFlowOption.Enabled);
        }

        public static TransactionScope CreateRepeatableReadTransaction()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.RepeatableRead,
            }, TransactionScopeAsyncFlowOption.Enabled);
        }

        public static TransactionScope CreateSerialisableTransaction()
        {
            return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
            }, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
