using MediatR;
using Sia.Data.Playbooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sia.Playbook.Requests
{
    public abstract class DatabaseOperationHandler<TRequest, TResult> : IAsyncRequestHandler<TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        protected readonly PlaybookContext _context;

        protected DatabaseOperationHandler(PlaybookContext context)
        {
            _context = context;
        }

        public abstract Task<TResult> Handle(TRequest message);
    }

    public abstract class DatabaseOperationHandler<TRequest> : IAsyncRequestHandler<TRequest>
    where TRequest : IRequest
    {
        protected readonly PlaybookContext _context;

        protected DatabaseOperationHandler(PlaybookContext context)
        {
            _context = context;
        }

        public abstract Task Handle(TRequest message);
    }
}
