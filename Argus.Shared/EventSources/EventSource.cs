using System;
using System.Collections.Generic;
using System.Text;

namespace Argus.Shared.EventSources
{
    /// <summary>
    /// Base class for event sources
    /// </summary>
    public abstract class EventSource
    {
        public EventSource(Modes mode)
        {

        }

        private readonly Modes _mode;

        /// <summary>
        /// Mode this event source is operating in
        /// </summary>
        public Modes Modes => _mode;


        public void SendMessageToServer(string message)
        {

        }

        public void ReceiveMessageFromServer(string message)
        {

        }
    }

    /// <summary>
    /// Whether the event source is operating as a source or client
    /// </summary>
    public enum Modes
    {
        /// <summary>
        /// Source mode
        /// </summary>
        Source,
        /// <summary>
        /// Client mode
        /// </summary>
        Client
    }
}
