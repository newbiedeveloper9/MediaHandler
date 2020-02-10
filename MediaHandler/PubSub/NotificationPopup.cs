namespace MediaHandler.PubSub
{
    public struct NotificationPopup : INotificationPopup
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public NotificationPopup(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }

    public interface INotificationPopup
    {
        string Title { get; set; }
        string Message { get; set; }
    }
}