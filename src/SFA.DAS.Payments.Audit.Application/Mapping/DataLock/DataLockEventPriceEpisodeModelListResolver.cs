using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public class DataLockEventPriceEpisodeModelListResolver : IValueResolver<DataLockEvent, DataLockEventModel, List<DataLockEventPriceEpisodeModel>>
    {
        public List<DataLockEventPriceEpisodeModel> Resolve(DataLockEvent source, DataLockEventModel destination, List<DataLockEventPriceEpisodeModel> destMember, ResolutionContext context)
        {
            var priceEpisodeModels = source.PriceEpisodes?
                .Select(priceEpisode => context.Mapper.Map<DataLockEventPriceEpisodeModel>(priceEpisode))
                .ToList() ?? new List<DataLockEventPriceEpisodeModel>();
            priceEpisodeModels.ForEach(model => model.DataLockEventId = source.EventId);
            return priceEpisodeModels;
        }
    }
}