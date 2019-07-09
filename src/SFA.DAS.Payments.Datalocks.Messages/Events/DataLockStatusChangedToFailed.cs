using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class DataLockStatusChangedToFailed : DataLockStatusChanged
    {
        public DataLockErrorCode ErrorCode { get; set; }
    }
}