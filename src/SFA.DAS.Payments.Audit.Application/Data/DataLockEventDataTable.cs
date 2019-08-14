using System;
using System.Collections.Generic;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    class DataLockEventDataTable : IPaymentsEventModelDataTable<DataLockEventModel>
    {
        public string TableName => "Payments2.DataLockEvent";

        public List<DataTable> GetDataTable(List<DataLockEventModel> events)
        {
                
        }

        private readonly DataTable dataLockEvents;
        private readonly DataTable payablePeriods;
        private readonly DataTable nonPayablePeriods;
        private readonly DataTable nonPayablePeriodFailures;
        private readonly DataTable dataLockFailures;
        private readonly DataTable priceEpisodes;

        public DataLockEventDataTable()
        {
            dataLockEvents.Columns.AddRange(new[]
            {
                new DataColumn("EventId", typeof(Guid)),
                new DataColumn("EarningEventId", typeof(Guid)),
                new DataColumn("EventTime", typeof(DateTimeOffset)),
                new DataColumn("JobId"),
                new DataColumn("CollectionPeriod"),
                new DataColumn("AcademicYear"),
                new DataColumn("Ukprn"),
                new DataColumn("LearnerReferenceNumber"),
                new DataColumn("LearnerUln"),
                new DataColumn("LearningAimReference"),
                new DataColumn("LearningAimProgrammeType"),
                new DataColumn("LearningAimStandardCode"),
                new DataColumn("LearningAimFrameworkCode"),
                new DataColumn("LearningAimPathwayCode"),
                new DataColumn("LearningAimFundingLineType"),
                new DataColumn("ContractType"),
                new DataColumn("IlrSubmissionDateTime", typeof(DateTime)),

                new DataColumn("AgreementId"),
                new DataColumn("CreationDate"),
                new DataColumn("LearningStartDate"),
                new DataColumn("IsPayable"), 
                new DataColumn("DataLockSourceId"), 
            });

            priceEpisodes = new DataTable("Payments2.DataLockEventPriceEpisode");
            priceEpisodes.Columns.AddRange(new[]
            {
                new DataColumn("DataLockEventId", typeof(Guid)),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("SfaContributionPercentage"),
                new DataColumn("TotalNegotiatedPrice1"),
                new DataColumn("TotalNegotiatedPrice2"),
                new DataColumn("TotalNegotiatedPrice3"),
                new DataColumn("TotalNegotiatedPrice4"),
                new DataColumn("StartDate",typeof(DateTime)),
                new DataColumn("EffectiveTotalNegotiatedPriceStartDate"),
                new DataColumn("PlannedEndDate",typeof(DateTime)),
                new DataColumn("ActualEndDate",typeof(DateTime)) {AllowDBNull = true},
                new DataColumn("NumberOfInstalments"),
                new DataColumn("InstalmentAmount"),
                new DataColumn("CompletionAmount"),
                new DataColumn("Completed",typeof(bool)),
                new DataColumn("EmployerContribution"),
                new DataColumn("CompletionHoldBackExemptionCode"),
                new DataColumn("NumberOfInstalments"),
                new DataColumn("CreationDate", typeof(DateTime)),
            });

            payablePeriods = new DataTable("Payments2.DataLockEventPayablePeriod");
            payablePeriods.Columns.AddRange(new[]
            {
                new DataColumn("DataLockEventId", typeof(Guid)),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("TransactionType"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("Amount"),
                new DataColumn("SfaContributionPercentage"),
                new DataColumn("CreationDate", typeof(DateTime)),
                new DataColumn("LearningStartDate",typeof(DateTime)),
                new DataColumn("ApprenticeshipId"),
                new DataColumn("ApprenticeshipPriceEpisodeId"),
                new DataColumn("ApprenticeshipEmployerType"),
            });

            dataLockFailures = new DataTable("Payments2.DataLockFailure");
            dataLockFailures.Columns.AddRange(new[]
            {
                new DataColumn("DataLockEventId", typeof(Guid)),
                new DataColumn("EarningEventId", typeof(Guid)),
                new DataColumn("Ukprn"),
                new DataColumn("LearnerUln"),
                new DataColumn("LearnerReferenceNumber"),
                new DataColumn("LearningAimReference"),
                new DataColumn("LearningAimProgrammeType"),
                new DataColumn("LearningAimStandardCode"),
                new DataColumn("LearningAimFrameworkCode"),
                new DataColumn("LearningAimPathwayCode"),
                new DataColumn("AcademicYear"),
                new DataColumn("TransactionType"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("CollectionPeriod"),
                new DataColumn("EarningPeriod"),
                new DataColumn("Amount"),
                new DataColumn("CreationDate", typeof(DateTime)),
            });

            nonPayablePeriodFailures = new DataTable("Payments2.DataLockEventNonPayablePeriodFailures");
            nonPayablePeriodFailures.Columns.AddRange(new[]
            {
                new DataColumn("DataLockEventNonPayablePeriodId", typeof(Guid)),
                new DataColumn("DataLockFailureId"),
                new DataColumn("CreationDate", typeof(DateTime)),
                new DataColumn("ApprenticeshipId") {AllowDBNull = true},
            });

            nonPayablePeriods = new DataTable("Payments2.DataLockEventNonPayablePeriod");
            nonPayablePeriods.Columns.AddRange(new[]
            {
                new DataColumn("DataLockEventId", typeof(Guid)),
                new DataColumn("DataLockEventNonPayablePeriodId", typeof(Guid)),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("TransactionType"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("Amount"),
                new DataColumn("SfaContributionPercentage"),
                new DataColumn("CreationDate", typeof(DateTime)),
                new DataColumn("LearningStartDate",typeof(DateTime)) {AllowDBNull = true},
            });
        }
    }
}
