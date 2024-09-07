namespace System.ComponentModel.DataAnnotations
{
    public static class RegexPattern
    {
        public const string Name = "^[a-zA-Z][a-zA-Z\\s']{0,20}[a-zA-Z]";

        public const string BasicText = "^[a-zA-Z][a-zA-Z\\s']{0,20}[a-zA-Z]";

        public const string Phone = "(?:([+]\\d{1,4})[-.\\s]?)?(?:[(](\\d{1,3})[)][-.\\s]?)?(\\d{1,4})[-.\\s]?(\\d{1,4})[-.\\s]?(\\d{1,9})";

        public const string SingleLowercaseWord = "^[a-z]\\w*$";

    }
}
