using System;
using System.Collections.Generic;
using System.Text;

namespace Argus.Shared.EventSources
{
    /// <summary>
    /// Simplest event source that generates a ping heartbeat 
    /// </summary>
    public class Ping : EventSource
    {

        public Ping(Modes mode) : base(mode)
        {

        }

        public int IntervalInSeconds { get; set; }

    }
}
