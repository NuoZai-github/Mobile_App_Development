using Mobile_App_Develop.Services;

namespace Mobile_App_Develop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // 直接注入 IAuthService 构造 Shell，缩短解析链便于定位问题
            var auth = ServiceHelper.GetService<IAuthService>();
            return new Window(new AppShell(auth));
        }
    }
}