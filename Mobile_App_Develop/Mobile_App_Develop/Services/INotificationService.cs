using Mobile_App_Develop.Models;

namespace Mobile_App_Develop.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllNotificationsAsync();
        Task<List<Notification>> GetUnreadNotificationsAsync();
        Task<bool> MarkAsReadAsync(string notificationId);
        Task<bool> MarkAllAsReadAsync();
        Task<bool> DeleteNotificationAsync(string notificationId);
        Task<bool> SendLocalNotificationAsync(string title, string message, NotificationType type = NotificationType.Info);
        Task<bool> ScheduleNotificationAsync(string title, string message, DateTime scheduledTime, NotificationType type = NotificationType.Reminder);
        Task<int> GetUnreadCountAsync();
        event EventHandler<NotificationEventArgs> NotificationReceived;
        event EventHandler<UnreadCountEventArgs> UnreadCountChanged;
    }
}