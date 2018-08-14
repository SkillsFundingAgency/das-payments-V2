using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using AutoMapper;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class RequiredPaymentsService : Actor, IRequiredPaymentsService
    {
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;
        private readonly string _apprenticeshipKey;
        private readonly IMapper _mapper;

        public RequiredPaymentsService(ActorService actorService, ActorId actorId, IPaymentHistoryRepository paymentHistoryRepository, IMapper mapper)
            : base(actorService, actorId)
        {
            _paymentHistoryRepository = paymentHistoryRepository;
            _mapper = mapper;
            _apprenticeshipKey = actorId.GetStringId();
        }

        public async Task<CalculatedPaymentDueEvent[]> HandlePayableEarning(PayableEarningEvent earningEntity, CancellationToken cancellationToken)
        {
            // TODO: get apprenticeship
            var earning = _mapper.Map<PayableEarningEvent, PayableEarning>(earningEntity);
            var apprenticeship = new Apprenticeship
            {
                // learner
                // course
                // price episodes
            };

            var paymentHistory = await _paymentHistoryRepository.GetPaymentHistory(_apprenticeshipKey, cancellationToken).ConfigureAwait(false);

            var paymentsDue = apprenticeship.CreatePaymentDue(new[] { earning }, paymentHistory);

            return paymentsDue.Select(_mapper.Map<PaymentDue, CalculatedPaymentDueEvent>).ToArray();
        }
    }
}
