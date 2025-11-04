using Mobile_App_Develop;
using Mobile_App_Develop.Models;
using Mobile_App_Develop.Services;
using System.Collections.ObjectModel;

namespace Mobile_App_Develop.Views;

public partial class NotificationsPage : ContentPage
{
    private readonly INotificationService _notificationService;
    
    public ObservableCollection<NotificationViewModel> Notifications { get; set; }

    public NotificationsPage(INotificationService notificationService)
    {
        InitializeComponent();
        _notificationService = notificationService;
        
        Notifications = new ObservableCollection<NotificationViewModel>();
        NotificationsCollectionView.ItemsSource = Notifications;
        
        // 监听通知事件
        _notificationService.NotificationReceived += OnNotificationReceived;
        _notificationService.UnreadCountChanged += OnUnreadCountChanged;
    }

    // 供 XAML 实例化使用的无参构造函数
    public NotificationsPage() : this(ServiceHelper.GetService<INotificationService>())
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadNotifications();
    }

    private async Task LoadNotifications()
    {
        try
        {
            await SetLoadingState(true);
            
            // 加载通知数据
            var notifications = await _notificationService.GetAllNotificationsAsync();
            var unreadCount = await _notificationService.GetUnreadCountAsync();
            
            // 更新UI
            Notifications.Clear();
            foreach (var notification in notifications.OrderByDescending(n => n.CreatedAt))
            {
                var viewModel = new NotificationViewModel
                {
                    Id = notification.Id.ToString(),
                    Title = notification.Title,
                    Message = notification.Message,
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead,
                    Type = notification.Type,
                    BusId = notification.BusId?.ToString() ?? string.Empty,
                    RouteId = notification.RouteId?.ToString() ?? string.Empty,
                    IconName = notification.IconName,
                    ActionUrl = notification.ActionUrl
                };
                
                Notifications.Add(viewModel);
            }
            
            // 更新未读计数
            UpdateUnreadCount(unreadCount);
            
            // 显示空状态或通知列表
            EmptyStateLayout.IsVisible = !Notifications.Any();
            NotificationsCollectionView.IsVisible = Notifications.Any();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load notifications: {ex.Message}", "OK");
        }
        finally
        {
            await SetLoadingState(false);
        }
    }

    private void UpdateUnreadCount(int count)
    {
        UnreadCountLabel.Text = count switch
        {
            0 => "All caught up!",
            1 => "1 unread message",
            _ => $"{count} unread messages"
        };
    }

    private async void OnNotificationTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is NotificationViewModel notification)
        {
            // 标记为已读
            if (!notification.IsRead)
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
                notification.IsRead = true;
                await LoadNotifications(); // 刷新列表
            }
            
            // 显示详细信息
            await ShowNotificationDetails(notification);
        }
    }

    private async Task ShowNotificationDetails(NotificationViewModel notification)
    {
        var actions = new List<string> { "OK" };
        
        if (!string.IsNullOrEmpty(notification.ActionUrl))
        {
            actions.Insert(0, "Take Action");
        }
        
        if (!string.IsNullOrEmpty(notification.BusId))
        {
            actions.Insert(0, "View on Map");
        }

        var action = await DisplayActionSheet(
            notification.Title,
            "Cancel",
            null,
            actions.ToArray());

        switch (action)
        {
            case "View on Map":
                await (Shell.Current?.GoToAsync($"//main/map?busId={notification.BusId}") ?? Task.CompletedTask);
                break;
            case "Take Action":
                await HandleNotificationAction(notification);
                break;
        }
    }

    private async Task HandleNotificationAction(NotificationViewModel notification)
    {
        if (!string.IsNullOrEmpty(notification.ActionUrl))
        {
            // 在实际应用中，这里会处理不同类型的操作
            await DisplayAlert("Action", $"Action URL: {notification.ActionUrl}", "OK");
        }
    }

    private async void OnDeleteNotificationClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is NotificationViewModel notification)
        {
            var confirm = await DisplayAlert("Delete Notification", 
                "Are you sure you want to delete this notification?", "Yes", "No");
            
            if (confirm)
            {
                await _notificationService.DeleteNotificationAsync(notification.Id);
                Notifications.Remove(notification);
                
                // 检查是否需要显示空状态
                EmptyStateLayout.IsVisible = !Notifications.Any();
                NotificationsCollectionView.IsVisible = Notifications.Any();
            }
        }
    }

    private async void OnMarkAllReadClicked(object sender, EventArgs e)
    {
        try
        {
            var unreadNotifications = Notifications.Where(n => !n.IsRead).ToList();
            
            if (!unreadNotifications.Any())
            {
                await DisplayAlert("Info", "All notifications are already read", "OK");
                return;
            }

            foreach (var notification in unreadNotifications)
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
                notification.IsRead = true;
            }
            
            await LoadNotifications(); // 刷新列表
            await DisplayAlert("Success", "All notifications marked as read", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to mark notifications as read: {ex.Message}", "OK");
        }
    }

    private async void OnClearAllClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Clear All Notifications", 
            "Are you sure you want to delete all notifications? This action cannot be undone.", 
            "Yes", "No");
        
        if (confirm)
        {
            try
            {
                var notificationIds = Notifications.Select(n => n.Id).ToList();
                
                foreach (var id in notificationIds)
                {
                    await _notificationService.DeleteNotificationAsync(id);
                }
                
                Notifications.Clear();
                EmptyStateLayout.IsVisible = true;
                NotificationsCollectionView.IsVisible = false;
                
                await DisplayAlert("Success", "All notifications cleared", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to clear notifications: {ex.Message}", "OK");
            }
        }
    }

    private async void OnSendTestNotificationClicked(object sender, EventArgs e)
    {
        try
        {
            var testNotification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Test Notification",
                Message = $"This is a test notification sent at {DateTime.Now:HH:mm}",
                Type = NotificationType.General,
                CreatedAt = DateTime.Now,
                IsRead = false,
                IsActive = true,
                IconName = "test"
            };

            await _notificationService.SendLocalNotificationAsync(
                testNotification.Title, 
                testNotification.Message);
            
            // 刷新通知列表
            await LoadNotifications();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to send test notification: {ex.Message}", "OK");
        }
    }

    private async void OnNotificationReceived(object sender, NotificationEventArgs e)
    {
        // 在主线程更新UI
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await LoadNotifications();
        });
    }

    private async void OnUnreadCountChanged(object sender, UnreadCountEventArgs e)
    {
        // 在主线程更新UI
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            UpdateUnreadCount(e.Count);
        });
    }

    private async Task SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        MarkAllReadButton.IsEnabled = !isLoading;
        ClearAllButton.IsEnabled = !isLoading;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _notificationService.NotificationReceived -= OnNotificationReceived;
        _notificationService.UnreadCountChanged -= OnUnreadCountChanged;
    }
}

