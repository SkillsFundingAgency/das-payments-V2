namespace SFA.DAS.Payments.Messaging.Serialization
{
    public interface IApplicationMessageModifier
    {
        object Modify(object applicationMessage);
    }
}