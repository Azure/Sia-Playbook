using Sia.Domain.Playbook;
using Sia.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sia.Data.Playbooks.Models
{
    public class Condition : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public AssertionType AssertionType { get; set; }
            = AssertionType.IsOrDoes;
        public ConditionType ConditionType { get; set; }
        public DataFormat DataFormat { get; set; }
            = DataFormat.String;
        public string ComparisonValue { get; set; }
        public long? IntegerComparisonValue { get; set; }
        public DateTime DateTimeComparisonValue { get; set; }
        public ConditionSource ConditionSource { get; set; }
        public long ConditionSourceId { get; set; }
        public ConditionSet ConditionSet { get; set; }
        public long ConditionSetId { get; set; }
    }
}
