using System.Collections.Generic;
using Paralect.Core.Domain;

namespace Paralect.Domain.EventBus
{
    public interface IEventBus
    {
        void Publish(IEvent eventMessage);
        void Publish(IEnumerable<IEvent> eventMessages);
    }
}