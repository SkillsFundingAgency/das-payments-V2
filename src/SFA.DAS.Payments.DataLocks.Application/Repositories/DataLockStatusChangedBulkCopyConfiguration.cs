using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public class DataLockStatusChangedBulkCopyConfiguration : IBulkCopyConfiguration<DataLockStatusChanged>
    {
        public string TableName => "[DataLock].[DataLockEvent]";

        public IDictionary<string, string> Columns => null;
    }
}