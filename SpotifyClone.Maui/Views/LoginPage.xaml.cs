using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}