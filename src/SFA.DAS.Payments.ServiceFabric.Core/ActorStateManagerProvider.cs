using Microsoft.ServiceFabric.Actors.Runtime;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IActorStateManagerProvider
    {
        IActorStateManager Current { get; }
    }

    public class ActorStateManagerProvider : IActorStateManagerProvider
    {
        public IActorStateManager Current { get; internal set; }
    }
}