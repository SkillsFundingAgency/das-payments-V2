using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    class DataLockEventDataTable : IPaymentsEventModelDataTable<DataLockEventModel>
    {
        public string TableName => "Payments2.DataLockEvent";

        private readonly DataTable dataLockEvents;
        private readonly DataTable payablePeriods;
        private readonly DataTable nonPayablePeriods;
        private readonly DataTable nonPayablePeriodFailures;
        private readonly DataTable dataLockFailures;
        private readonly DataTable priceEpisodes;

        public DataLockEventDataTable()
        {
            dataLockEvents.TableName = TableName;
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

        private void PopulateDataLockEventRow(DataLockEventModel model)
        {
            var row = dataLockEvents.NewRow();
            
            row["EventId"] = model.EventId;
            row["EarningEventId"] = model.EarningEventId;
            row["EventTime"] = model.EventTime;
            row["JobId"] = model.JobId;
            row["CollectionPeriod"] = model.CollectionPeriod;
            row["AcademicYear"] = model.AcademicYear;
            row["Ukprn"] = model.Ukprn;
            row["LearnerReferenceNumber"] = model.LearnerReferenceNumber;
            row["LearnerUln"] = model.LearnerUln;
            row["LearningAimReference"] = model.LearningAimReference;
            row["LearningAimProgrammeType"] = model.LearningAimProgrammeType;
            row["LearningAimStandardCode"] = model.LearningAimStandardCode;
            row["LearningAimFrameworkCode"] = model.LearningAimFrameworkCode;
            row["LearningAimPathwayCode"] = model.LearningAimPathwayCode;
            row["LearningAimFundingLineType"] = model.LearningAimFundingLineType;
            row["ContractType"] = ContractType.Act1;
            row["IlrSubmissionDateTime"] = model.IlrSubmissionDateTime;

            row["AgreementId"] = model.AgreementId;
            row["CreationDate"] = model.CreationDate;
            row["LearningStartDate"] = model.StartDate;
            row["IsPayable"] = model.IsPayable; 
            //row["DataLockSourceId"] = model.; 

            dataLockEvents.Rows.Add(row);
        }

        private void PopulateNonPayablePeriods(DataLockEventModel model)
        {
            foreach (var earningPeriod in model.OnProgrammeEarnings.SelectMany(x => x.Periods).Where(x => x.DataLockFailures.Any()))
            {
                var row = nonPayablePeriods.NewRow();
                var id = Guid.NewGuid();

                row["DataLockEventId"] = model.EventId;
                row["DataLockEventNonPayablePeriodId"] = id;
                row["PriceEpisodeIdentifier"] = earningPeriod.PriceEpisodeIdentifier;
                row["TransactionType"] = earningPeriod.;
                row["DeliveryPeriod"] = earningPeriod.Period;
                row["Amount"] = earningPeriod.Amount;
                row["SfaContributionPercentage"] = earningPeriod.SfaContributionPercentage;
                //row["CreationDate"] = model.;
                row["LearningStartDate"] = model.StartDate;

                nonPayablePeriods.Rows.Add(row);

                PopulateNonPayablePeriodFailures(earningPeriod, id);
            }
        }

        private void PopulateNonPayablePeriodFailures(EarningPeriod model, Guid parentId)
        {
            foreach (var modelDataLockFailure in model.DataLockFailures)
            {
                var row = nonPayablePeriodFailures.NewRow();

                row["DataLockEventNonPayablePeriodId"] = parentId;
                row["DataLockFailureId"] = modelDataLockFailure.DataLockError;
                //row["CreationDate"] = model.;
                row["ApprenticeshipId"] = modelDataLockFailure.ApprenticeshipId;

                nonPayablePeriodFailures.Rows.Add(row);
            }
        }

        private void PopulateDataLockFailure(DataLockEventModel model)
        {
            var row = dataLockFailures.NewRow();

            row["DataLockEventId"] = model.; 
            row["EarningEventId"] = model.; 
            row["Ukprn"] = model.;
            row["LearnerUln"] = model.;
            row["LearnerReferenceNumber"] = model.;
            row["LearningAimReference"] = model.;
            row["LearningAimProgrammeType"] = model.;
            row["LearningAimStandardCode"] = model.;
            row["LearningAimFrameworkCode"] = model.;
            row["LearningAimPathwayCode"] = model.;
            row["AcademicYear"] = model.;
            row["TransactionType"] = model.;
            row["DeliveryPeriod"] = model.;
            row["CollectionPeriod"] = model.;
            row["EarningPeriod"] = model.;
            row["Amount"] = model.;
            row["CreationDate"] = model.; 

            dataLockFailures.Rows.Add(row);
        }

        private void PopulatePayablePeriods(DataLockEventModel model)
        {
            var row = payablePeriods.NewRow();

            row["DataLockEventId"] = model.; 
            row["PriceEpisodeIdentifier"] = model.;
            row["TransactionType"] = model.;
            row["DeliveryPeriod"] = model.;
            row["Amount"] = model.;
            row["SfaContributionPercentage"] = model.;
            row["CreationDate"] = model.; 
            row["LearningStartDate"] = model.; 
            row["ApprenticeshipId"] = model.;
            row["ApprenticeshipPriceEpisodeId"] = model.;
            row["ApprenticeshipEmployerType"] = model.;

            payablePeriods.Rows.Add(row);
        }

        private void PopulatePriceEpisodes(DataLockEventModel model)
        {
            var row = priceEpisodes.NewRow();

            row["DataLockEventId"] = model.; 
            row["PriceEpisodeIdentifier"] = model.;
            row["SfaContributionPercentage"] = model.;
            row["TotalNegotiatedPrice1"] = model.;
            row["TotalNegotiatedPrice2"] = model.;
            row["TotalNegotiatedPrice3"] = model.;
            row["TotalNegotiatedPrice4"] = model.;
            row["StartDate"] = model.; 
            row["EffectiveTotalNegotiatedPriceStartDate"] = model.;
            row["PlannedEndDate"] = model.; 
            row["ActualEndDate"] = model.; 
            row["NumberOfInstalments"] = model.;
            row["InstalmentAmount"] = model.;
            row["CompletionAmount"] = model.;
            row["Completed"] = model.; 
            row["EmployerContribution"] = model.;
            row["CompletionHoldBackExemptionCode"] = model.;
            row["NumberOfInstalments"] = model.;
            row["CreationDate"] = model.; 

            priceEpisodes.Rows.Add(row);
        }

        public List<DataTable> GetDataTable(List<DataLockEventModel> events)
        {
            foreach (var dataLockEventModel in events)
            {
                PopulateDataLockEventRow(dataLockEventModel);
            }

            return new List<DataTable>
            {
                dataLockEvents,
                payablePeriods,
                nonPayablePeriods,
                nonPayablePeriodFailures,
                dataLockFailures,
                priceEpisodes,
            };
        }
    }
}
