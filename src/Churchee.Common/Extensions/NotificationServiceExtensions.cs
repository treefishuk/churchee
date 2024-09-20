using Churchee.Common.ResponseTypes;

namespace Radzen
{
    public static class NotificationServiceExtensions
    {
        public static void Notify(this NotificationService service, CommandResponse commandResponse)
        {
            service.Notify(commandResponse, "Saved Successfully");
        }

        public static void Notify(this NotificationService service, CommandResponse commandResponse, string successMessage)
        {
            if (commandResponse.IsSuccess)
            {
                service.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = successMessage });
            }

            if (!commandResponse.IsSuccess)
            {
                foreach (var error in commandResponse.Errors)
                {
                    service.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = error.Description });
                }
            }
        }
    }
}
