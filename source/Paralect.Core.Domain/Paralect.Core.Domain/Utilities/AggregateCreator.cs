using System;
using System.Reflection;

namespace Paralect.Core.Domain.Utilities
{
    public static class AggregateCreator
    {
        public static AggregateRoot CreateAggregateRoot(Type aggregateRootType)
        {
            if (!aggregateRootType.IsSubclassOf(typeof(AggregateRoot)))
            {
                var msg = $"Specified type {aggregateRootType.FullName} is not a subclass of AggregateRoot class.";
                throw new ArgumentOutOfRangeException(nameof(aggregateRootType), msg);
            }

            // Flags to search for a public and non public contructor.
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            // Get the constructor that we want to invoke.
            var ctor = aggregateRootType.GetConstructor(flags, null, Type.EmptyTypes, null);

            // If there was no ctor found, throw exception.
            if (ctor == null)
            {
                var message =
                    $"No constructor found on aggregate root type {aggregateRootType.AssemblyQualifiedName} that accepts " +
                    "no parameters.";
                throw new Exception(message);
            }

            // There was a ctor found, so invoke it and return the instance.
            var aggregateRoot = (AggregateRoot)ctor.Invoke(null);

            return aggregateRoot;
        }

        public static T CreateAggregateRoot<T>() where T : AggregateRoot
        {
            return (T) CreateAggregateRoot(typeof (T));
        }
    }}
