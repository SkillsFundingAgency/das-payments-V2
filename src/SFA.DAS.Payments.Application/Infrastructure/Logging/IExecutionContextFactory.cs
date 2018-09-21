using ESFA.DC.Logging.Interfaces;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public interface IExecutionContextFactory
    {
        IExecutionContext GetExecutionContext();
    }
}