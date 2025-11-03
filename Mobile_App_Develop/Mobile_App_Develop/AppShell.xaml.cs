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
                // 用户已登录，显示主应用导航
                MainTabBar.IsVisible = true;
                LoginShell.IsVisible = false;
                RegisterShell.IsVisible = false;
                
                // 导航到Dashboard
                await Current.GoToAsync("//main/dashboard");
            }
            else
            {
                // 用户未登录，显示登录页面
                MainTabBar.IsVisible = false;
                LoginShell.IsVisible = true;
                RegisterShell.IsVisible = true;
                
                // 导航到登录页面
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
