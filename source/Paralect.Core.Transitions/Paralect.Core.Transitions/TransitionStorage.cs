using System;

namespace Paralect.Core.Transitions
{
    public class TransitionStorage : ITransitionStorage
    {
        private readonly ITransitionRepository _transitionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TransitionStorage(ITransitionRepository transitionRepository)
        {
            _transitionRepository = transitionRepository;
        }

        public ITransitionStream OpenStream(string streamId, int fromVersion, int toVersion)
        {
            return new TransitionStream(streamId, _transitionRepository, fromVersion, toVersion);
        }

        public ITransitionStream OpenStream(string streamId)
        {
            return new TransitionStream(streamId, _transitionRepository, 0, int.MaxValue);
        }
    }
}
