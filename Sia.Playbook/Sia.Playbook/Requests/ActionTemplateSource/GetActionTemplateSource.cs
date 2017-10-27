using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Domain.Playbook;
using Sia.Shared.Authentication;
using Sia.Data.Playbooks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sia.Shared.Requests;

namespace Sia.Playbook.Requests
{
    public class GetActionTemplateSourceRequest : AuthenticatedRequest<Domain.Playbook.ActionTemplateSource>
    {
        public GetActionTemplateSourceRequest(long actionTemplateSourceId, long actionTemplateId, AuthenticatedUserContext userContext)
            : base(userContext)
        {
            ActionTemplateSourceId = actionTemplateSourceId;
            ActionTemplateId = actionTemplateId;
        }

        public long ActionTemplateSourceId { get; private set; }
        public long ActionTemplateId { get; private set; }
    }

    public class GetActionTemplateSourceHandler : PlaybookDatabaseHandler<GetActionTemplateSourceRequest, ActionTemplateSource>
    {
        public GetActionTemplateSourceHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<ActionTemplateSource> Handle(GetActionTemplateSourceRequest message)
            => Mapper.Map<ActionTemplateSource>(await _context.ActionTemplateSources.FirstOrDefaultAsync(record => record.Id == message.ActionTemplateSourceId && record.ActionTemplateId == message.ActionTemplateId));
    }
}
