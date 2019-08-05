using System;
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

        protected BaseLegacyBulkCopyConfiguration(IConfigurationHelper configurationHelper, string tableName, Type type)
        {
            this.configurationHelper = configurationHelper;
            TableName = tableName;
            Columns = type.GetProperties().ToDictionary(p => p.Name, p => p.Name);
        }

        public IDictionary<string, string> Columns { get; }

        public string TableName { get; }

        public string ConnectionString => configurationHelper.GetConnectionString("ProviderEventsConnectionString");
    }


    public class LegacyDataLockEventBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEvent>
    {
        public LegacyDataLockEventBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEvents]", typeof(LegacyDataLockEvent))
        {
        }

    }

    public class LegacyDataLockEventCommitmentVersionBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventCommitmentVersion>
    {
        public LegacyDataLockEventCommitmentVersionBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventCommitmentVersions]", typeof(LegacyDataLockEventCommitmentVersion))
        {
        }

    }

    public class LegacyDataLockEventErrorBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventError>
    {
        public LegacyDataLockEventErrorBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventErrors]", typeof(LegacyDataLockEventError))
        {
        }

    }

    public class LegacyDataLockEventPeriodBulkCopyConfiguration : BaseLegacyBulkCopyConfiguration, IBulkCopyConfiguration<LegacyDataLockEventPeriod>
    {
        public LegacyDataLockEventPeriodBulkCopyConfiguration(IConfigurationHelper configurationHelper) 
            : base(configurationHelper, "[DataLock].[DataLockEventPeriods]", typeof(LegacyDataLockEventPeriod))
        {
        }

    }
}