using Microsoft.EntityFrameworkCore;
using Sia.Data.Playbooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sia.Data.Playbooks
{
    public static class EagerLoading
    {
        public static IQueryable<Models.Action> WithEagerLoading(this DbSet<Models.Action> table)
            => table
                .Include(act => act.EventTypeAssociations)
                    .ThenInclude(ata => ata.EventType)
                .Include(act => act.ActionTemplate)
                    .ThenInclude(at => at.Sources)
                .Include(act => act.ConditionSets)
                    .ThenInclude(cs => cs.Conditions)
                        .ThenInclude(con => con.ConditionSource);

        public static IQueryable<EventType> WithEagerLoading(this DbSet<EventType> table) 
            => table
                .Include(et => et.ActionAssociations)
                    .ThenInclude(ettaa => ettaa.Action)
                        .ThenInclude(act => act.ActionTemplate)
                            .ThenInclude(at => at.Sources)
                .Include(et => et.ActionAssociations)
                    .ThenInclude(ettaa => ettaa.Action)
                        .ThenInclude(act => act.ConditionSets)
                            .ThenInclude(cs => cs.Conditions)
                                .ThenInclude(cond => cond.ConditionSource);

        public static IQueryable<ConditionSource> WithEagerLoading(this DbSet<ConditionSource> table)
            => table
                .Include(cs => cs.Conditions)
                    .ThenInclude(con => con.ConditionSet)
                        .ThenInclude(cs => cs.Action)
                            .ThenInclude(act => act.ActionTemplate)
                                .ThenInclude(at => at.Sources)
                .Include(cs => cs.Conditions)
                    .ThenInclude(con => con.ConditionSet)
                        .ThenInclude(cs => cs.Action)
                            .ThenInclude(act => act.EventTypeAssociations)
                                .ThenInclude(ata => ata.EventType);

        public static IQueryable<ActionTemplate> WithEagerLoading(this DbSet<ActionTemplate> table)
            => table
                .Include(at => at.Sources)
                .Include(at => at.Actions)
                    .ThenInclude(act => act.EventTypeAssociations)
                        .ThenInclude(ata => ata.EventType);
    }
}
