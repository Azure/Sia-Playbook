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
            var bridgeJoinEventType = new EventType()
            {
                Name = "Bridge Join"
            };
            var impactStartEventType = new EventType()
            {
                Name = "Impact Start"
            };
            var impactDetectedEventType = new EventType()
            {
                Name = "Impact Detected",
                Data = ""
            };
            var initialInternalNotificationEventType = new EventType()
            {
                Name = "Initial Internal Notification"
            };
            var initialCustomerNotificationEventType = new EventType()
            {
                Name = "Initial Customer Notification"
            };
            var followUpInternalNotificationEventType = new EventType()
            {
                Name = "Follow Up Internal Notification"
            };
            var followUpCustomerNotificationEventType = new EventType()
            {
                Name = "Follow Up Customer Notification"
            };
            var impactMitigatedEventType = new EventType()
            {
                Name = "Impact Mitigated"
            };
            var internalMitigationNotificationEventType = new EventType()
            {
                Name = "Internal Mitigation Notification"
            };
            var externalMitigationNotificationEventType = new EventType()
            {
                Name = "External Mitigation Notification"
            };
            var issueResolvedEventType = new EventType()
            {
                Name = "Issue Resolved"
            };
            var issueDowngradedEventType = new EventType()
            {
                Name = "Issue Downgraded"
            };
            var confirmCustomerImpactEventType = new EventType()
            {
                Name = "Customer Impact Confirmed"
            };
            var confirmNoCustomerImpactEventType = new EventType()
            {
                Name = "Confirmed: No Customer Impact"
            };
            var escalationStartedEventType = new EventType()
            {
                Name = "Escalation Started"
            };
            var escalationCancelledEventType = new EventType()
            {
                Name = "Escalation Cancelled"
            };
            var noteEventType = new EventType() { Name = "Note" };

            var eventTypes = new List<EventType>()
            {
                bridgeJoinEventType,
                impactStartEventType,
                impactDetectedEventType,
                initialInternalNotificationEventType,
                initialCustomerNotificationEventType,
                followUpInternalNotificationEventType,
                followUpCustomerNotificationEventType,
                impactMitigatedEventType,
                internalMitigationNotificationEventType,
                externalMitigationNotificationEventType,
                issueResolvedEventType,
                issueDowngradedEventType,
                confirmCustomerImpactEventType,
                confirmNoCustomerImpactEventType,
                escalationStartedEventType,
                escalationCancelledEventType,
                noteEventType
            };

            context.EventTypes.AddRange(eventTypes);
            await context.SaveChangesAsync();

            var waincDraftLinkTemplate = new ActionTemplate()
            {
                Name = "WAINC Draft Link",
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
            };

            var ticketSeverityConditionSource = new ConditionSource()
            {
                SourceObject = Domain.Playbook.SourceObject.Ticket,
                Key = "severity",
                Name = "Severity"
            };
            var engagementRoleConditionSource = new ConditionSource()
            {
                SourceObject = Domain.Playbook.SourceObject.Engagement,
                Key = "role",
                Name = "Role"
            };
            var ticketStatusConditionSource = new ConditionSource()
            {
                SourceObject = Domain.Playbook.SourceObject.Ticket,
                Key = "status",
                Name = "Status"
            };

            var isActiveCondition = new Condition()
            {
                Name = "Is Active",
                DataFormat = Domain.Playbook.DataFormat.String,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.Equal,
                ComparisonValue = "Active",
                ConditionSource = ticketStatusConditionSource
            };
            var isMitigatedCondition = new Condition()
            {
                Name = "Is Mitigated",
                DataFormat = Domain.Playbook.DataFormat.String,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.Equal,
                ComparisonValue = "Mitigated",
                ConditionSource = ticketStatusConditionSource
            };
            var isResolvedCondition = new Condition()
            {
                Name = "Is Resolved",
                DataFormat = Domain.Playbook.DataFormat.String,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.Equal,
                ComparisonValue = "Resolved",
                ConditionSource = ticketStatusConditionSource
            };
            var highSeverityCondition = new Condition()
            {
                Name = "High severity",
                DataFormat = Domain.Playbook.DataFormat.Integer,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.LessThan,
                IntegerComparisonValue = 3,
                ConditionSource = ticketSeverityConditionSource
            };
            var userIsWaomCondition = new Condition()
            {
                Name = "Is WAOM",
                DataFormat = Domain.Playbook.DataFormat.String,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.Equal,
                ComparisonValue = "WAOM",
                ConditionSource = engagementRoleConditionSource
            };
            var userIsSeniorImCondition = new Condition()
            {
                Name = "Is Senior Incident Manager",
                DataFormat = Domain.Playbook.DataFormat.String,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.Equal,
                ComparisonValue = "SrIM",
                ConditionSource = engagementRoleConditionSource
            };
            var userIsCommsCondition = new Condition()
            {
                Name = "Is Comms",
                DataFormat = Domain.Playbook.DataFormat.String,
                AssertionType = Domain.Playbook.AssertionType.IsOrDoes,
                ConditionType = Domain.Playbook.ConditionType.Equal,
                ComparisonValue = "Communications",
                ConditionSource = engagementRoleConditionSource
            };

            var isImpactingConditionSet = new ConditionSet()
            {
                Name = "Is Impacting",
                Type = ConditionSetType.AllOf,
                Conditions = new HashSet<Condition>()
                {
                    highSeverityCondition,
                    isActiveCondition
                }
            };
            var isMitigatedConditionSet = new ConditionSet()
            {
                Name = "Is Mitigated",
                Type = ConditionSetType.AllOf,
                Conditions = new HashSet<Condition>()
                {
                    highSeverityCondition,
                    isMitigatedCondition
                }
            };

            var isWaomConditionSet = new ConditionSet()
            {
                Name = "Is WAOM",
                Type = ConditionSetType.AnyOf,
                Conditions = new HashSet<Condition>()
                {
                    userIsWaomCondition,
                    userIsSeniorImCondition
                }
            };
            var isCommsConditionSet = new ConditionSet()
            {
                Name = "Is Comms",
                Type = ConditionSetType.AnyOf,
                Conditions = new HashSet<Condition>()
                {
                    userIsCommsCondition
                }
            };

            var initialInternalNotifyAction = new Playbooks.Models.Action()
            {
                Name = "Send Initial Internal Notification",
                ActionTemplate = waincDraftLinkTemplate,
                ConditionSets = new HashSet<ConditionSet>()
                {
                    isImpactingConditionSet
                }
            };
            var followupInternalNotifyAction = new Playbooks.Models.Action()
            {
                Name = "Send Follow-up Internal Notification",
                ActionTemplate = waincDraftLinkTemplate,
                ConditionSets = new HashSet<ConditionSet>()
                {
                    isImpactingConditionSet
                }
            };
            var mitigatedInternalNotifyAction = new Playbooks.Models.Action()
            {
                Name = "Send Mitigated Internal Notification",
                ActionTemplate = waincDraftLinkTemplate,
                ConditionSets = new HashSet<ConditionSet>()
                {
                    isMitigatedConditionSet
                }
            };

            var testAction = new Playbooks.Models.Action()
            {
                Name = "Action Without Conditions For Testing",
                ActionTemplate = new ActionTemplate()
                {
                    Name = "Link to Bing",
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

            var orphanAction = new Playbooks.Models.Action()
            {
                Name = "Orphaned Action",
                ConditionSets = new HashSet<ConditionSet>()
                {
                    new ConditionSet()
                    {
                        Name = "Condition Set For Orphaned Action"
                    }
                },
                ActionTemplate = new ActionTemplate()
            };

            var orphanEventType = new EventType()
            {
                Name = "Orphaned Event Type"
            };

            var orphanActionTemplate = new ActionTemplate()
            {
                Name = "Orphaned Action Template",
                Sources = new HashSet<ActionTemplateSource>()
                {
                    new ActionTemplateSource()
                    {
                        Name = "Source for Orphaned Action Template"
                    }
                }
            };

            var orphanConditionSet = new ConditionSet()
            {
                Name = "Orphaned Condition Set",
                Conditions = new HashSet<Condition>()
                {
                    new Condition()
                    {
                        Name = "Condition for Orphaned Condition Set"
                    }
                }
            };

            var orphanConditionSource = new ConditionSource()
            {
                Name = "Orphaned Condition Source"
            };

            impactDetectedEventType.Actions.Add(initialInternalNotifyAction);
            impactDetectedEventType.Actions.Add(testAction);

            context.EventTypes.Add(orphanEventType);
            context.Actions.Add(orphanAction);
            context.ActionTemplates.Add(orphanActionTemplate);
            context.ConditionSets.Add(orphanConditionSet);
            context.ConditionSources.Add(orphanConditionSource);

            await context.SaveChangesAsync();
        }
    }
}
