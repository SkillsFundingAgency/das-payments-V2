using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Model
{
    public class EarningEventModel: PaymentsEventModel
    {
        public ContractType ContractType { get; set; }
        public string AgreementId { get; set; }
    }
}