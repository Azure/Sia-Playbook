﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("/actionTemplates/")]
    public class ActionTemplateController : BaseController
    {
        public ActionTemplateController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
            => Ok(await _mediator.Send(new GetActionTemplateRequest(id, _authContext)));

        [HttpPost()]
        public async Task<IActionResult> Post(CreateActionTemplate content)
            => Ok(await _mediator.Send(new PostActionTemplateRequest(content, _authContext)));
    }
}
