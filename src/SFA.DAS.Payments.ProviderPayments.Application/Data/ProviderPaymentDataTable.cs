using System;
using System.Data;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Data
{
    public class ProviderPaymentDataTable : PeriodisedPaymentsEventModelDataTable<ProviderPaymentEventModel>
    {
        public ProviderPaymentDataTable()
        {
            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("FundingSourceEventId", typeof(Guid)),
                new DataColumn("FundingSource"),
            });
            //DataTable.Columns.Remove("EventId");
            //DataTable.Columns.Remove("EventTime");
        }

        protected override DataRow CreateDataRow(ProviderPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["FundingSourceEventId"] = eventModel.FundingSourceId;
            dataRow["FundingSource"] = (byte)eventModel.FundingSource;

            //var dataRow = DataTable.NewRow();
            //dataRow["ExternalId"] = eventModel.EventId;

            //dataRow["LearnerReferenceNumber"] = eventModel.LearnerReferenceNumber;
            //dataRow["LearnerUln"] = eventModel.LearnerUln;
            //dataRow["LearningAimReference"] = eventModel.LearningAimReference;
            //dataRow["LearningAimProgrammeType"] = eventModel.LearningAimProgrammeType;
            //dataRow["LearningAimStandardCode"] = eventModel.LearningAimStandardCode;
            //dataRow["LearningAimFrameworkCode"] = eventModel.LearningAimFrameworkCode;
            //dataRow["LearningAimPathwayCode"] = eventModel.LearningAimPathwayCode;
            //dataRow["LearningAimFundingLineType"] = eventModel.LearningAimFundingLineType;
            //dataRow["Ukprn"] = eventModel.Ukprn;
            //dataRow["IlrSubmissionDateTime"] = eventModel.IlrSubmissionDateTime;
            //dataRow["JobId"] = eventModel.JobId;

            //dataRow["PriceEpisodeIdentifier"] = eventModel.PriceEpisodeIdentifier;
            //dataRow["ContractType"] = (byte)eventModel.ContractType;
            //dataRow["TransactionType"] = (byte)eventModel.TransactionType;
            //dataRow["Amount"] = eventModel.Amount;

            //dataRow["FundingSourceEventId"] = eventModel.FundingSourceId;
            //dataRow["FundingSource"] = (byte)eventModel.FundingSource;

            //dataRow["CollectionPeriodName"] = eventModel.CollectionPeriodName;
            //dataRow["CollectionPeriodYear"] = eventModel.CollectionPeriodYear;
            //dataRow["CollectionPeriodMonth"] = eventModel.CollectionPeriodMonth;

            //dataRow["DeliveryPeriodName"] = eventModel.DeliveryPeriodName;
            //dataRow["DeliveryPeriodYear"] = eventModel.DeliveryPeriodYear;
            //dataRow["DeliveryPeriodMonth"] = eventModel.DeliveryPeriodMonth;
            //dataRow["SfaContributionPercentage"] = eventModel.SfaContributionPercentage;

            return dataRow;
        }

        public override string TableName => "Payments2.Payment";
    }
}