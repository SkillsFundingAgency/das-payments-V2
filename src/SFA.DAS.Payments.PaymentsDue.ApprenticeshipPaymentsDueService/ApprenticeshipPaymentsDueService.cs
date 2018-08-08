using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;
using SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService.Interfaces;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService
{
    [StatePersistence(StatePersistence.Volatile)]
    public class ApprenticeshipPaymentsDueService : Actor, IApprenticeshipPaymentsDueService
    {
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;
        private readonly string _apprenticeshipKey;

        public ApprenticeshipPaymentsDueService(ActorService actorService, ActorId actorId, IPaymentHistoryRepository paymentHistoryRepository) 
            : base(actorService, actorId)
        {
            _paymentHistoryRepository = paymentHistoryRepository;
            _apprenticeshipKey = actorId.GetStringId();
        }

        public async Task<IEnumerable<ICalculatedPaymentDueEvent>> HandlePayableEarning(IPayableEarningEvent earningEntity, CancellationToken cancellationToken)
        {
            // TODO: get apprenticeship
            var earning = Mapper.Map<IPayableEarningEvent, PayableEarning>(earningEntity);
            var apprenticeship = new Apprenticeship
            {
                // learner
                // course
                // price episodes
            };
        
            var paymentHistory = await _paymentHistoryRepository.GetPaymentHistory(_apprenticeshipKey, cancellationToken).ConfigureAwait(false);

            var paymentsDue = apprenticeship.CreatePaymentDue(new[] {earning}, paymentHistory);

            return paymentsDue.Select(Mapper.Map<PaymentDue, CalculatedPaymentDueEvent>);
        }
    }
}

