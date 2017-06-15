using System;

namespace Paralect.Core.Transitions
{
    public interface ITransitionStorage
    {
        ITransitionStream OpenStream(string streamId, int fromVersion, int toVersion);
        ITransitionStream OpenStream(string streamId);
    }
}
