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
    public class GetActionRequest : AuthenticatedRequest<Domain.Playbook.Action>
    {
        public GetActionRequest(long actionId, AuthenticatedUserContext userContext)
            : base(userContext)
        {
            ActionId = actionId;
        }

        public long ActionId { get; private set; }
    }

    public class GetActionHandler : PlaybookDatabaseHandler<GetActionRequest, Domain.Playbook.Action>
    {
        public GetActionHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Domain.Playbook.Action> Handle(GetActionRequest message)
            => Mapper.Map<Domain.Playbook.Action>(await _context
                .Actions
                .WithEagerLoading()
                .FirstOrDefaultAsync(record => record.Id == message.ActionId));
    }
}
