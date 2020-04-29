namespace SFA.DAS.Payments.Messaging.Serialization
{
    public class DefaultApplicationMessageModifier: IApplicationMessageModifier
    {
        public object Modify(object applicationMessage)
        {
            return applicationMessage;
        }
    }
}