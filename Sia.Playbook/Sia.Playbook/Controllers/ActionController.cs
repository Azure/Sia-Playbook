using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Sia.Playbook.Authentication;
using Sia.Playbook.Requests;
using Sia.Domain.ApiModels.Playbooks;

namespace Sia.Playbook.Controllers
{
    [Route("/actions/")]
    public class ActionController : BaseController
    {
        public ActionController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper)
        {
        }

        [HttpGet("{id}", Name = nameof(Get))]
        public async Task<IActionResult> Get(long id)
            => Ok(await _mediator.Send(new GetActionRequest(id, _authContext)));

        [HttpPost()]
        public async Task<IActionResult> Post(CreateAction content)
            => CreatedAtRoute(nameof(Get), await _mediator.Send(new PostActionRequest(content, _authContext)));

        [HttpPut("{actionId}/eventTypes/{eventTypeId}")]
        public async Task<IActionResult> AssociateEventType(long actionId, long eventTypeId)
        {
            await _mediator.Send(new AssociateActionWithEventTypeRequest(actionId, eventTypeId, _authContext));
            return Ok();
        }
    }
}
