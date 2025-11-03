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
            var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();
            return new Window(new AppShell(authService!));
        }
    }
}