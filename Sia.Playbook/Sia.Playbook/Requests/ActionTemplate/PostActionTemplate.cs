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
    public class PostActionTemplateRequest : AuthenticatedRequest<Domain.Playbook.ActionTemplate>
    {
        public PostActionTemplateRequest(CreateActionTemplate createActionTemplate, AuthenticatedUserContext userContext) 
            : base(userContext)
        {
            CreateActionTemplate = createActionTemplate;
        }

        public CreateActionTemplate CreateActionTemplate { get; }
    }

    public class PostActionTemplateHandler : PlaybookDatabaseHandler<PostActionTemplateRequest, Domain.Playbook.ActionTemplate>
    {
        public PostActionTemplateHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<Domain.Playbook.ActionTemplate> Handle(PostActionTemplateRequest message)
        {
            var dataActionTemplate = Mapper.Map<Data.Playbooks.Models.ActionTemplate>(message.CreateActionTemplate);

            var result = _context.ActionTemplates.Add(dataActionTemplate);
            await _context.SaveChangesAsync();

            return Mapper.Map<Domain.Playbook.ActionTemplate>(dataActionTemplate);
        }
    }
}
