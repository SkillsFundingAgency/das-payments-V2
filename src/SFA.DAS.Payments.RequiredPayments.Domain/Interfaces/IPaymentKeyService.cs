
namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IPaymentKeyService
    {
        string GeneratePaymentKey(string learnAimReference, int transactionType, short academicYear, byte deliveryPeriod);
    }
}