using System;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{
    public interface IFundingSourceEventMapper
    {
        ProviderPaymentEventModel Map(FundingSourcePaymentEvent message);
    }
    
    public class FundingSourceEventMapper: IFundingSourceEventMapper
    {
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        public FundingSourceEventMapper(IPaymentLogger  logger, IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ProviderPaymentEventModel Map(FundingSourcePaymentEvent message)
        {
            logger.LogVerbose($"Mapping funding source event of type: {message.GetType().Name} to ProviderPaymentEventModel.");
            return mapper.Map<ProviderPaymentEventModel>(message);
        }
    }
}