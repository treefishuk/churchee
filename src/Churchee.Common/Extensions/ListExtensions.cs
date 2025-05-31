namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static T GetLast<T>(this List<T> list)
        {
            return list[^1];
        }
    }
}
