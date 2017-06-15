using System;
using System.Collections.Generic;
using Paralect.Core.Domain.Utilities;
using Paralect.Core.Transitions;

namespace Paralect.Core.Domain
{
    public abstract class AggregateRoot
    {
        /// <summary>
        /// Unique identifier of Aggregate Root
        /// </summary>
        protected string _id;

        /// <summary>
        /// List of changes (i.e. list os pending events)
        /// </summary>
        private readonly List<IEvent> _changes = new List<IEvent>();

        /// <summary>
        /// Unique identifier of Aggregate Root
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Aggregate version
        /// </summary>
        public int Version { get; internal set; } = 0;

        protected AggregateRoot()
        {

        }

        /// <summary>
        /// Create changeset. Used to persist changes in aggregate
        /// </summary>
        /// <returns></returns>
        public Transition CreateTransition(IDataTypeRegistry dataTypeRegistry)
        {
            if (string.IsNullOrEmpty(_id))
                throw new Exception(
                    $"ID was not specified for domain object. AggregateRoot [{this.GetType().FullName}] doesn't have correct ID. Maybe you forgot to set an _id field?");

            var transitionEvents = new List<TransitionEvent>();
            foreach (var e in _changes)
            {
                e.Metadata.StoredDate = DateTime.UtcNow;
                e.Metadata.TypeName = e.GetType().Name;
                transitionEvents.Add(new TransitionEvent(dataTypeRegistry.GetTypeId(e.GetType()), e, null));
            }

            return new Transition(new TransitionId(_id, Version + 1), DateTime.UtcNow, transitionEvents, null);
        }

        /// <summary>
        /// Load aggreagate from history
        /// </summary>
        public void LoadFromTransitionStream(ITransitionStream stream)
        {
            foreach (var transition in stream.Read())
            {
                foreach (var evnt in transition.Events)
                {
                    Apply((IEvent) evnt.Data, false);
                }

                Version = transition.Id.Version;
            }
        }

        /// <summary>
        /// Load aggregate from events
        /// </summary>
        /// <param name="events"></param>
        /// <param name="version"></param>
        public void LoadFromEvents(IEnumerable<IEvent> events, Int32 version = 1)
        {
            foreach (var evnt in events)
            {
                Apply(evnt, false);
            }

            Version = version;            
        }

        /// <summary>
        /// Apply event on aggregate 
        /// </summary>
        public void Apply(IEvent evnt)
        {
            Apply(evnt, true);
        }

        private void Apply(IEvent evnt, bool isNew)
        {
            this.AsDynamic().On(evnt);
            
            if (isNew) 
                _changes.Add(evnt);
        }
    }
}
