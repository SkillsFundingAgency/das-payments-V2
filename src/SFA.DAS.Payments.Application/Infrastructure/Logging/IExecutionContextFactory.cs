using ESFA.DC.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public interface IExecutionContextFactory
    {
        ExecutionContext GetExecutionContext();
    }
}