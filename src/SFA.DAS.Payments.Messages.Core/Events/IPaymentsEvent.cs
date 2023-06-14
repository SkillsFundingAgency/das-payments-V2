﻿using SFA.DAS.Payments.Model.Core;
using System;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IPaymentsEvent : IJobMessage, IEvent
    {
        long Ukprn { get; }
        Learner Learner { get; }
        LearningAim LearningAim { get; }
        DateTime? IlrSubmissionDateTime { get; }
        CollectionPeriod CollectionPeriod { get; }
    }
}