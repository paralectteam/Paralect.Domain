using System;

namespace Paralect.Core.Domain
{
    /// <summary>
    /// Metadata of particular event
    /// </summary>
    public class EventMetadata : IEventMetadata
    {
        /// <summary>
        /// Unique Id of event
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Command Id of command that initiate this event
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// User Id of user who initiated this event
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Datetime when event was stored in Event Store.
        /// </summary>
        public DateTime StoredDate { get; set; }

        /// <summary>
        /// Assembly qualified CLR Type name
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Was transfered from previous system
        /// </summary>
        public bool TransferedEvent { get; set; }
    }


}