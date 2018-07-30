using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService.Interfaces;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService
{
    [StatePersistence(StatePersistence.Volatile)]
    internal class ApprenticeshipPaymentsDueService : Actor, IApprenticeshipPaymentsDueService
    {
        public ApprenticeshipPaymentsDueService(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        public Task HandlePayableEarning(IPayableEarningEvent earning, CancellationToken cancellationToken)
        {
            // TODO: create repositories and inject cache provider
            // TODO: get apprenticeship
            // TODO: get historical payments
            // TODO: calculate payments due (call apprenticeship domain method)
            // TODO: command to send payments due down the line
            return Task.FromResult(0);
        }
    }
}
