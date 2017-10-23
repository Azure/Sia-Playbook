using Sia.Data.Playbooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Playbook.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Domain;
using AutoMapper;

namespace Sia.Playbook.Requests
{
    public class PostEventTypeRequest : AuthenticatedRequest<Domain.Playbook.EventType>
    {
        public PostEventTypeRequest(CreateEventType createEventType, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateEventType = createEventType;
        }

        public CreateEventType CreateEventType { get; }
    }

    public class PostEventTypeHandler : DatabaseOperationHandler<PostEventTypeRequest, Domain.Playbook.EventType>
    {
        public PostEventTypeHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Domain.Playbook.EventType> Handle(PostEventTypeRequest message)
        {
            var dataEventType = Mapper.Map<Data.Playbooks.Models.EventType>(message.CreateEventType);

            var result = _context.EventTypes.Add(dataEventType);
            await _context.SaveChangesAsync();

            return Mapper.Map<Domain.Playbook.EventType>(dataEventType);
        }
    }
}
