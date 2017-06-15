using System.Collections.Generic;

namespace Paralect.Core.Domain.EventBus
{
    public interface IEventBus
    {
        void Publish(IEvent eventMessage);
        void Publish(IEnumerable<IEvent> eventMessages);
    }
}