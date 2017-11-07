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

    public class GetEventTypeHandler : IAsyncRequestHandler<GetEventTypeRequest, EventType>
    {
        private readonly ConcurrentDictionary<long, EventType> _index;

        public GetEventTypeHandler(ConcurrentDictionary<long, EventType> eventTypeIndex)
        {
            _index = eventTypeIndex;
        }

        public Task<EventType> Handle(GetEventTypeRequest message)
            => _index.TryGetValue(message.EventTypeId, out var value)
            ? Task.FromResult(value)
            : throw new KeyNotFoundException($"Could not find Event Type with Id:{message.EventTypeId}");
    }
}
