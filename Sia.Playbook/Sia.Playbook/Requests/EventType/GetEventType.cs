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

    public class GetEventTypeHandler : PlaybookDatabaseHandler<GetEventTypeRequest, EventType>
    {
        public GetEventTypeHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<EventType> Handle(GetEventTypeRequest message)
            => Mapper.Map<EventType>(await _context
                                        .EventTypes
                                        .WithEagerLoading()
                                        .FirstOrDefaultAsync(record => record.Id == message.EventTypeId));
    }
}
