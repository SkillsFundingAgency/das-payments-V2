namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IPartitionKeyProvider
    {
        string Current { get; }
    }

    public class PartitionKeyProvider : IPartitionKeyProvider
    {
        public string Current { get; internal set; }
    }
}