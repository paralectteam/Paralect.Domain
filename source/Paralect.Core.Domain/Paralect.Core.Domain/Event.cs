namespace Paralect.Core.Domain
{
    /// <summary>
    /// Domain event
    /// </summary>
    public class Event : IEvent
    {
        /// <summary>
        /// Metadata of event
        /// </summary>
        public EventMetadata Metadata { get; set; } = new EventMetadata();
    }
}
