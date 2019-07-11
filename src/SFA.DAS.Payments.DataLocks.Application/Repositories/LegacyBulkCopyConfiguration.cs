using System.Collections.Generic;
using System.Linq;
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
        public IDictionary<string, string> Columns { get; protected set; }

        public string TableName { get; }

        public string ConnectionString => configurationHelper.GetConnectionString("PaymentsConnectionStringV1");
    }


    public class LegacyDataLockEventBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEvent>
    {
        public LegacyDataLockEventBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEvents]")
        {
            Columns = typeof(LegacyDataLockEvent).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

    }

    public class LegacyDataLockEventCommitmentVersionBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventCommitmentVersion>
    {
        public LegacyDataLockEventCommitmentVersionBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[LegacyDataLockEventCommitmentVersion]")
        {
            Columns = typeof(LegacyDataLockEventCommitmentVersion).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

    }

    public class LegacyDataLockEventErrorBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventError>
    {
        public LegacyDataLockEventErrorBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventErrors]")
        {
            Columns = typeof(LegacyDataLockEventError).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

    }

    public class LegacyDataLockEventPeriodBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventPeriod>
    {
        public LegacyDataLockEventPeriodBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventPeriods]")
        {
            Columns = typeof(LegacyDataLockEventPeriod).GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

    }
}