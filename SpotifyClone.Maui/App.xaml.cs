using SpotifyClone.Maui.Services; 

namespace SpotifyClone.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var token = SecureStorage.GetAsync("auth_token").Result;
        if (string.IsNullOrEmpty(token))
        {
            MainPage = new AppShell_Auth();
        }
        else
        {
            // Resolve the fully constructed AppShell from the DI container
            MainPage = IPlatformApplication.Current.Services.GetRequiredService<AppShell>();
        }
    }
}