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
using Sia.Domain.Playbook;

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

    public class GetEventTypeHandler : DatabaseOperationHandler<GetEventTypeRequest, EventType>
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
