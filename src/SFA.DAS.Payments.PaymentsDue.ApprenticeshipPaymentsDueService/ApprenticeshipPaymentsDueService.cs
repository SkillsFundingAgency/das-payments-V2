using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application.Repositories;
using SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService.Interfaces;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService
{
    [StatePersistence(StatePersistence.Volatile)]
    internal class ApprenticeshipPaymentsDueService : Actor, IApprenticeshipPaymentsDueService
    {
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;

        public ApprenticeshipPaymentsDueService(ActorService actorService, ActorId actorId, IPaymentHistoryRepository paymentHistoryRepository) 
            : base(actorService, actorId)
        {
            _paymentHistoryRepository = paymentHistoryRepository;
        }

        public async Task HandlePayableEarning(IPayableEarningEvent earningEntity, CancellationToken cancellationToken)
        {
            // TODO: get apprenticeship
            var earning = Mapper.Map<IPayableEarningEvent, PayableEarning>(earningEntity);
            var apprenticeship = new Apprenticeship
            {
                // learner
                // course
                // price episodes
            };
        
            var paymentHistory = await _paymentHistoryRepository.GetPaymentHistory(apprenticeship.Key, cancellationToken).ConfigureAwait(false);

            var paymentsDue = apprenticeship.CreatePaymentDue(new[] {earning}, paymentHistory);

            // TODO: command to send payments due down the line
        }
    }
}

