using System.Collections.Generic;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Model.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public abstract class BaseLegacyBulkCopyConfiguration
    {
        private readonly IConfigurationHelper configurationHelper;

        protected BaseLegacyBulkCopyConfiguration(IConfigurationHelper configurationHelper, string tableName)
        {
            this.configurationHelper = configurationHelper;
            TableName = tableName;
        }
        public IDictionary<string, string> Columns => null;

        public string TableName { get; }

        public string ConnectionString => configurationHelper.GetConnectionString("PaymentsConnectionStringV1");
    }


    public class LegacyDataLockEventBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEvent>
    {
        public LegacyDataLockEventBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEvents]")
        {
        }

    }

    public class LegacyDataLockEventCommitmentVersionBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventCommitmentVersion>
    {
        public LegacyDataLockEventCommitmentVersionBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[LegacyDataLockEventCommitmentVersion]")
        {
        }

    }

    public class LegacyDataLockEventErrorBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventError>
    {
        public LegacyDataLockEventErrorBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventErrors]")
        {
        }

    }

    public class LegacyDataLockEventPeriodBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventPeriod>
    {
        public LegacyDataLockEventPeriodBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventPeriods]")
        {
        }

    }
}