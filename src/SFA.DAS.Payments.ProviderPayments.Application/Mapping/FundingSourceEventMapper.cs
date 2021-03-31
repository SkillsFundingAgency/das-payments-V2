using System;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{
    public interface IFundingSourceEventMapper
    {
        PaymentModel Map(FundingSourcePaymentEvent message);
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

        public PaymentModel Map(FundingSourcePaymentEvent message)
        {
            try
            {
                logger.LogVerbose($"Mapping funding source event of type: {message.GetType().Name} to PaymentModel.");
                return mapper.Map<PaymentModel>(message);

            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed mapping message: {message.GetType()}. Error: {e.Message}.  Exception: {e}");
                throw;
            }
        }
    }
}