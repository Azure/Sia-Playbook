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
    [Route("/conditionSources/")]
    public class ConditionSourceController : BaseController
    {
        public ConditionSourceController(IMediator mediator, AzureActiveDirectoryAuthenticationInfo authConfig, IUrlHelper urlHelper)
            : base(mediator, authConfig, urlHelper) 
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
            => Ok(await _mediator.Send(new GetConditionSourceRequest(id, _authContext)));

        [HttpPost()]
        public async Task<IActionResult> Post(CreateConditionSource content)
            => Ok(await _mediator.Send(new PostConditionSourceRequest(content,  _authContext)));
    }
}