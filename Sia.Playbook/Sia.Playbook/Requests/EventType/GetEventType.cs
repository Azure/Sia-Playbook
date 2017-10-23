using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Domain;
using Sia.Playbook.Authentication;
using Sia.Data.Playbooks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Sia.Playbook.Requests
{
    public class GetEventTypeRequest : AuthenticatedRequest<EventType>
    {
        public GetEventTypeRequest(long eventTypeId, AuthenticatedUserContext userContext)
            :base(userContext)
        {
            EventTypeId = eventTypeId;
        }

        public long EventTypeId { get; private set; }
    }

    public class GetEventTypeHandler : DatabaseOperationHandler<GetEventTypeRequest, EventType>
    {
        public GetEventTypeHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<EventType> Handle(GetEventTypeRequest message)
            => Mapper.Map<EventType>(await _context.EventTypes.FirstOrDefaultAsync(et => et.Id == message.EventTypeId));
    }
}
