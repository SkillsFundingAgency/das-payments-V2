using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface IDeferredApprovalsEventRepository
    {
        Task<List<DeferredApprovalsEventEntity>> GetDeferredEvents(CancellationToken cancellationToken);
        Task<bool> DeferredEventsExist(CancellationToken cancellationToken);
        Task StoreDeferredEvent(DeferredApprovalsEventEntity deferredApprovalsEvent, CancellationToken cancellationToken);
        Task DeleteDeferredEvents(List<long> eventIds, CancellationToken cancellationToken);
    }

    public class DeferredApprovalsEventRepository : IDeferredApprovalsEventRepository
    {
        private readonly IPaymentsDataContext dataContext;
        private static readonly JsonSerializerSettings SerialisationSettings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented};

        public DeferredApprovalsEventRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<List<DeferredApprovalsEventEntity>> GetDeferredEvents(CancellationToken cancellationToken)
        {
            var models = await dataContext.DeferredApprovalsEvent
                .OrderBy(e => e.EventTime)
                .Take(100)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return models.Select(m => new DeferredApprovalsEventEntity
                {
                    Id = m.Id,
                    EventTime = m.EventTime,
                    ApprovalsEvent = JsonConvert.DeserializeObject(m.EventBody, Type.GetType(m.EventType), SerialisationSettings)
                })
                .ToList();
        }

        public async Task<bool> DeferredEventsExist(CancellationToken cancellationToken)
        {
            return await dataContext.DeferredApprovalsEvent.AnyAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StoreDeferredEvent(DeferredApprovalsEventEntity deferredApprovalsEvent, CancellationToken cancellationToken)
        {
            await dataContext.DeferredApprovalsEvent.AddAsync(new DeferredApprovalsEventModel
            {
                EventTime = deferredApprovalsEvent.EventTime,
                EventType = deferredApprovalsEvent.ApprovalsEvent.GetType().AssemblyQualifiedName,
                EventBody = JsonConvert.SerializeObject(deferredApprovalsEvent.ApprovalsEvent, SerialisationSettings)
            }, cancellationToken).ConfigureAwait(false);

            await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteDeferredEvents(List<long> eventIds, CancellationToken cancellationToken)
        {
            var deferredApprovalsEventModels = dataContext.DeferredApprovalsEvent.Where(e => eventIds.Contains(e.Id));
            dataContext.DeferredApprovalsEvent.RemoveRange(deferredApprovalsEventModels);
            await dataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
