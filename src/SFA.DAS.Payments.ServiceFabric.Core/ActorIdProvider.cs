using Microsoft.ServiceFabric.Actors;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IActorIdProvider
    {
        ActorId Current { get; }
    }

    public class ActorIdProvider : IActorIdProvider
    {
        public ActorId Current { get; internal set; }
    }
}