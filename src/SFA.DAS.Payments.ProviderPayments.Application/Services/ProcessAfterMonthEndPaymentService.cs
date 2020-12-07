using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProcessAfterMonthEndPaymentService
    {
        Task<ProviderPaymentEvent> GetPaymentEvent(FundingSourcePaymentEvent message);
    }

    public class ProcessAfterMonthEndPaymentService : IProcessAfterMonthEndPaymentService
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IMapper mapper;
        private readonly IProviderPeriodEndService providerPeriodEndService;

        public ProcessAfterMonthEndPaymentService(IPaymentLogger paymentLogger, IMapper mapper, IProviderPeriodEndService providerPeriodEndService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.mapper = mapper;
            this.providerPeriodEndService = providerPeriodEndService;
        }

        public async Task<ProviderPaymentEvent> GetPaymentEvent(FundingSourcePaymentEvent message)
        {
            var isMonthEnd = await providerPeriodEndService
                .IsMonthEndStarted(message.Ukprn, message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period)
                .ConfigureAwait(false);

            if (!isMonthEnd) return null;

            paymentLogger.LogVerbose($"Processing Month End for {message.GetType().Name} Ukprn {message.Ukprn} - AcademicYear {message.CollectionPeriod.AcademicYear} - Period {message.CollectionPeriod.Period}");

            var monthEndJobId = await providerPeriodEndService.GetMonthEndJobId(message.Ukprn,
                    message.CollectionPeriod.AcademicYear,
                    message.CollectionPeriod.Period)
                .ConfigureAwait(false);

            var payment = MapToProviderPaymentEvent(message, monthEndJobId, message.EventId);

            return payment;
        }

        private ProviderPaymentEvent MapToProviderPaymentEvent(FundingSourcePaymentEvent fundingSourcePaymentEvent, long monthEndJobId, Guid eventId)
        {
            paymentLogger.LogVerbose($"Mapping funding source payment: {fundingSourcePaymentEvent.ToDebug()}, funding source: {fundingSourcePaymentEvent.FundingSourceType:G}");
            var providerPayment = mapper.Map<ProviderPaymentEvent>(fundingSourcePaymentEvent);
            providerPayment.JobId = monthEndJobId;
            providerPayment.EventId = eventId;
            paymentLogger.LogDebug($"Finished mapping payment. Id: {providerPayment.EventId}");
            return providerPayment;
        }
    }
}
