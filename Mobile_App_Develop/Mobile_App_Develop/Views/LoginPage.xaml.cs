using Mobile_App_Develop;
using Mobile_App_Develop.Services;

namespace Mobile_App_Develop.Views;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _authService;

    public LoginPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    // 供 XAML 实例化使用的无参构造函数
    public LoginPage() : this(ServiceHelper.GetService<IAuthService>())
    {
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            await ShowError("Please enter both email and password.");
            return;
        }

        await SetLoadingState(true);
        HideError();

        try
        {
            var success = await _authService.LoginAsync(EmailEntry.Text.Trim(), PasswordEntry.Text);
            
            if (success)
            {
                // 导航到主应用页面
                await (Shell.Current?.GoToAsync("//main") ?? Task.CompletedTask);
            }
            else
            {
                await ShowError("Invalid email or password. Please try again.");
            }
        }
        catch (Exception ex)
        {
            await ShowError($"Login failed: {ex.Message}");
        }
        finally
        {
            await SetLoadingState(false);
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        // 切换到 Shell 顶级的注册页面
        await Shell.Current.GoToAsync("//register");
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            await DisplayAlert("Forgot Password", "Please enter your email address first.", "OK");
            return;
        }

        var result = await DisplayAlert("Reset Password", 
            $"Send password reset instructions to {EmailEntry.Text}?", 
            "Send", "Cancel");

        if (result)
        {
            await SetLoadingState(true);
            try
            {
                var success = await _authService.ResetPasswordAsync(EmailEntry.Text.Trim());
                if (success)
                {
                    await DisplayAlert("Password Reset", 
                        "Password reset instructions have been sent to your email.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", 
                        "Email address not found. Please check and try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to send reset email: {ex.Message}", "OK");
            }
            finally
            {
                await SetLoadingState(false);
            }
        }
    }

    private async Task SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        LoginButton.IsEnabled = !isLoading;
        EmailEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;

        if (isLoading)
        {
            LoginButton.Text = "Signing In...";
        }
        else
        {
            LoginButton.Text = "Sign In";
        }
    }

    private async Task ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
        
        // 自动隐藏错误消息
        await Task.Delay(5000);
        if (ErrorLabel.Text == message) // 确保没有新的错误消息
        {
            HideError();
        }
    }

    private void HideError()
    {
        ErrorLabel.IsVisible = false;
        ErrorLabel.Text = string.Empty;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // 检查是否已经登录
        if (await _authService.IsLoggedInAsync())
        {
            await (Shell.Current?.GoToAsync("//main") ?? Task.CompletedTask);
        }
        
        // 清空输入框
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        HideError();

        await this.FadeTo(0, 0);
        await this.FadeTo(1, 350);
        await LoginButton.ScaleTo(1.02, 200);
        await LoginButton.ScaleTo(1, 120);
    }

    private void OnTogglePasswordVisibility(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
    }
}
