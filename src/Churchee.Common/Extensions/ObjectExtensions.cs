namespace System
{
    public static class ObjectExtensions
    {
        public static bool ImplementsInterface<TInterface>(this object thing)
        {
            ArgumentNullException.ThrowIfNull(thing);

            return thing.GetType().GetInterfaces().Contains(typeof(TInterface));
        }
    }
}
