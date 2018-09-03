﻿using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    /// <summary>
    /// Base earning event
    /// </summary>
    /// <seealso cref="SFA.DAS.Payments.EarningEvents.Messages.Events.IEarningEvent" />
    public abstract class EarningEvent : IEarningEvent
    {
        public string JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }
        public Learner Learner { get; set; }
        public LearningAim LearningAim { get; set; }
        public short EarningYear { get; set; }
        public IReadOnlyCollection<PriceEpisode> PriceEpisodes { get; set; }
    }
}