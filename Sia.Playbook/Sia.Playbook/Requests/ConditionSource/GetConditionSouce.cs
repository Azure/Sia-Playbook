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
    public class GetConditionSourceRequest : AuthenticatedRequest<ConditionSource>
    {
        public GetConditionSourceRequest(long id, AuthenticatedUserContext userContext)
            :base(userContext)
        {
            ConditionSourceId = id;
        }

        public long ConditionSourceId { get; private set; }
    }

    public class GetConditionSourceHandler : PlaybookDatabaseHandler<GetConditionSourceRequest, ConditionSource>
    {
        public GetConditionSourceHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<ConditionSource> Handle(GetConditionSourceRequest message)
            => Mapper.Map<ConditionSource>(await _context
                                        .ConditionSources
                                        .WithEagerLoading()
                                        .FirstOrDefaultAsync(record => record.Id == message.ConditionSourceId));
    }
}
