namespace SFA.DAS.Payments.Messages.Core.Events
{
    public interface IContractType1EarningEvent : IContractTypeEarningEvent
    {
        string AgreementId { get; }
    }
}