using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public interface IDataLockStatusService
    {
        DataLockStatusChange GetStatusChange(List<DataLockFailure> oldFailures, List<DataLockFailure> newFailures);
    }

    public enum DataLockStatusChange
    {
        NoChange,
        ChangedToFailed,
        ChangedToPassed,
        FailureCodeChanged
    }
}
