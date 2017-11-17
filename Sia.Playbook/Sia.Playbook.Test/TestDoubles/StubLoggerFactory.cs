using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sia.Playbook.Test.TestDoubles
{
    internal class StubLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider) => throw new NotImplementedException();
        public ILogger CreateLogger(string categoryName) => new StubLogger();
        public void Dispose() => throw new NotImplementedException();
    }
}
