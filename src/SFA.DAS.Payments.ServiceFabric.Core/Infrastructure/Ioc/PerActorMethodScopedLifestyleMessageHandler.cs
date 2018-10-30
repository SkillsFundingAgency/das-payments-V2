using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc
{
    public class PerActorMethodScopedLifestyleMessageHandler : ActorServiceRemotingDispatcher
    {
        public PerActorMethodScopedLifestyleMessageHandler(ActorService actorService, IServiceRemotingMessageBodyFactory serviceRemotingRequestMessageBodyFactory)
            : base(actorService, serviceRemotingRequestMessageBodyFactory)
        {
        }
 
        public override void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
        {
            base.HandleOneWayMessage(requestMessage);
        }
 
        public override Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(IServiceRemotingRequestContext requestContext, IServiceRemotingRequestMessage requestMessage)
        {
            return base.HandleRequestResponseAsync(requestContext, requestMessage);
        }
    }
}