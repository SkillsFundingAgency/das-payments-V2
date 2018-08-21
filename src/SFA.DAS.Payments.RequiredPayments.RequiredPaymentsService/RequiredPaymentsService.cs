using System;
using Autofac;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.RequiredPayments.Messages.Entities;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;
        private readonly string _apprenticeshipKey;
        private readonly IMapper _mapper;
        private readonly IPaymentLogger _paymentLogger;
        private readonly ILifetimeScope _lifetimeScope;

        public RequiredPaymentsService(ActorService actorService,
                                        ActorId actorId,
                                        IPaymentHistoryRepository paymentHistoryRepository,
                                        IMapper mapper,
                                        IPaymentLogger paymentLogger,
                                        ILifetimeScope lifetimeScope) : base(actorService, actorId)
        {
            _paymentHistoryRepository = paymentHistoryRepository;
            _mapper = mapper;
            _apprenticeshipKey = actorId.GetStringId();
            _paymentLogger = paymentLogger;
            _lifetimeScope = lifetimeScope;
        }

        public async Task<CalculatedPaymentDueEvent[]> HandlePayableEarning(PayableEarningEvent earningEntity, CancellationToken cancellationToken)
        {
            using (var scope = _lifetimeScope.BeginLifetimeScope())
            {
                var executionContext = (ESFA.DC.Logging.ExecutionContext)_lifetimeScope.Resolve<ESFA.DC.Logging.Interfaces.IExecutionContext>();
                executionContext.JobId = earningEntity.JobId;

                _paymentLogger.LogInfo($"Handling Payable Earning for {earningEntity?.Ukprn} ");
                return new [] {
                    new CalculatedPaymentDueEvent
                    {
                        JobId = earningEntity.JobId, 
                        EventTime = DateTimeOffset.UtcNow,
                        PaymentDueEntity = new PaymentDueEntity()
                    }
                };
            };
        }
    }
}