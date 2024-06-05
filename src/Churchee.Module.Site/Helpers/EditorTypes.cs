namespace Churchee.Module.Site.Helpers
{
    public static class EditorType
    {
        public const string RichTextEditor = "RichTextEditor";
        public const string Number = "Number";
        public const string SimpleText = "SimpleText";
        public const string MultilineText = "MultilineText";


        public static List<string> All()
        {
            return new List<string>
            {
                RichTextEditor, Number, SimpleText, MultilineText
            };
        }
    }
}
