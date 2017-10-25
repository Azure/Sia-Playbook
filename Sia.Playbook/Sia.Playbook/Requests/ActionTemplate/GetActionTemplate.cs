using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Domain.Playbook;
using Sia.Playbook.Authentication;
using Sia.Data.Playbooks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Sia.Playbook.Requests
{
    public class GetActionTemplateRequest : AuthenticatedRequest<Domain.Playbook.ActionTemplate>
    {
        public GetActionTemplateRequest(long actionTemplateId, AuthenticatedUserContext userContext)
            : base(userContext)
        {
            ActionTemplateId = actionTemplateId;
        }

        public long ActionTemplateId { get; private set; }
    }

    public class GetActionTemplateHandler : DatabaseOperationHandler<GetActionTemplateRequest, Domain.Playbook.ActionTemplate>
    {
        public GetActionTemplateHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<ActionTemplate> Handle(GetActionTemplateRequest message)
            => Mapper.Map<ActionTemplate>(await _context
                .ActionTemplates
                .WithEagerLoading()
                .FirstOrDefaultAsync(record => record.Id == message.ActionTemplateId));
    }
}
