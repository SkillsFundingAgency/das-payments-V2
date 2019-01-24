namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing
{
    public interface IBatchScopeFactory
    {
        IBatchScope Create();
    }
}