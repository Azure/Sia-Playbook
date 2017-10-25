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
    public class GetConditionSetRequest : AuthenticatedRequest<Domain.Playbook.ConditionSet>
    {
        public GetConditionSetRequest(long conditionSetId, long actionId, AuthenticatedUserContext userContext)
            : base(userContext)
        {
            ConditionSetId = conditionSetId;
            ActionId = actionId;
        }

        public long ConditionSetId { get; private set; }
        public long ActionId { get; }
    }

    public class GetConditionSetHandler : DatabaseOperationHandler<GetConditionSetRequest, Domain.Playbook.ConditionSet>
    {
        public GetConditionSetHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<ConditionSet> Handle(GetConditionSetRequest message)
            => Mapper.Map<ConditionSet>(await _context.ConditionSets.FirstOrDefaultAsync(record => record.Id == message.ConditionSetId && record.ActionId == message.ActionId));
    }
}
