﻿using ESFA.DC.Logging.Interfaces;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService.Handlers
{
    public class CalculatedRequiredCoInvestedAmountEventHandler : IHandleMessages<CalculatedRequiredCoInvestedAmount>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly ICoInvestedFundingSourceService contractType2RequiredPaymentService;
        private readonly IExecutionContext executionContext;

        public CalculatedRequiredCoInvestedAmountEventHandler(IPaymentLogger paymentLogger,
                                                                      ICoInvestedFundingSourceService contractType2RequiredPaymentService,
                                                                      IExecutionContext executionContext)
        {
            this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
            this.contractType2RequiredPaymentService = contractType2RequiredPaymentService ?? throw new ArgumentNullException(nameof(contractType2RequiredPaymentService));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public async Task Handle(CalculatedRequiredCoInvestedAmount message, IMessageHandlerContext context)
        {
            paymentLogger.LogInfo($"Processing Required Payment Service event for Message Id : {context.MessageId}");
            var currentExecutionContext = (ESFA.DC.Logging.ExecutionContext)executionContext;
            currentExecutionContext.JobId = message.JobId.ToString();

            var payments = contractType2RequiredPaymentService.GetFundedPayments(message);
            foreach (var recordablePaymentEvent in payments)
            {
                try
                {
                    await context.Publish(recordablePaymentEvent);
                    paymentLogger.LogInfo($"Successfully published CoInvestedPayment of Type  {recordablePaymentEvent.GetType().Name}");
                }
                catch (Exception ex)
                {
                    paymentLogger.LogError($"Error publishing the event: RecordablePaymentEvent", ex);
                    throw;
                }
            }

            paymentLogger.LogInfo($"Successfully processed NonLevyFunded Service event for Job Id {message.JobId}");

        }
    }
}