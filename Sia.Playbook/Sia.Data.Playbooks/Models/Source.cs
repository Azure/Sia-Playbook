﻿using Sia.Shared.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sia.Data.Playbooks.Models
{
    public abstract class Source : IEntity
    {
        public long Id { get; set; }
        public SourceObject SourceObject { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public enum SourceObject
    {
        Event,
        Ticket,
        EventType,
        Engagement
    }
}
