using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public interface IDataLockStatusService
    {
        DataLockStatusChange GetStatusChange(List<DataLockFailure> currentFailures, EarningPeriod newFailure);
    }

    public enum DataLockStatusChange
    {
        NoChange,
        ChangedToFailed,
        ChangedToPassed,
        FailureCodeChanged
    }
}
