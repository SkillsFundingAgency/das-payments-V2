using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public class DataLockEventDataTable : IPaymentsEventModelDataTable<DataLockEventModel>
    {
        public string TableName => "Payments2.DataLockEvent";

        private readonly DataTable dataLockEvents;
        private readonly DataTable payablePeriods;
        private readonly DataTable nonPayablePeriods;
        private readonly DataTable nonPayablePeriodFailures;
        private readonly DataTable priceEpisodes;

        public DataLockEventDataTable()
        {
            dataLockEvents = new DataTable(TableName);
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
                new DataColumn("StartDate", typeof(DateTime)),
                new DataColumn("EffectiveTotalNegotiatedPriceStartDate"),
                new DataColumn("PlannedEndDate", typeof(DateTime)),
                new DataColumn("ActualEndDate", typeof(DateTime)) {AllowDBNull = true},
                new DataColumn("NumberOfInstalments"),
                new DataColumn("InstalmentAmount"),
                new DataColumn("CompletionAmount"),
                new DataColumn("Completed", typeof(bool)),
                new DataColumn("EmployerContribution"),
                new DataColumn("CompletionHoldBackExemptionCode"),
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
                new DataColumn("LearningStartDate", typeof(DateTime)),
                new DataColumn("ApprenticeshipId"),
                new DataColumn("ApprenticeshipPriceEpisodeId"),
                new DataColumn("ApprenticeshipEmployerType"),
            });

            nonPayablePeriodFailures = new DataTable("Payments2.DataLockEventNonPayablePeriodFailures");
            nonPayablePeriodFailures.Columns.AddRange(new[]
            {
                new DataColumn("DataLockEventNonPayablePeriodId", typeof(Guid)),
                new DataColumn("DataLockFailureId"),
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
                new DataColumn("LearningStartDate", typeof(DateTime)) {AllowDBNull = true},
            });
        }

        private void PopulateDataLockEvent(DataLockEventModel model)
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
            row["ContractType"] = (int)ContractType.Act1;
            row["IlrSubmissionDateTime"] = model.IlrSubmissionDateTime;

            row["AgreementId"] = model.AgreementId;
            row["LearningStartDate"] = model.StartDate;
            row["IsPayable"] = model.IsPayable;
            row["DataLockSourceId"] = 1; // This should be changed when the commitment validation is in place

            dataLockEvents.Rows.Add(row);
        }

        private void PopulateNonPayablePeriods(DataLockEventModel model)
        {

            if (model.OnProgrammeEarnings != null)
            {
                foreach (var onProgrammeEarning in model.OnProgrammeEarnings)
                {
                    foreach (var earningPeriod in onProgrammeEarning.Periods)
                    {
                        var row = nonPayablePeriods.NewRow();
                        var id = Guid.NewGuid();

                        row["DataLockEventId"] = model.EventId;
                        row["DataLockEventNonPayablePeriodId"] = id;
                        row["PriceEpisodeIdentifier"] = earningPeriod.PriceEpisodeIdentifier;
                        row["TransactionType"] = (int)onProgrammeEarning.Type;
                        row["DeliveryPeriod"] = earningPeriod.Period;
                        row["Amount"] = earningPeriod.Amount;
                        row["SfaContributionPercentage"] = earningPeriod.SfaContributionPercentage ?? (object)DBNull.Value;
                        row["LearningStartDate"] = model.StartDate;

                        nonPayablePeriods.Rows.Add(row);

                        PopulateNonPayablePeriodFailures(earningPeriod, id);
                    }
                }
            }

            if (model.IncentiveEarnings != null)
            {
                foreach (var incentiveEarning in model.IncentiveEarnings)
                {
                    foreach (var earningPeriod in incentiveEarning.Periods)
                    {
                        var row = nonPayablePeriods.NewRow();
                        var id = Guid.NewGuid();

                        row["DataLockEventId"] = model.EventId;
                        row["DataLockEventNonPayablePeriodId"] = id;
                        row["PriceEpisodeIdentifier"] = earningPeriod.PriceEpisodeIdentifier;
                        row["TransactionType"] = (int)incentiveEarning.Type;
                        row["DeliveryPeriod"] = earningPeriod.Period;
                        row["Amount"] = earningPeriod.Amount;
                        row["SfaContributionPercentage"] = earningPeriod.SfaContributionPercentage;
                        row["LearningStartDate"] = model.StartDate;

                        nonPayablePeriods.Rows.Add(row);

                        PopulateNonPayablePeriodFailures(earningPeriod, id);
                    }
                }
            }
            
            if (model.Earnings != null)
            {
                foreach (var functionalSkillEarning in model.Earnings)
                {
                    foreach (var earningPeriod in functionalSkillEarning.Periods)
                    {
                        var row = nonPayablePeriods.NewRow();
                        var id = Guid.NewGuid();

                        row["DataLockEventId"] = model.EventId;
                        row["DataLockEventNonPayablePeriodId"] = id;
                        row["PriceEpisodeIdentifier"] = earningPeriod.PriceEpisodeIdentifier;
                        row["TransactionType"] = (int)functionalSkillEarning.Type;
                        row["DeliveryPeriod"] = earningPeriod.Period;
                        row["Amount"] = earningPeriod.Amount;
                        row["SfaContributionPercentage"] = earningPeriod.SfaContributionPercentage;
                        row["LearningStartDate"] = model.StartDate;

                        nonPayablePeriods.Rows.Add(row);

                        PopulateNonPayablePeriodFailures(earningPeriod, id);
                    }
                }
            }
        }

        private void PopulatePayablePeriods(DataLockEventModel model)
        {
            if (model.IncentiveEarnings != null)
            {
                foreach (var incentiveEarning in model.IncentiveEarnings)
                {
                    LoadPayablePeriodDataTable(model, incentiveEarning.Periods, (int)incentiveEarning.Type);
                }
            }

            if (model.OnProgrammeEarnings != null)
            {
                foreach (var onProgrammeEarning in model.OnProgrammeEarnings)
                {
                    LoadPayablePeriodDataTable(model, onProgrammeEarning.Periods, (int)onProgrammeEarning.Type);
                }
            }

            if (model.Earnings != null)
            {
                foreach (var functionalSkillEarning in model.Earnings)
                {
                    LoadPayablePeriodDataTable(model, functionalSkillEarning.Periods, (int)functionalSkillEarning.Type);
                }
            }
        }

        private void PopulatePriceEpisodes(DataLockEventModel model)
        {
            foreach (var priceEpisode in model.PriceEpisodes)
            {
                var row = priceEpisodes.NewRow();

                row["DataLockEventId"] = model.EventId;
                row["PriceEpisodeIdentifier"] = priceEpisode.Identifier;
                // This is a period value rather than a P/E value
                row["SfaContributionPercentage"] = 0;
                row["TotalNegotiatedPrice1"] = priceEpisode.TotalNegotiatedPrice1;
                row["TotalNegotiatedPrice2"] = priceEpisode.TotalNegotiatedPrice2 ?? (object)DBNull.Value;
                row["TotalNegotiatedPrice3"] = priceEpisode.TotalNegotiatedPrice3 ?? (object)DBNull.Value;
                row["TotalNegotiatedPrice4"] = priceEpisode.TotalNegotiatedPrice4 ?? (object)DBNull.Value;
                row["StartDate"] = priceEpisode.StartDate;
                row["EffectiveTotalNegotiatedPriceStartDate"] = priceEpisode.EffectiveTotalNegotiatedPriceStartDate;
                row["PlannedEndDate"] = priceEpisode.PlannedEndDate;
                row["ActualEndDate"] = priceEpisode.ActualEndDate ?? (object)DBNull.Value;
                row["NumberOfInstalments"] = priceEpisode.NumberOfInstalments;
                row["InstalmentAmount"] = priceEpisode.InstalmentAmount;
                row["CompletionAmount"] = priceEpisode.CompletionAmount;
                row["Completed"] = priceEpisode.Completed;
                row["EmployerContribution"] = priceEpisode.EmployerContribution ?? (object)DBNull.Value;
                row["CompletionHoldBackExemptionCode"] =
                    priceEpisode.CompletionHoldBackExemptionCode ?? (object)DBNull.Value;

                priceEpisodes.Rows.Add(row);
            }
        }

        private void PopulateNonPayablePeriodFailures(EarningPeriod model, Guid parentId)
        {
            foreach (var modelDataLockFailure in model.DataLockFailures)
            {
                var row = nonPayablePeriodFailures.NewRow();

                row["DataLockEventNonPayablePeriodId"] = parentId;
                row["DataLockFailureId"] = (byte)modelDataLockFailure.DataLockError;
                row["ApprenticeshipId"] = modelDataLockFailure.ApprenticeshipId ?? (object)DBNull.Value;

                nonPayablePeriodFailures.Rows.Add(row);
            }
        }

        public List<DataTable> GetDataTable(List<DataLockEventModel> events)
        {
            foreach (var dataLockEvent in events)
            {
                PopulateDataLockEvent(dataLockEvent);
                PopulatePriceEpisodes(dataLockEvent);

                if (dataLockEvent.IsPayable)
                {
                    PopulatePayablePeriods(dataLockEvent);
                }
                else
                {
                    PopulateNonPayablePeriods(dataLockEvent);
                }
            }

            return new List<DataTable>
            {
                dataLockEvents,
                payablePeriods,
                nonPayablePeriods,
                nonPayablePeriodFailures,
                priceEpisodes,
            };
        }

        private void LoadPayablePeriodDataTable(DataLockEventModel model, ReadOnlyCollection<EarningPeriod> periods, int transactionType)
        {
            foreach (var earningPeriod in periods)
            {
                var row = payablePeriods.NewRow();

                row["DataLockEventId"] = model.EventId;
                row["PriceEpisodeIdentifier"] = earningPeriod.PriceEpisodeIdentifier;
                row["TransactionType"] = transactionType;
                row["DeliveryPeriod"] = earningPeriod.Period;
                row["Amount"] = earningPeriod.Amount;
                row["SfaContributionPercentage"] = earningPeriod.SfaContributionPercentage ?? (object)DBNull.Value;
                row["LearningStartDate"] = model.StartDate;
                row["ApprenticeshipId"] = earningPeriod.ApprenticeshipId ?? (object)DBNull.Value;
                row["ApprenticeshipPriceEpisodeId"] = earningPeriod.ApprenticeshipPriceEpisodeId ?? (object)DBNull.Value;
                //row["ApprenticeshipEmployerType"] = ;

                payablePeriods.Rows.Add(row);
            }
        }
    }
}
