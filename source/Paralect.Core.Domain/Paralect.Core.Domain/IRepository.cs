using System;

namespace Paralect.Core.Domain
{
    public interface IRepository 
    {
        void Save(AggregateRoot aggregate);

        /// <summary>
        /// Generic version
        /// </summary>
        TAggregate GetById<TAggregate>(String id)
            where TAggregate : AggregateRoot;
    }
}