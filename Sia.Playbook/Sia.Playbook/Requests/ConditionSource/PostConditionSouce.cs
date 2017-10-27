using Sia.Data.Playbooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Shared.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Domain;
using AutoMapper;
using Sia.Shared.Requests;

namespace Sia.Playbook.Requests
{
    public class PostConditionSourceRequest : AuthenticatedRequest<Domain.Playbook.ConditionSource>
    {
        public PostConditionSourceRequest(CreateConditionSource createConditionSource, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateConditionSource = createConditionSource;
        }

        public CreateConditionSource CreateConditionSource { get; }
    }

    public class PostConditionSourceHandler : PlaybookDatabaseHandler<PostConditionSourceRequest, Domain.Playbook.ConditionSource>
    {
        public PostConditionSourceHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Domain.Playbook.ConditionSource> Handle(PostConditionSourceRequest message)
        {
            var dataConditionSource = Mapper.Map<Data.Playbooks.Models.ConditionSource>(message.CreateConditionSource);

            var result = _context.ConditionSources.Add(dataConditionSource);
            await _context.SaveChangesAsync();

            return Mapper.Map<Domain.Playbook.ConditionSource>(dataConditionSource);
        }
    }
}