// 通知视图模型
public class NotificationViewModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public NotificationType Type { get; set; }
    public string BusId { get; set; }
    public string RouteId { get; set; }
    public string IconName { get; set; }
    public string ActionUrl { get; set; }

    public string TimeAgo
    {
        get
        {
            var timeSpan = DateTime.Now - CreatedAt;
            return timeSpan.TotalMinutes switch
            {
                < 1 => "Just now",
                < 60 => $"{(int)timeSpan.TotalMinutes}m ago",
                < 1440 => $"{(int)timeSpan.TotalHours}h ago",
                _ => CreatedAt.ToString("MMM dd")
            };
        }
    }

    public string IconText
    {
        get
        {
            return Type switch
            {
                NotificationType.BusAlert => "🚌",
                NotificationType.RouteUpdate => "🛣️",
                NotificationType.ServiceDisruption => "⚠️",
                NotificationType.Arrival => "📍",
                _ => "🔔"
            };
        }
    }

    public Color IconColor
    {
        get
        {
            return Type switch
            {
                NotificationType.BusAlert => Colors.Blue,
                NotificationType.RouteUpdate => Colors.Green,
                NotificationType.ServiceDisruption => Colors.Orange,
                NotificationType.Arrival => Colors.Purple,
                _ => Colors.Gray
            };
        }
    }

    public Color BackgroundColor => IsRead ? Color.FromArgb("#2d2d2d") : Color.FromArgb("#3d3d3d");
    public Color StatusColor => Colors.Red;
    public bool ShowUnreadIndicator => !IsRead;
}