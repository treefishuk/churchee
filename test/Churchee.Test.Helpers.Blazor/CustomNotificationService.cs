using Radzen;

namespace Churchee.Test.Helpers.Blazor
{
    public class CustomNotificationService : NotificationService
    {
        private readonly object _lock = new();

        public List<NotificationMessage> Notifications { get; } = new();

        public CustomNotificationService()
        {
            try
            {
                // Create a new collection instance and assign it to the base class member (property or field).
                var baseType = typeof(NotificationService);
                var newCollection = new System.Collections.ObjectModel.ObservableCollection<NotificationMessage>();

                // Try property first
                var prop = baseType.GetProperty("Messages", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (prop != null && prop.CanWrite)
                {
                    // If it's static, pass null; otherwise pass 'this'.
                    prop.SetValue(prop.GetAccessors(true).Any(a => a.IsStatic) ? null : this, newCollection);
                }
                else
                {
                    // Fallback to field by common name variants
                    var field = baseType.GetField("Messages", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                                ?? baseType.GetField("messages", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                                ?? baseType.GetField("messages", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                    if (field != null)
                    {
                        bool isStatic = field.IsStatic;
                        field.SetValue(isStatic ? null : this, newCollection);
                    }
                }

                // Make sure our instance-level list is clear and subscribe to the new collection.
                Notifications.Clear();

                newCollection.CollectionChanged += OnMessagesChanged;
            }
            catch
            {
                // Defensive: if reflection fails, fall back to clearing shared collection and subscribe.
                try
                {
                    Messages.Clear();
                    Notifications.Clear();
                    Messages.CollectionChanged += OnMessagesChanged;
                }
                catch
                {
                    // best-effort; don't throw from test setup
                }
            }
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
