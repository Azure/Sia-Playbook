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

    public class GetEventTypesHandler : IAsyncRequestHandler<GetEventTypesRequest, IEnumerable<EventType>>
    {
        private readonly ConcurrentDictionary<long, EventType> _index;

        public GetEventTypesHandler(ConcurrentDictionary<long, EventType> eventTypeIndex)
        {
            _index = eventTypeIndex;
        }

        public Task<IEnumerable<EventType>> Handle(GetEventTypesRequest message)
            => Task.FromResult(_index.Values.AsEnumerable());

    }
}