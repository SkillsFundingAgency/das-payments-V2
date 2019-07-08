namespace SFA.DAS.Payments.ServiceFabric.Core.BatchWriting
{
    public interface IBatchScopeFactory
    {
        IBatchScope Create();
    }
}