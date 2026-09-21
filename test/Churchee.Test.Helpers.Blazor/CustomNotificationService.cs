using Radzen;

namespace Churchee.Test.Helpers.Blazor
{
    public abstract partial class BasePageTests
    {
        public class CustomNotificationService : NotificationService
        {
            private readonly object _lock = new();

            public List<NotificationMessage> Notifications { get; } = [];

            public CustomNotificationService()
            {
                // Make sure any leftover messages from other tests are cleared
                try
                {
                    Messages.Clear();
                }
                catch
                {
                    // defensive: if Messages is null or shared in a way that throws, don't fail tests here
                }

                Notifications.Clear();

                Messages.CollectionChanged += OnMessagesChanged;
            }

            private void OnMessagesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                {
                    lock (_lock)
                    {
                        foreach (NotificationMessage newItem in e.NewItems)
                        {
                            Notifications.Add(newItem);
                        }
                    }
                }
            }
        }

    }

}
