using MediatR;
using Sia.Playbook.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Requests
{
    public abstract class AuthenticatedRequest<T> : IRequest<T>
    {
        protected AuthenticatedRequest(AuthenticatedUserContext userContext)
        {
            UserContext = userContext;
        }

        public AuthenticatedUserContext UserContext { get; private set; }
    }

    public abstract class AuthenticatedRequest : IRequest
    {
        protected AuthenticatedRequest(AuthenticatedUserContext userContext)
        {
            UserContext = userContext;
        }

        public AuthenticatedUserContext UserContext { get; private set; }
    }
}
