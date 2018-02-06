using MediatR;
using Sia.Domain.Playbook;
using Sia.Shared.Authentication;
using Sia.Shared.Requests;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sia.Playbook.Requests
{
    public class GetEventTypeRequest : AuthenticatedRequest<EventType>
    {
        public GetEventTypeRequest(long id, AuthenticatedUserContext userContext)
            :base(userContext)
        {
            EventTypeId = id;
        }

        public long EventTypeId { get; private set; }
    }

    public class GetEventTypeHandler : AsyncRequestHandler<GetEventTypeRequest, EventType>
    {
        private readonly IReadOnlyDictionary<long, EventType> _index;

        public GetEventTypeHandler(IReadOnlyDictionary<long, EventType> eventTypeIndex)
        {
            _index = eventTypeIndex;
        }

        protected override Task<EventType> HandleCore(GetEventTypeRequest message)
        {
            _index.TryGetValue(message.EventTypeId, out var value);
            return Task.FromResult(value);
        }
    }
}
