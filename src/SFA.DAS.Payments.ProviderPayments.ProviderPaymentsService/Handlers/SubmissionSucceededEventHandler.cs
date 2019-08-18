using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class SubmissionSucceededEventHandler:IHandleMessages<SubmissionSucceededEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IProviderPaymentsService paymentsService;
        private readonly IMapper mapper;
        private readonly IEarningsJobClient earningsJobClient;
        private readonly IProcessAfterMonthEndPaymentService afterMonthEndPaymentService;

        public SubmissionSucceededEventHandler(IPaymentLogger paymentLogger,
            IProviderPaymentsService paymentsService,
            IMapper mapper,
            IMonthEndService monthEndService,
            IEarningsJobClient earningsJobClient,
            IProcessAfterMonthEndPaymentService afterMonthEndPaymentService)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.paymentsService = paymentsService ?? throw new ArgumentNullException(nameof(paymentsService));
            this.mapper = mapper;
            this.earningsJobClient = earningsJobClient;
            this.afterMonthEndPaymentService = afterMonthEndPaymentService;
        }

        public async Task Handle(SubmissionSucceededEvent message, IMessageHandlerContext context)
        {
            try
            {
                paymentLogger.LogDebug($"Processing Submission Succeeded Event for Message Id : {context.MessageId}");
              
            }
            catch (Exception ex)
            {
                throw;
            }

            paymentLogger.LogDebug($"Finished processing Submission Succeeded Event for Message Id : {context.MessageId}. ");
        }
    }
}