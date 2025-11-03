using Mobile_App_Develop;
using Mobile_App_Develop.Models;
using Mobile_App_Develop.Services;
using System.Collections.ObjectModel;

namespace Mobile_App_Develop.Views;

public partial class DashboardPage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly IBusService _busService;
    private readonly INotificationService _notificationService;
    
    public ObservableCollection<BusViewModel> Buses { get; set; }

    public DashboardPage(IAuthService authService, IBusService busService, INotificationService notificationService)
    {
        InitializeComponent();
        _authService = authService;
        _busService = busService;
        _notificationService = notificationService;
        
        Buses = new ObservableCollection<BusViewModel>();
        BusesCollectionView.ItemsSource = Buses;
        
        // 监听巴士位置更新
        _busService.BusLocationUpdated += OnBusLocationUpdated;
        _busService.BusStatusChanged += OnBusStatusChanged;
    }

    // 供 XAML 实例化使用的无参构造函数
    public DashboardPage() : this(
        ServiceHelper.GetService<IAuthService>(),
        ServiceHelper.GetService<IBusService>(),
        ServiceHelper.GetService<INotificationService>())
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserInfo();
        await LoadDashboardData();
    }

    private async Task LoadUserInfo()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                UserNameLabel.Text = $"{user.FirstName} {user.LastName}";
                WelcomeLabel.Text = GetWelcomeMessage();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load user info: {ex.Message}", "OK");
        }
    }

    private string GetWelcomeMessage()
    {
        var hour = DateTime.Now.Hour;
        return hour switch
        {
            >= 5 and < 12 => "Good Morning!",
            >= 12 and < 17 => "Good Afternoon!",
            >= 17 and < 22 => "Good Evening!",
            _ => "Good Night!"
        };
    }

    private async Task LoadDashboardData()
    {
        try
        {
            await SetLoadingState(true);
            
            // 加载巴士数据
            var buses = await _busService.GetAllBusesAsync();
            var routes = await _busService.GetAllRoutesAsync();
            
            // 更新统计信息
            var activeBuses = buses.Where(b => b.IsActive).ToList();
            ActiveBusesLabel.Text = activeBuses.Count.ToString();
            TotalRoutesLabel.Text = routes.Count().ToString();
            
            // 更新巴士列表
            Buses.Clear();
            foreach (var bus in activeBuses.OrderBy(b => b.BusNumber))
            {
                var route = routes.FirstOrDefault(r => r.Id == bus.RouteId);
                var busViewModel = new BusViewModel
                {
                    Id = bus.Id.ToString(),
                    BusNumber = bus.BusNumber,
                    RouteName = route?.Name ?? "Unknown Route",
                    Status = bus.Status.ToString(),
                    StatusColor = GetStatusColor(bus.Status),
                    NextStop = bus.NextStop ?? "No stops",
                    EstimatedArrival = bus.EstimatedArrival ?? DateTime.Now,
                    PassengerCount = bus.CurrentPassengers,
                    Latitude = bus.Latitude,
                    Longitude = bus.Longitude
                };
                Buses.Add(busViewModel);
            }
            
            // 显示空状态或列表
            EmptyStateLayout.IsVisible = !Buses.Any();
            BusesCollectionView.IsVisible = Buses.Any();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load dashboard data: {ex.Message}", "OK");
        }
        finally
        {
            await SetLoadingState(false);
        }
    }

    private Color GetStatusColor(BusStatus status)
    {
        return status switch
        {
            BusStatus.InService => Colors.Green,
            BusStatus.OutOfService => Colors.Red,
            BusStatus.Maintenance => Colors.Orange,
            BusStatus.Delayed => Colors.Yellow,
            _ => Colors.Gray
        };
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadDashboardData();
        
        // 显示刷新成功消息
        await _notificationService.SendLocalNotificationAsync(
            "Dashboard Updated", 
            "Bus information has been refreshed successfully.");
    }

    private async void OnBusItemTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is BusViewModel bus)
        {
            var action = await DisplayActionSheet(
                $"Bus {bus.BusNumber}", 
                "Cancel", 
                null, 
                "View on Map", 
                "Get Notifications", 
                "View Route Details");

            switch (action)
            {
                case "View on Map":
                    await Shell.Current.GoToAsync($"//main/map?busId={bus.Id}");
                    break;
                case "Get Notifications":
                    await SetupBusNotifications(bus);
                    break;
                case "View Route Details":
                    await ShowRouteDetails(bus);
                    break;
            }
        }
    }

    private async Task SetupBusNotifications(BusViewModel bus)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Bus {bus.BusNumber} Alert",
            Message = $"You will receive notifications for bus {bus.BusNumber} on {bus.RouteName}",
            Type = NotificationType.BusAlert,
            BusId = int.Parse(bus.Id),
            CreatedAt = DateTime.Now,
            IsRead = false,
            IsActive = true
        };

        await _notificationService.SendLocalNotificationAsync(notification.Title, notification.Message);
        await DisplayAlert("Notifications Enabled", 
            $"You will now receive updates for Bus {bus.BusNumber}", "OK");
    }

    private async Task ShowRouteDetails(BusViewModel bus)
    {
        try
        {
            var routes = await _busService.GetAllRoutesAsync();
            var route = routes.FirstOrDefault(r => r.Name == bus.RouteName);
            
            if (route != null)
            {
                var stops = string.Join("\n", route.Stops.Select(s => $"• {s.Name}"));
                await DisplayAlert($"Route: {route.Name}", 
                    $"Description: {route.Description}\n\nStops:\n{stops}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load route details: {ex.Message}", "OK");
        }
    }

    private async void OnBusLocationUpdated(object sender, BusLocationEventArgs e)
    {
        // 在主线程更新UI
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var busViewModel = Buses.FirstOrDefault(b => b.Id == e.BusId.ToString());
            if (busViewModel != null)
            {
                busViewModel.Latitude = e.Latitude;
                busViewModel.Longitude = e.Longitude;
            }
        });
    }

    private async void OnBusStatusChanged(object sender, BusStatusEventArgs e)
    {
        // 在主线程更新UI
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            var busViewModel = Buses.FirstOrDefault(b => b.Id == e.BusId.ToString());
            if (busViewModel != null)
            {
                busViewModel.Status = e.Status.ToString();
                busViewModel.StatusColor = GetStatusColor(e.Status);
            }
        });
    }

    private async Task SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RefreshButton.IsEnabled = !isLoading;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _busService.BusLocationUpdated -= OnBusLocationUpdated;
        _busService.BusStatusChanged -= OnBusStatusChanged;
    }
}

// 巴士视图模型
public class BusViewModel
{
    public string Id { get; set; }
    public string BusNumber { get; set; }
    public string RouteName { get; set; }
    public string Status { get; set; }
    public Color StatusColor { get; set; }
    public string NextStop { get; set; }
    public DateTime EstimatedArrival { get; set; }
    public int PassengerCount { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}