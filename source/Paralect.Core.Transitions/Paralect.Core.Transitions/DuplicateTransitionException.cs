using System;

namespace Paralect.Core.Transitions
{
    public class DuplicateTransitionException : Exception
    {
        public int VersionId { get; set; }

        public string StreamId { get; set; }

        public DuplicateTransitionException(string streamId, int version, Exception innerException)
            : base($"Transition ({streamId}, {version}) already exists.", innerException)
        {
            VersionId = version;
            StreamId = streamId;
        }
    }
}
