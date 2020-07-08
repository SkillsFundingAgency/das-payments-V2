using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.PeriodEnd.Messages.Events
{
    public class PeriodEndRequestReportsEvent : PeriodEndEvent
    {  
        private readonly IPaymentLogger logger;
        public PeriodEndRequestReportsEvent( IPaymentLogger logger;
        {
            
        }
    }
}