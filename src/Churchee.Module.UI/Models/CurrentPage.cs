namespace Churchee.Module.UI.Models
{
    public class CurrentPage
    {
        public event Action? Changed;

        public string CurrentPageName { get; private set; } = string.Empty;

        public void SetCurrentPageName(string name)
        {
            if (!string.Equals(CurrentPageName, name))
            {
                CurrentPageName = name;
                Changed?.Invoke();
            }
        }
    }
}
