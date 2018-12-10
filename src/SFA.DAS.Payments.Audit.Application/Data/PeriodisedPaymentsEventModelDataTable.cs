using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public abstract class PeriodisedPaymentsEventModelDataTable<T>: PaymentsEventModelDataTable<T> where T: IPeriodisedPaymentsEventModel
    {
        protected PeriodisedPaymentsEventModelDataTable()
        {
            DataTable.Columns.AddRange(new []
            {
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("ContractType"),
                new DataColumn("TransactionType"),
                new DataColumn("Amount"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("AgreementId"),
            });
        }

        protected override DataRow CreateDataRow(T eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["PriceEpisodeIdentifier"] = eventModel.PriceEpisodeIdentifier;
            dataRow["ContractType"] = (byte)eventModel.ContractType;
            dataRow["TransactionType"] = (byte)eventModel.TransactionType;
            dataRow["Amount"] = eventModel.Amount;
            dataRow["DeliveryPeriod"] = eventModel.DeliveryPeriod;
            dataRow["AgreementId"] = eventModel.AgreementId;
            return dataRow;
        }
    }
}