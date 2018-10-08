using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using System.Collections.Generic;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Services
{
    public class ApprenticeshipContractType2EarningEventsService : IEarningEventsProcessingService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IEarningEventMapper mapper;

        public ApprenticeshipContractType2EarningEventsService(IPaymentLogger paymentLogger, IEarningEventMapper mapper)
        {
            this.paymentLogger = paymentLogger;
            this.mapper = mapper;
        }

        public IEnumerable<ApprenticeshipContractType2EarningEvent> GetEarningEvents(FM36Global fm36Output)
        {
            paymentLogger.LogInfo("Processing FM36Global event");

            return mapper.MapEarningEvent(fm36Output);
        }
    }
}
