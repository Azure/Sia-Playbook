using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sia.Shared.Authentication;
using Sia.Domain.ApiModels.Playbooks;
using Sia.Data.Playbooks;
using Sia.Domain.Playbook;
using Sia.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sia.Shared.Requests;

namespace Sia.Playbook.Requests
{
    public class PostActionTemplateSourceRequest : AuthenticatedRequest<Domain.Playbook.ActionTemplateSource>
    {
        public PostActionTemplateSourceRequest(long actionTemplateId, CreateActionTemplateSource createActionTemplateSource, AuthenticatedUserContext userContext) : base(userContext)
        {
            CreateActionTemplateSource = createActionTemplateSource;
            ActionTemplateId = actionTemplateId;
        }

        public CreateActionTemplateSource CreateActionTemplateSource { get; }
        public long ActionTemplateId { get; }
    }

    public class PostActionTemplateSourceHandler : PlaybookDatabaseHandler<PostActionTemplateSourceRequest, ActionTemplateSource>
    {
        public PostActionTemplateSourceHandler(PlaybookContext context) : base(context)
        {
        }

        public override async Task<ActionTemplateSource> Handle(PostActionTemplateSourceRequest message)
        {
            var dataActionTemplateSource = Mapper.Map<Data.Playbooks.Models.ActionTemplateSource>(message.CreateActionTemplateSource);

            var dataActionTemplate = await _context.ActionTemplates.FirstAsync(at => at.Id == message.ActionTemplateId);
            dataActionTemplate.Sources.Add(dataActionTemplateSource);
            await _context.SaveChangesAsync();

            return Mapper.Map<ActionTemplateSource>(dataActionTemplateSource);
        }
    }
}
