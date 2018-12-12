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
                new DataColumn("ExternalId", typeof(Guid)),
                new DataColumn("FundingSourceId", typeof(Guid)),
                new DataColumn("FundingSource"),
                new DataColumn("CollectionPeriodName"),
                new DataColumn("CollectionPeriodYear"),
                new DataColumn("CollectionPeriodMonth"),
                new DataColumn("DeliveryPeriodName"),
                new DataColumn("DeliveryPeriodYear"),
                new DataColumn("DeliveryPeriodMonth"),
            });
            DataTable.Columns.Remove("CollectionPeriod");
            DataTable.Columns.Remove("CollectionYear");
            DataTable.Columns.Remove("DeliveryPeriod");
            DataTable.Columns.Remove("EventId");
            DataTable.Columns.Remove("EventTime");
            DataTable.Columns.Remove("AgreementId");

        }

        protected override DataRow CreateDataRow(ProviderPaymentEventModel eventModel)
        {
            var dataRow = DataTable.NewRow();
            dataRow["ExternalId"] = eventModel.EventId;

            dataRow["LearnerReferenceNumber"] = eventModel.LearnerReferenceNumber;
            dataRow["LearnerUln"] = eventModel.LearnerUln;
            dataRow["LearningAimReference"] = eventModel.LearningAimReference;
            dataRow["LearningAimProgrammeType"] = eventModel.LearningAimProgrammeType;
            dataRow["LearningAimStandardCode"] = eventModel.LearningAimStandardCode;
            dataRow["LearningAimFrameworkCode"] = eventModel.LearningAimFrameworkCode;
            dataRow["LearningAimPathwayCode"] = eventModel.LearningAimPathwayCode;
            dataRow["LearningAimFundingLineType"] = eventModel.LearningAimFundingLineType;
            dataRow["Ukprn"] = eventModel.Ukprn;
            dataRow["IlrSubmissionDateTime"] = eventModel.IlrSubmissionDateTime;
            dataRow["JobId"] = eventModel.JobId;
            //dataRow["EventTime"] = eventModel.EventTime;

            dataRow["PriceEpisodeIdentifier"] = eventModel.PriceEpisodeIdentifier;
            dataRow["ContractType"] = (byte)eventModel.ContractType;
            dataRow["TransactionType"] = (byte)eventModel.TransactionType;
            dataRow["Amount"] = eventModel.Amount;
            //dataRow["AgreementId"] = eventModel.AgreementId;
            
            dataRow["FundingSourceId"] = eventModel.FundingSourceId;
            dataRow["FundingSource"] = (byte)eventModel.FundingSource;

            //dataRow["CollectionPeriod"] = eventModel.CollectionPeriod;
            //dataRow["CollectionYear"] = eventModel.CollectionYear;
            dataRow["CollectionPeriodName"] = eventModel.CollectionPeriodName;
            dataRow["CollectionPeriodYear"] = eventModel.CollectionPeriodYear;
            dataRow["CollectionPeriodMonth"] = eventModel.CollectionPeriodMonth;

            //dataRow["DeliveryPeriod"] = eventModel.DeliveryPeriod;
            dataRow["DeliveryPeriodName"] = eventModel.DeliveryPeriodName;
            dataRow["DeliveryPeriodYear"] = eventModel.DeliveryPeriodYear;
            dataRow["DeliveryPeriodMonth"] = eventModel.DeliveryPeriodMonth;
            dataRow["SfaContributionPercentage"] = eventModel.SfaContributionPercentage;

            return dataRow;
        }

        public override string TableName => "Payments2.Payment";
    }
}