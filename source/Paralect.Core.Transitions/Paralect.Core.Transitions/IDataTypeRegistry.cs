using System;

namespace Paralect.Core.Transitions
{
    public interface IDataTypeRegistry
    {
        Type GetType(string typeId);
        string GetTypeId(Type type);
    }
}