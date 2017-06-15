using System;
using System.Collections.Generic;

namespace Paralect.Core.Transitions
{
    public interface ITransitionRepository
    {
        void SaveTransition(Transition transition);
        List<Transition> GetTransitions(string streamId, int fromVersion, int toVersion);

        /// <summary>
        /// Get all transitions ordered ascendantly by Timestamp of transiton
        /// Should be used only for testing and for very simple event replying 
        /// </summary>
        List<Transition> GetTransitions();

        void RemoveTransition(string streamId, int version);
        void RemoveStream(string streamId);

        /// <summary>
        /// Build indexes for transitions
        /// </summary>
        void EnsureIndexes();
    }
}
