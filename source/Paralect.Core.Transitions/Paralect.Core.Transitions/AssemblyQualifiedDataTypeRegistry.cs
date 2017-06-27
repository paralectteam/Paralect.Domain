using System;

namespace Paralect.Core.Transitions
{
    public class AssemblyQualifiedDataTypeRegistry : IDataTypeRegistry
    {
        public Type GetType(string typeId)
        {
            var type = Type.GetType(typeId);

            if (type == null)
                throw new Exception(string.Format("Cannot load this type: {0}. Make sure that assembly containing this type is referenced by your project.", typeId));

            return type;
        }

        public string GetTypeId(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
