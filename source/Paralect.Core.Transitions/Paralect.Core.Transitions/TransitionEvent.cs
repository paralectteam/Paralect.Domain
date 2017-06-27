using System;
using System.Collections.Generic;

namespace Paralect.Core.Transitions
{
    /// <summary>
    /// 
    /// </summary>
    public class TransitionEvent
    {
        /// <summary>
        /// Type of event. By default this is a fully qualified name of CLR type.
        /// But can be anything that can help identify event type during deserialization phase.
        /// </summary>
        public string TypeId { get; }

        /// <summary>
        ///     Data or body of event
        /// </summary>
        public object Data { get; }

        /// <summary>
        ///     Metadata of event
        /// </summary>
        public Dictionary<string, object> Metadata { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TransitionEvent(string typeId, object data, Dictionary<string, object> metadata)
        {
            TypeId = typeId;
            Data = data;
            Metadata = metadata ?? new Dictionary<string, object>();
        }
    }
}
