namespace Churchee.Module.Site.Entities
{
    public class RedirectUrl
    {
        public RedirectUrl(string path)
        {
            Path = path;
        }

        public int Id { get; set; }

        public string Path { get; private set; }
    }
}
