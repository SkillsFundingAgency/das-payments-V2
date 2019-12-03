using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IManageReceivedDataLockEvent
    {
        Task ProcessDataLockEvent(DataLockEvent dataLockEvent);
    }
    
    public class ManageReceivedDataLockEvent: IManageReceivedDataLockEvent
    {
        private readonly IReceivedDataLockEventStore receivedDataLockEventStore;

        public ManageReceivedDataLockEvent(IReceivedDataLockEventStore  receivedDataLockEventStore)
        {
            this.receivedDataLockEventStore = receivedDataLockEventStore;
        }

        public async Task ProcessDataLockEvent(DataLockEvent dataLockEvent)
        {
            if(dataLockEvent is FunctionalSkillDataLockEvent) return;
            
            if (dataLockEvent is EarningFailedDataLockMatching)
            {
                var hasNoApprenticeship = dataLockEvent
                    .OnProgrammeEarnings
                    .SelectMany(o => o.Periods)
                    .SelectMany(p => p.DataLockFailures)
                    .All(d => !d.ApprenticeshipId.HasValue);

                if(hasNoApprenticeship) return;
            }

            if (dataLockEvent is PayableEarningEvent)
            {
                var hasNoApprenticeship = dataLockEvent
                    .OnProgrammeEarnings
                    .SelectMany(o => o.Periods)
                    .All(p => !p.ApprenticeshipId.HasValue);
                if (hasNoApprenticeship) return;
            }
            
            await receivedDataLockEventStore.Add(new ReceivedDataLockEvent
            {
                JobId = dataLockEvent.JobId,
                Ukprn = dataLockEvent.Ukprn,
                Message = dataLockEvent.ToJson(),
                MessageType = dataLockEvent.GetType().AssemblyQualifiedName
            });
        }
    }
}
