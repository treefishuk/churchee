using System;
using System.Linq;

namespace System
{
    public static class ObjectExtensions
    {
        public static bool ImplementsInterface<TInterface>(this object @object)
        {
            return @object.GetType().GetInterfaces().Contains(typeof(TInterface));
        }

        public static bool TypeImplementsInterface<TInterface>(this Type type)
        {
            var interfaces = type.GetInterfaces();

            var interfaceToLookFor = typeof(TInterface);

            return interfaces.Contains(interfaceToLookFor);
        }
    }
}
