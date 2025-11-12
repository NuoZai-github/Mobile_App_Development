using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Mobile_App_Develop.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, __) => await NavigateAsync();
        Root.GestureRecognizers.Add(tap);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Logo.FadeTo(1, 600);
        await Logo.ScaleTo(1.05, 500, Easing.CubicOut);
        await Handle.TranslateTo(0, -8, 500);
        await Handle.FadeTo(1, 400);
        await Task.Delay(1500);
        await NavigateAsync();
    }

    Task NavigateAsync() => Shell.Current.GoToAsync("login", true);
}
