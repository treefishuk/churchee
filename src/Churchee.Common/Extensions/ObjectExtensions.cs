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
    }
}
