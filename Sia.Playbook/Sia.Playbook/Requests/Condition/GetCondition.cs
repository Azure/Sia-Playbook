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
    public class GetConditionRequest : AuthenticatedRequest<Domain.Playbook.Condition>
    {
        public GetConditionRequest(long conditionId, long conditionSetId, AuthenticatedUserContext userContext)
            : base(userContext)
        {
            ConditionId = conditionId;
            ConditionSetId = conditionSetId;
        }

        public long ConditionId { get; private set; }
        public long ConditionSetId { get; private set; }
    }

    public class GetConditionHandler : PlaybookDatabaseHandler<GetConditionRequest, Domain.Playbook.Condition>
    {
        public GetConditionHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Condition> Handle(GetConditionRequest message)
            => Mapper.Map<Condition>(await _context.Conditions.FirstOrDefaultAsync(record => record.Id == message.ConditionId && record.ConditionSetId == message.ConditionSetId));
    }
}
