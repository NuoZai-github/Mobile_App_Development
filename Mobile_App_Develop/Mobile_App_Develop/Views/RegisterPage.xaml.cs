using Mobile_App_Develop;
using Mobile_App_Develop.Models;
using Mobile_App_Develop.Services;
using System.Text.RegularExpressions;

namespace Mobile_App_Develop.Views;

public partial class RegisterPage : ContentPage
{
    private readonly IAuthService _authService;

    public RegisterPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    // 供 XAML 实例化使用的无参构造函数
    public RegisterPage() : this(ServiceHelper.GetService<IAuthService>())
    {
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        await SetLoadingState(true);
        HideError();

        try
        {
            var user = new User
            {
                FirstName = FirstNameEntry.Text.Trim(),
                LastName = LastNameEntry.Text.Trim(),
                StudentId = StudentIdEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim().ToLower(),
                Password = PasswordEntry.Text,
                AvatarUrl = "https://via.placeholder.com/150"
            };

            var success = await _authService.RegisterAsync(user);
            
            if (success)
            {
                await DisplayAlert("Success", 
                    "Account created successfully! You are now logged in.", "OK");
                
                // 导航到主应用页面
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                await ShowError("Registration failed. Email address may already be in use.");
            }
        }
        catch (Exception ex)
        {
            await ShowError($"Registration failed: {ex.Message}");
        }
        finally
        {
            await SetLoadingState(false);
        }
    }

    private bool ValidateForm()
    {
        // 检查必填字段
        if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
        {
            ShowError("Please enter your first name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
        {
            ShowError("Please enter your last name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(StudentIdEntry.Text))
        {
            ShowError("Please enter your student ID.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(EmailEntry.Text))
        {
            ShowError("Please enter your email address.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
        {
            ShowError("Please enter a password.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
        {
            ShowError("Please confirm your password.");
            return false;
        }

        // 验证邮箱格式
        if (!IsValidEmail(EmailEntry.Text.Trim()))
        {
            ShowError("Please enter a valid email address.");
            return false;
        }

        // 验证学号格式（假设8位数字）
        if (!IsValidStudentId(StudentIdEntry.Text.Trim()))
        {
            ShowError("Student ID must be 8 digits.");
            return false;
        }

        // 验证密码强度
        if (PasswordEntry.Text.Length < 6)
        {
            ShowError("Password must be at least 6 characters long.");
            return false;
        }

        // 验证密码确认
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            ShowError("Passwords do not match.");
            return false;
        }

        // 检查条款同意
        if (!TermsCheckBox.IsChecked)
        {
            ShowError("Please agree to the Terms of Service and Privacy Policy.");
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidStudentId(string studentId)
    {
        return studentId.Length == 8 && studentId.All(char.IsDigit);
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await (Shell.Current?.GoToAsync("..") ?? Task.CompletedTask);
    }

    private async void OnTermsTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Terms of Service", 
            "Please read our terms of service carefully before using the UTS Bus Tracker application.", "OK");
    }

    private async void OnPrivacyTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Privacy Policy", 
            "Your privacy is important to us. Please review our privacy policy to understand how we handle your data.", "OK");
    }

    private async Task SetLoadingState(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        RegisterButton.IsEnabled = !isLoading;
        
        // 禁用所有输入字段
        FirstNameEntry.IsEnabled = !isLoading;
        LastNameEntry.IsEnabled = !isLoading;
        StudentIdEntry.IsEnabled = !isLoading;
        EmailEntry.IsEnabled = !isLoading;
        PasswordEntry.IsEnabled = !isLoading;
        ConfirmPasswordEntry.IsEnabled = !isLoading;
        TermsCheckBox.IsEnabled = !isLoading;

        if (isLoading)
        {
            RegisterButton.Text = "Creating Account...";
        }
        else
        {
            RegisterButton.Text = "Create Account";
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // 清空所有输入框
        FirstNameEntry.Text = string.Empty;
        LastNameEntry.Text = string.Empty;
        StudentIdEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        ConfirmPasswordEntry.Text = string.Empty;
        TermsCheckBox.IsChecked = false;
        HideError();
    }
}