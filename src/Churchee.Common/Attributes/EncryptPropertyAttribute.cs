using System;

namespace Churchee.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EncryptPropertyAttribute : Attribute;
}
