namespace SFA.DAS.Payments.Application.BatchWriting
{
    public interface IBatchScopeFactory
    {
        IBatchScope Create();
    }
}