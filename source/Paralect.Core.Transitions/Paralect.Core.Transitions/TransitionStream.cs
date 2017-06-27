using System;
using System.Collections.Generic;

namespace Paralect.Core.Transitions
{
    public class TransitionStream : ITransitionStream
    {
        private readonly string _streamId;
        private readonly int _fromVersion;
        private readonly int _toVersion;
        private int _latestVersion;
        private readonly ITransitionRepository _transitionRepository;
        private bool _readStarted;

        private List<Transition> _transitions;
        private IEnumerator<Transition> _enumerator;

        public TransitionStream(string streamId, ITransitionRepository transitionRepository, int fromVersion,
            int toVersion)
        {
            _streamId = streamId;
            _transitionRepository = transitionRepository;
            _fromVersion = fromVersion;
            _toVersion = toVersion;
            _transitions = new List<Transition>();
        }

        public IEnumerable<Transition> Read()
        {
            // go to storage on first call
            if (!_readStarted)
            {
                // TODO: should be possible to load transitions on demand here
                _transitions = _transitionRepository.GetTransitions(_streamId, _fromVersion, _toVersion);
                _readStarted = true;
            }

            Transition current = null;
            foreach (var transition in _transitions)
            {
                if (current != null && current.Id.Version >= transition.Id.Version)
                    throw new IncorrectOrderOfTransitionsException(
                        "Order of Aggreagate Root transitions should be ascending by Version (1, 2, 10, ..., 15)");

                current = transition;
                yield return current;
            }
        }

        public void Write(Transition transition)
        {
            if (_readStarted)
                throw new InvalidOperationException(
                    "You cannot write to stream once you read from it. Open another stream.");

            try
            {
                _transitionRepository.SaveTransition(transition);
            }
            catch (DuplicateTransitionException e)
            {
                if (e.VersionId > 1)
                    throw new ConcurrencyException("Transition with same Version was saved before.");

                throw;
            }
        }

        public void Dispose()
        {
        }
    }
}
