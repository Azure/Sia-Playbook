using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Domain;
using Sia.Shared.Authentication;
using Sia.Data.Playbooks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sia.Domain.Playbook;
using Sia.Shared.Requests;
using System.Collections.Concurrent;

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
