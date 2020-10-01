﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Messages;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class CompletionPaymentService : ICompletionPaymentService
    {
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        private readonly IProviderPaymentsRepository repository;

        public CompletionPaymentService(IPaymentLogger  logger, IMapper mapper, IProviderPaymentsRepository repository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<IList<RecordedAct1CompletionPayment>> GetAct1CompletionPaymentEvents(ProcessMonthEndAct1CompletionPaymentCommand message)
        {
            logger.LogInfo($"now building Act1 Completion Payment Events for collection period: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}, job: {message.JobId}");
            
            var payments = await repository.GetMonthEndAct1CompletionPayments(message.CollectionPeriod).ConfigureAwait(false);

            var recordedAct1CompletionPaymentEvents = mapper.Map<IList<RecordedAct1CompletionPayment>>(payments);

            logger.LogInfo($"Finished creating the provider period end events for collection period: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}, job: {message.JobId}");
            
            return recordedAct1CompletionPaymentEvents;
        }
    }
}