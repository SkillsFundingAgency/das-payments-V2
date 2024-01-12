﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Common;
using SFA.DAS.Payments.Messages.Common.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Base earning event
    /// </summary>
    [KnownType("GetInheritors")]
    public abstract class EarningEvent : PaymentsEvent, IEarningEvent, IMonitoredMessage
    {
        public List<PriceEpisode> PriceEpisodes { get; set; }

        public short CollectionYear { get; set; }

        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(EarningEvent).Assembly.GetTypes()
                .Where(x => x.IsSubclassOf(typeof(EarningEvent)))
                .ToArray());
        }
    }
}