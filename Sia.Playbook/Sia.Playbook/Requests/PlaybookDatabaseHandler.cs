using MediatR;
using Sia.Data.Playbooks;
using Sia.Shared.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Requests
{
    public abstract class PlaybookDatabaseHandler<TRequest, TResult>
        : DatabaseOperationHandler<PlaybookContext, TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        protected PlaybookDatabaseHandler(PlaybookContext context) : base(context)
        {
        }
    }

    public abstract class PlaybookDatabaseHandler<TRequest>
        : DatabaseOperationHandler<PlaybookContext, TRequest>
        where TRequest : IRequest
    {
        protected PlaybookDatabaseHandler(PlaybookContext context) : base(context)
        {
        }
    }
}
