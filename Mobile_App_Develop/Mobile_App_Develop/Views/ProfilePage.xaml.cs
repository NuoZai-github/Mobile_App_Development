using Mobile_App_Develop;
using Mobile_App_Develop.Models;
using Mobile_App_Develop.Services;

namespace Mobile_App_Develop.Views;

public partial class ProfilePage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly IBusService _busService;
    private readonly INotificationService _notificationService;
    private User _currentUser;

    public ProfilePage(IAuthService authService, IBusService busService, INotificationService notificationService)
    {
        InitializeComponent();
        _authService = authService;
        _busService = busService;
        _notificationService = notificationService;
        
        // 监听用户变更事件
        _authService.UserChanged += OnUserChanged;
    }

    // 供 XAML 实例化使用的无参构造函数
    public ProfilePage() : this(
        ServiceHelper.GetService<IAuthService>(),
        ServiceHelper.GetService<IBusService>(),
        ServiceHelper.GetService<INotificationService>())
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserProfile();
        await LoadUserStats();
    }

    private async Task LoadUserProfile()
    {
        try
        {
            await SetLoadingState(true);
            
            _currentUser = await _authService.GetCurrentUserAsync();
            
            if (_currentUser != null)
            {
                // 更新用户信息显示
                UserNameLabel.Text = $"{_currentUser.FirstName} {_currentUser.LastName}";
                UserEmailLabel.Text = _currentUser.Email;
                StudentIdLabel.Text = $"Student ID: {_currentUser.StudentId}";
                
                // 设置头像初始字母
                var initials = $"{_currentUser.FirstName?.FirstOrDefault()}{_currentUser.LastName?.FirstOrDefault()}";
                ProfileInitialsLabel.Text = initials.ToUpper();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load user profile: {ex.Message}", "OK");
        }
        finally
        {
            await SetLoadingState(false);
        }
    }

    private async Task LoadUserStats()
    {
        try
        {
            // 模拟用户统计数据
            var random = new Random();
            
            // 本周出行次数
            var tripsThisWeek = random.Next(5, 20);
            TripsThisWeekLabel.Text = tripsThisWeek.ToString();
            
            // 收藏路线数量
            var favoriteRoutes = random.Next(1, 6);
            FavoriteRoutesLabel.Text = favoriteRoutes.ToString();
            
            // 总距离
            var totalDistance = random.Next(20, 100);
            TotalDistanceLabel.Text = $"{totalDistance}km";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load user stats: {ex.Message}", "OK");
        }
    }

    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        try
        {
            if (_currentUser == null) return;

            // 显示编辑对话框
            var firstName = await DisplayPromptAsync("Edit Profile", "First Name:", 
                initialValue: _currentUser.FirstName, maxLength: 50);
            
            if (string.IsNullOrWhiteSpace(firstName)) return;

            var lastName = await DisplayPromptAsync("Edit Profile", "Last Name:", 
                initialValue: _currentUser.LastName, maxLength: 50);
            
            if (string.IsNullOrWhiteSpace(lastName)) return;

            // 更新用户信息
            var updatedUser = new User
            {
                Id = _currentUser.Id,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = _currentUser.Email,
                StudentId = _currentUser.StudentId,
                IsActive = _currentUser.IsActive,
                CreatedAt = _currentUser.CreatedAt,
                LastLoginAt = _currentUser.LastLoginAt
            };

            var success = await _authService.UpdateUserAsync(updatedUser);
            
            if (success)
            {
                await DisplayAlert("Success", "Profile updated successfully", "OK");
                await LoadUserProfile(); // 刷新显示
            }
            else
            {
                await DisplayAlert("Error", "Failed to update profile", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update profile: {ex.Message}", "OK");
        }
    }

    private async void OnFavoriteRoutesClicked(object sender, EventArgs e)
    {
        try
        {
            var routes = await _busService.GetAllRoutesAsync();
            var favoriteRoutes = routes.Take(3).ToList(); // 模拟收藏的路线
            
            var routeNames = favoriteRoutes.Select(r => r.Name).ToArray();
            
            if (routeNames.Any())
            {
                await DisplayActionSheet("Favorite Routes", "Close", null, routeNames);
            }
            else
            {
                await DisplayAlert("Favorite Routes", "You haven't added any favorite routes yet.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load favorite routes: {ex.Message}", "OK");
        }
    }

    private async void OnTripHistoryClicked(object sender, EventArgs e)
    {
        // 模拟出行历史
        var tripHistory = new[]
        {
            "Today - Route 891: Central to Ultimo",
            "Yesterday - Route 370: Coogee to City",
            "2 days ago - Route 891: Ultimo to Central",
            "3 days ago - Route 370: City to Coogee"
        };

        await DisplayActionSheet("Recent Trips", "Close", null, tripHistory);
    }

    private async void OnHelpSupportClicked(object sender, EventArgs e)
    {
        var action = await DisplayActionSheet("Help & Support", "Cancel", null,
            "FAQ", "Contact Support", "Report a Bug", "Feature Request");

        switch (action)
        {
            case "FAQ":
                await DisplayAlert("FAQ", "Frequently Asked Questions will be available in the next update.", "OK");
                break;
            case "Contact Support":
                await DisplayAlert("Contact Support", "Email: support@utsbus.edu.au\nPhone: 1800 UTS BUS", "OK");
                break;
            case "Report a Bug":
                await DisplayAlert("Report a Bug", "Please email bug reports to: bugs@utsbus.edu.au", "OK");
                break;
            case "Feature Request":
                await DisplayAlert("Feature Request", "We'd love to hear your ideas! Email: feedback@utsbus.edu.au", "OK");
                break;
        }
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("About UTS Bus Tracker", 
            "Version: 1.0.0\n" +
            "Developed by: UTS Mobile Development Team\n" +
            "© 2024 University of Technology Sydney\n\n" +
            "This app helps UTS students track campus shuttle buses in real-time.", 
            "OK");
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        try
        {
            var currentPassword = await DisplayPromptAsync("Change Password", 
                "Enter current password:", keyboard: Keyboard.Text);
            
            if (string.IsNullOrWhiteSpace(currentPassword)) return;

            var newPassword = await DisplayPromptAsync("Change Password", 
                "Enter new password (min 6 characters):", keyboard: Keyboard.Text);
            
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                await DisplayAlert("Error", "Password must be at least 6 characters long", "OK");
                return;
            }

            var confirmPassword = await DisplayPromptAsync("Change Password", 
                "Confirm new password:", keyboard: Keyboard.Text);
            
            if (newPassword != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            var success = await _authService.ChangePasswordAsync(currentPassword, newPassword);
            
            if (success)
            {
                await DisplayAlert("Success", "Password changed successfully", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to change password. Please check your current password.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to change password: {ex.Message}", "OK");
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert("Logout", 
            "Are you sure you want to logout?", "Yes", "No");
        
        if (confirm)
        {
            try
            {
                await _authService.LogoutAsync();
                // 导航将由AppShell中的用户状态变更事件处理
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to logout: {ex.Message}", "OK");
            }
        }
    }

    private async void OnUserChanged(object sender, UserChangedEventArgs e)
    {
        // 在主线程更新UI
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (e.User == null)
            {
                // 用户已登出，导航将由AppShell处理
                return;
            }
            
            // 用户信息已更新，刷新显示
            await LoadUserProfile();
        });
    }

    private async Task SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        EditProfileButton.IsEnabled = !isLoading;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _authService.UserChanged -= OnUserChanged;
    }
}