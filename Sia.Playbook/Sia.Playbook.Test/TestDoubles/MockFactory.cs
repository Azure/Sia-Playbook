using Microsoft.EntityFrameworkCore;
using Sia.Data.Playbook;
using Sia.Data.Playbooks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sia.Playbook.Test.TestDoubles
{
    public static class MockFactory
    {
        /// <summary>
        /// Returns an in-memory Context with seed data
        /// </summary>
        /// <param name="instance">Name of the particular in-memory store to use. Re-use is not suggested when modifying data during test (nameof() the test method is preferred)</param>
        /// <returns></returns>
        public static async Task<PlaybookContext> PlaybookContext(string instance)
        {
            if (_contexts.TryGetValue(instance, out var context)) return context;
            while (_contextBeingGenerated.TryGetValue(instance, out var beingGenerated)
                && beingGenerated)
            {
                Thread.Sleep(100);
            }

            if (_contextBeingGenerated.TryAdd(instance, true))
            {
                var options = new DbContextOptionsBuilder<PlaybookContext>()
                    .UseInMemoryDatabase(instance)
                    .Options;
                context = new PlaybookContext(options);
                await context.AddSeedData();
                _contextBeingGenerated.TryAdd(instance, false);
                if (_contexts.TryAdd(instance, context)) return context;
                if (_contexts.TryGetValue(instance, out var otherContext)) return otherContext;
            }

            return context;
        }

        private static ConcurrentDictionary<string, bool> _contextBeingGenerated { get; set; } = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, PlaybookContext> _contexts { get; set; } = new ConcurrentDictionary<string, PlaybookContext>();
    }
}
