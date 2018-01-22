using MediatR;
using Sia.Domain.Playbook;
using Sia.Shared.Authentication;
using Sia.Shared.Requests;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Requests
{
    public class GetEventTypesRequest : AuthenticatedRequest<IEnumerable<EventType>>
    {
        public GetEventTypesRequest(AuthenticatedUserContext userContext)
            : base(userContext)
        {
        }
    }

    public class GetEventTypesHandler : AsyncRequestHandler<GetEventTypesRequest, IEnumerable<EventType>>
    {
        private readonly IReadOnlyDictionary<long, EventType> _index;

        public GetEventTypesHandler(IReadOnlyDictionary<long, EventType> eventTypeIndex)
        {
            _index = eventTypeIndex;
        }

        protected override Task<IEnumerable<EventType>> HandleCore(GetEventTypesRequest message)
            => Task.FromResult(_index.Values.AsEnumerable());
    }
}