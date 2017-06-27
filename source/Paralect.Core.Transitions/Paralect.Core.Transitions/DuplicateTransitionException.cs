using System;

namespace Paralect.Core.Transitions
{
    public class DuplicateTransitionException : Exception
    {
        public int VersionId { get; set; }

        public string StreamId { get; set; }

        public DuplicateTransitionException(string streamId, int version, Exception innerException)
            : base(string.Format("Transition ({0}, {1}) already exists.", streamId, version), innerException)
        {
            VersionId = version;
            StreamId = streamId;
        }
    }
}
