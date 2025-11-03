using Mobile_App_Develop.Models;
using Plugin.LocalNotification;

namespace Mobile_App_Develop.Services
{
    public class NotificationService : INotificationService
    {
        private readonly List<Notification> _notifications;
        private int _nextId = 1;

        public event EventHandler<NotificationEventArgs>? NotificationReceived;
        public event EventHandler<UnreadCountEventArgs>? UnreadCountChanged;

        public NotificationService()
        {
            _notifications = InitializeNotifications();
        }

        private List<Notification> InitializeNotifications()
        {
            return new List<Notification>
            {
                new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Welcome to UTS Bus Tracker!",
                    Message = "Track your campus buses in real-time and never miss your ride.",
                    Type = NotificationType.Info,
                    CreatedAt = DateTime.Now.AddHours(-2),
                    IsRead = false,
                    IconName = "welcome.png"
                },
                new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Bus UTS001 Arriving Soon",
                    Message = "Your bus will arrive at Library stop in 3 minutes.",
                    Type = NotificationType.BusArrival,
                    CreatedAt = DateTime.Now.AddMinutes(-15),
                    IsRead = true,
                    BusId = 1,
                    IconName = "bus_arrival.png"
                },
                new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Route Update",
                    Message = "Campus Loop route has been updated with new timing.",
                    Type = NotificationType.RouteUpdate,
                    CreatedAt = DateTime.Now.AddHours(-1),
                    IsRead = false,
                    RouteId = 1,
                    IconName = "route_update.png"
                },
                new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Service Disruption",
                    Message = "City Express route delayed due to traffic conditions.",
                    Type = NotificationType.ServiceDisruption,
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    IsRead = false,
                    RouteId = 2,
                    IconName = "warning.png"
                }
            };
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            await Task.Delay(300);
            return _notifications.Where(n => n.IsActive)
                                .OrderByDescending(n => n.CreatedAt)
                                .ToList();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync()
        {
            await Task.Delay(200);
            return _notifications.Where(n => n.IsActive && !n.IsRead)
                                .OrderByDescending(n => n.CreatedAt)
                                .ToList();
        }

        public async Task<bool> MarkAsReadAsync(string notificationId)
        {
            await Task.Delay(100);
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                var unreadCount = await GetUnreadCountAsync();
                UnreadCountChanged?.Invoke(this, new UnreadCountEventArgs(unreadCount));
                return true;
            }
            return false;
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            await Task.Delay(200);
            var unreadNotifications = _notifications.Where(n => n.IsActive && !n.IsRead).ToList();
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }
            
            if (unreadNotifications.Any())
            {
                UnreadCountChanged?.Invoke(this, new UnreadCountEventArgs(0));
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteNotificationAsync(string notificationId)
        {
            await Task.Delay(100);
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsActive = false;
                var unreadCount = await GetUnreadCountAsync();
                UnreadCountChanged?.Invoke(this, new UnreadCountEventArgs(unreadCount));
                return true;
            }
            return false;
        }

        public async Task<bool> SendLocalNotificationAsync(string title, string message, NotificationType type = NotificationType.Info)
        {
            try
            {
                var request = new NotificationRequest
                {
                    NotificationId = _nextId,
                    Title = title,
                    Subtitle = GetTypeDisplayName(type),
                    Description = message,
                    BadgeNumber = await GetUnreadCountAsync() + 1,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1)
                    }
                };

                await LocalNotificationCenter.Current.Show(request);

                // 添加到本地通知列表
                var notification = new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title,
                    Message = message,
                    Type = type,
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    IconName = GetIconForType(type)
                };

                _notifications.Add(notification);
                NotificationReceived?.Invoke(this, new NotificationEventArgs(notification));
                
                var unreadCount = await GetUnreadCountAsync();
                UnreadCountChanged?.Invoke(this, new UnreadCountEventArgs(unreadCount));

                return true;
            }
            catch (Exception ex)
            {
                // 记录错误日志
                System.Diagnostics.Debug.WriteLine($"Failed to send notification: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ScheduleNotificationAsync(string title, string message, DateTime scheduledTime, NotificationType type = NotificationType.Reminder)
        {
            try
            {
                var request = new NotificationRequest
                {
                    NotificationId = _nextId,
                    Title = title,
                    Subtitle = GetTypeDisplayName(type),
                    Description = message,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = scheduledTime
                    }
                };

                await LocalNotificationCenter.Current.Show(request);

                // 添加到本地通知列表
                var notification = new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title,
                    Message = message,
                    Type = type,
                    CreatedAt = DateTime.Now,
                    ScheduledFor = scheduledTime,
                    IsRead = false,
                    IconName = GetIconForType(type)
                };

                _notifications.Add(notification);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to schedule notification: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetUnreadCountAsync()
        {
            await Task.Delay(50);
            return _notifications.Count(n => n.IsActive && !n.IsRead);
        }

        private string GetTypeDisplayName(NotificationType type)
        {
            return type switch
            {
                NotificationType.BusArrival => "Bus Arrival",
                NotificationType.RouteUpdate => "Route Update",
                NotificationType.ServiceDisruption => "Service Alert",
                NotificationType.Warning => "Warning",
                NotificationType.Alert => "Alert",
                NotificationType.Reminder => "Reminder",
                _ => "Information"
            };
        }

        private string GetIconForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.BusArrival => "bus_arrival.png",
                NotificationType.RouteUpdate => "route_update.png",
                NotificationType.ServiceDisruption => "warning.png",
                NotificationType.Warning => "warning.png",
                NotificationType.Alert => "alert.png",
                NotificationType.Reminder => "reminder.png",
                _ => "info_circle.png"
            };
        }
    }

    // 事件参数类
    public class NotificationEventArgs : EventArgs
    {
        public Notification Notification { get; set; }
        
        public NotificationEventArgs(Notification notification)
        {
            Notification = notification;
        }
    }

    public class UnreadCountEventArgs : EventArgs
    {
        public int Count { get; set; }
        
        public UnreadCountEventArgs(int count)
        {
            Count = count;
        }
    }
}