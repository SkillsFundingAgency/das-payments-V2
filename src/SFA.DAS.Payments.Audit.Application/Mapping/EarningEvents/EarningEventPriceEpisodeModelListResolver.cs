﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Mapping.EarningEvents
{
    public class EarningEventPriceEpisodeModelListResolver: IValueResolver<EarningEvent,EarningEventModel,List<EarningEventPriceEpisodeModel>>
    {
        public List<EarningEventPriceEpisodeModel> Resolve(EarningEvent source, EarningEventModel destination, List<EarningEventPriceEpisodeModel> destMember, ResolutionContext context)
        {
            var priceEpisodeModels = source.PriceEpisodes?
                .Select(priceEpisode => context.Mapper.Map<EarningEventPriceEpisodeModel>(priceEpisode))
                .ToList() ?? new List<EarningEventPriceEpisodeModel>();
            priceEpisodeModels.ForEach(model => model.EarningEventId = source.EventId);
            return priceEpisodeModels;
        }
    }
}