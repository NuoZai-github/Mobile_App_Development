using Mobile_App_Develop;
using Mobile_App_Develop.Models;
using Mobile_App_Develop.Services;

namespace Mobile_App_Develop.Views;

[QueryProperty(nameof(BusId), "busId")]
public partial class MapPage : ContentPage
{
    private readonly IBusService _busService;
    private readonly INotificationService _notificationService;
    private List<Bus> _buses = new();
    private string _selectedBusId;
    
    public string BusId { get; set; }

    public MapPage(IBusService busService, INotificationService notificationService)
    {
        InitializeComponent();
        _busService = busService;
        _notificationService = notificationService;
        
        // 监听巴士位置更新
        _busService.BusLocationUpdated += OnBusLocationUpdated;
        _busService.BusStatusChanged += OnBusStatusChanged;
    }

    // 供 XAML 实例化使用的无参构造函数
    public MapPage() : this(
        ServiceHelper.GetService<IBusService>(),
        ServiceHelper.GetService<INotificationService>())
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMapData();
        
        // 如果有指定的巴士ID，则聚焦到该巴士
        if (!string.IsNullOrEmpty(BusId))
        {
            await FocusOnBus(BusId);
        }
    }

    private async Task LoadMapData()
    {
        try
        {
            await SetLoadingState(true);
            
            // 加载巴士数据
            var allBuses = await _busService.GetAllBusesAsync();
            _buses = allBuses.Where(b => b.IsActive).ToList();
            
            // 更新地图上的巴士标记
            await UpdateBusMarkers();
            
            // 更新底部信息
            UpdateBottomInfo();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load map data: {ex.Message}", "OK");
        }
        finally
        {
            await SetLoadingState(false);
        }
    }

    private async Task UpdateBusMarkers()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            // 清除现有标记
            BusMarkersContainer.Children.Clear();
            
            // 为每个活跃巴士创建标记
            foreach (var bus in _buses)
            {
                var busMarker = CreateBusMarker(bus);
                
                // 计算标记位置 (模拟坐标转换)
                var position = ConvertToMapPosition(bus.Latitude, bus.Longitude);
                // Note: StackLayout doesn't support absolute positioning
                // In a real implementation, you would use AbsoluteLayout or a proper map control
                
                BusMarkersContainer.Children.Add(busMarker);
            }
        });
    }

    private Border CreateBusMarker(Bus bus)
    {
        var markerColor = GetBusMarkerColor(bus.Status);
        
        var marker = new Border
        {
            BackgroundColor = markerColor,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0,
            Shadow = new Shadow { Brush = new SolidColorBrush(Colors.Gray), Offset = new Point(2, 2), Radius = 4, Opacity = 0.3f },
            Content = new Label
            {
                Text = "🚌",
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        // 添加点击手势
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (sender, e) => await OnBusMarkerTapped(bus);
        marker.GestureRecognizers.Add(tapGesture);

        return marker;
    }

    private Color GetBusMarkerColor(BusStatus status)
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

    private Point ConvertToMapPosition(double latitude, double longitude)
    {
        // 模拟坐标转换 - 在实际应用中，这里会使用真实的地图投影
        // 这里使用简单的线性映射来模拟巴士在地图上的位置
        
        // 假设地图区域为 300x300 像素
        var mapWidth = 300.0;
        var mapHeight = 300.0;
        
        // 模拟UTS校园坐标范围
        var minLat = -33.8850;
        var maxLat = -33.8800;
        var minLng = 151.1950;
        var maxLng = 151.2000;
        
        // 线性映射到地图像素坐标
        var x = ((longitude - minLng) / (maxLng - minLng)) * mapWidth + 50;
        var y = ((latitude - minLat) / (maxLat - minLat)) * mapHeight + 75;
        
        // 确保坐标在地图范围内
        x = Math.Max(50, Math.Min(350, x));
        y = Math.Max(75, Math.Min(375, y));
        
        return new Point(x, y);
    }

    private async Task OnBusMarkerTapped(Bus bus)
    {
        _selectedBusId = bus.Id.ToString();
        
        // 更新底部信息显示
        var route = (await _busService.GetAllRoutesAsync()).FirstOrDefault(r => r.Id == bus.RouteId);
        
        SelectedBusLabel.Text = $"Bus {bus.BusNumber} - {route?.Name ?? "Unknown Route"}";
        BusDetailsLabel.Text = $"Status: {bus.Status} | Next Stop: {bus.NextStop ?? "N/A"} | Passengers: {bus.CurrentPassengers}";
        
        // 显示详细信息弹窗
        var action = await DisplayActionSheet(
            $"Bus {bus.BusNumber}", 
            "Cancel", 
            null, 
            "Get Arrival Time", 
            "Set Notification", 
            "View Route Details");

        switch (action)
        {
            case "Get Arrival Time":
                await ShowArrivalTime(bus);
                break;
            case "Set Notification":
                await SetBusNotification(bus);
                break;
            case "View Route Details":
                await ShowRouteDetails(bus, route);
                break;
        }
    }

    private async Task ShowArrivalTime(Bus bus)
    {
        try
        {
            var estimatedTime = bus.EstimatedArrival ?? DateTime.Now.AddMinutes(5);
            var timeString = estimatedTime.ToString("HH:mm");
            
            await DisplayAlert("Arrival Time", 
                $"Bus {bus.BusNumber} is estimated to arrive at {timeString}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to get arrival time: {ex.Message}", "OK");
        }
    }

    private async Task SetBusNotification(Bus bus)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Bus {bus.BusNumber} Notification",
            Message = $"You will receive updates for Bus {bus.BusNumber}",
            Type = NotificationType.BusAlert,
            BusId = bus.Id,
            CreatedAt = DateTime.Now,
            IsRead = false,
            IsActive = true
        };

        await _notificationService.SendLocalNotificationAsync(notification.Title, notification.Message);
        await DisplayAlert("Notification Set", 
            $"You will receive updates for Bus {bus.BusNumber}", "OK");
    }

    private async Task ShowRouteDetails(Bus bus, Route route)
    {
        if (route != null)
        {
            var stops = string.Join("\n", route.Stops.Select(s => $"• {s.Name}"));
            await DisplayAlert($"Route: {route.Name}", 
                $"Description: {route.Description}\n\nBus Stops:\n{stops}", "OK");
        }
        else
        {
            await DisplayAlert("Route Details", "Route information not available", "OK");
        }
    }

    private async Task FocusOnBus(string busId)
    {
        var bus = _buses.FirstOrDefault(b => b.Id.ToString() == busId);
        if (bus != null)
        {
            await OnBusMarkerTapped(bus);
        }
    }

    private void UpdateBottomInfo()
    {
        if (string.IsNullOrEmpty(_selectedBusId))
        {
            SelectedBusLabel.Text = $"Tracking {_buses.Count} active buses";
            BusDetailsLabel.Text = "Tap on a bus marker for more information";
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadMapData();
        await _notificationService.SendLocalNotificationAsync(
            "Map Updated", 
            "Bus locations have been refreshed");
    }

    private async void OnCenterMapClicked(object sender, EventArgs e)
    {
        // 模拟地图居中功能
        await DisplayAlert("Map Centered", 
            "Map has been centered on UTS campus", "OK");
    }

    private async void OnViewListClicked(object sender, EventArgs e)
    {
        // 导航到Dashboard页面
        await Shell.Current.GoToAsync("//main/dashboard");
    }

    private async void OnBusLocationUpdated(object sender, BusLocationEventArgs e)
    {
        // 更新对应巴士的位置
        var bus = _buses.FirstOrDefault(b => b.Id == e.BusId);
        if (bus != null)
        {
            bus.Latitude = e.Latitude;
            bus.Longitude = e.Longitude;
            await UpdateBusMarkers();
        }
    }

    private async void OnBusStatusChanged(object sender, BusStatusEventArgs e)
    {
        // 更新对应巴士的状态
        var bus = _buses.FirstOrDefault(b => b.Id == e.BusId);
        if (bus != null)
        {
            bus.Status = e.Status;
            await UpdateBusMarkers();
        }
    }

    private async Task SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RefreshButton.IsEnabled = !isLoading;
        CenterButton.IsEnabled = !isLoading;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _busService.BusLocationUpdated -= OnBusLocationUpdated;
        _busService.BusStatusChanged -= OnBusStatusChanged;
    }
}