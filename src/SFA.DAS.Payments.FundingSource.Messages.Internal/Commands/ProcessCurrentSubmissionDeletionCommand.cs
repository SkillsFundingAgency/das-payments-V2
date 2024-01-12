﻿using SFA.DAS.Payments.Messages.Common.Commands;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Internal.Commands
{
    public class ProcessCurrentSubmissionDeletionCommand : PaymentsCommand
    {
        public CollectionPeriod CollectionPeriod { get; set; }

        public long AccountId { get; set; }
    }
}