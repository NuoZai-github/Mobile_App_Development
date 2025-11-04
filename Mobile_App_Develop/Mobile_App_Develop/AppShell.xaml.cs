using Mobile_App_Develop.Views;
using Mobile_App_Develop.Services;

namespace Mobile_App_Develop
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            
            // 直接为登录/注册 ShellContent 设定页面实例，避免 DataTemplate 构造期服务解析问题
            LoginShell.ContentTemplate = null;
            LoginShell.Content = new LoginPage(_authService);
            RegisterShell.ContentTemplate = null;
            RegisterShell.Content = new RegisterPage(_authService);
            
            // 注册路由
            RegisterRoutes();
            
            // 监听认证状态变化
            _authService.UserChanged += OnUserChanged;
            
            // 设置初始导航状态
            SetInitialNavigationState();
        }

        private void RegisterRoutes()
        {
            // Shell 顶级页面已在 XAML 中通过 ShellContent 声明 Route。
            // 这里只在需要时注册非 Shell 的子页面（当前无）。
        }

        private async void OnUserChanged(object sender, EventArgs e)
        {
            await SetNavigationState();
        }

        private async void SetInitialNavigationState()
        {
            await SetNavigationState();
        }

        private async Task SetNavigationState()
        {
            var isLoggedIn = await _authService.IsLoggedInAsync();
            
            if (isLoggedIn)
            {
                // 先导航到主应用，再隐藏登录/注册，避免隐藏当前活动项导致内容丢失
                MainTabBar.IsVisible = true;
                CurrentItem = MainTabBar;
                await Current.GoToAsync("//main/dashboard");

                LoginShell.IsVisible = false;
                RegisterShell.IsVisible = false;
            }
            else
            {
                // 确保当前项是 Login，再导航，避免 Register 处于活动态却不可见
                MainTabBar.IsVisible = false;
                LoginShell.IsVisible = true;
                RegisterShell.IsVisible = true;

                CurrentItem = LoginShell;
                await Current.GoToAsync("//login");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (_authService != null)
            {
                _authService.UserChanged -= OnUserChanged;
            }
        }
    }
}
