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
        }

        protected override DataRow CreateDataRow(ProviderPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["FundingSourceEventId"] = eventModel.FundingSourceId;
            dataRow["FundingSource"] = (byte)eventModel.FundingSource;

            if (!eventModel.ApprenticeshipId.HasValue)
            {
                dataRow["ApprenticeshipId"] = DBNull.Value;
            }
            else
            {
                dataRow["ApprenticeshipId"] = eventModel.ApprenticeshipId.Value;
            }

            if (!eventModel.ApprenticeshipPriceEpisodeId.HasValue)
            {
                dataRow["ApprenticeshipPriceEpisodeId"] = DBNull.Value;
            }
            else
            {
                dataRow["ApprenticeshipPriceEpisodeId"] = eventModel.ApprenticeshipPriceEpisodeId.Value;
            }

            return dataRow;
        }

        public override string TableName => "Payments2.Payment";
    }
}