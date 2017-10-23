using Sia.Data.Playbooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Playbook.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain;
using AutoMapper;

namespace Sia.Playbook.Requests
{
    public class PostEventTypeRequest : AuthenticatedRequest<Domain.EventType>
    {
        public PostEventTypeRequest(CreateEventType createEventType, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateEventType = createEventType;
        }

        public CreateEventType CreateEventType { get; }
    }

    public class PostEventTypeHandler : DatabaseOperationHandler<PostEventTypeRequest, Domain.EventType>
    {
        public PostEventTypeHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Domain.EventType> Handle(PostEventTypeRequest message)
        {
            var dataEventType = Mapper.Map<Data.Playbooks.Models.EventType>(message.CreateEventType);

            var result = _context.EventTypes.Add(dataEventType);
            await _context.SaveChangesAsync();

            return Mapper.Map<Domain.EventType>(dataEventType);
        }
    }
}
