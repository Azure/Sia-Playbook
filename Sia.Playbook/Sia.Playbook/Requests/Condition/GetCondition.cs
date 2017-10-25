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

    public class GetConditionHandler : DatabaseOperationHandler<GetConditionRequest, Domain.Playbook.Condition>
    {
        public GetConditionHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Condition> Handle(GetConditionRequest message)
            => Mapper.Map<Condition>(await _context.Conditions.FirstOrDefaultAsync(record => record.Id == message.ConditionId && record.ConditionSetId == message.ConditionSetId));
    }
}
