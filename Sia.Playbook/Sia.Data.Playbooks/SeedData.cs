using Sia.Data.Playbooks;
using Sia.Data.Playbooks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sia.Data.Playbook
{
   public static class SeedData
    {
        public static async Task AddSeedData(this PlaybookContext context)
        {
            var impactDetectedEventType = new EventType()
            {
                Name = "Impact Detected",
                Data = ""
            };

            var notifyAction = new Playbooks.Models.Action()
            {
                ActionTemplate = new ActionTemplate()
                {
                    Name = "Send Initial Internal Notification",
                    IsUrl = true,
                    Template = "https://azurelse.azurewebsites.net/Message/Draft/${ticketId}",
                    Sources = new HashSet<ActionTemplateSource>()
                    {
                        new ActionTemplateSource()
                        {
                            SourceObject = Domain.Playbook.SourceObject.Ticket,
                            Key = "originId",
                            Name = "ticketId"
                        }
                    }
                },
                ConditionSets = new HashSet<ConditionSet>()
                {
                    new ConditionSet()
                    {
                        Name = "Is Impacting",
                        Type = ConditionSetType.AnyOf,
                        Conditions = new HashSet<Condition>()
                        {
                            new Condition()
                            {
                                Name = "High severity",
                                DataFormat = Domain.Playbook.DataFormat.Integer,
                                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                                ConditionType = Domain.Playbook.ConditionType.LessThan,
                                IntegerComparisonValue = 3,
                                ConditionSource = new ConditionSource()
                                {
                                    SourceObject = Domain.Playbook.SourceObject.Ticket,
                                    Key = "severity",
                                    Name = "Severity"
                                }
                            }
                        }
                    }
                }
            };

            var testAction = new Playbooks.Models.Action()
            {
                ActionTemplate = new ActionTemplate()
                {
                    Name = "Action Without Conditions For Testing",
                    IsUrl = true,
                    Template = "https://www.bing.com",
                    Sources = new HashSet<ActionTemplateSource>()
                },
                ConditionSets = new HashSet<ConditionSet>()
                {
                    new ConditionSet()
                    {
                        Name = "This condition should always be met",
                        Type = ConditionSetType.NoneOf,
                        Conditions = new HashSet<Condition>()
                    }
                }
            };
            impactDetectedEventType.Actions.Add(notifyAction);
            impactDetectedEventType.Actions.Add(testAction);

            context.EventTypes.Add(impactDetectedEventType);

            await context.SaveChangesAsync();
        }
    }
}
