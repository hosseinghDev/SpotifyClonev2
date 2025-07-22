namespace SpotifyClone.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Check if user is logged in
        var token = SecureStorage.GetAsync("auth_token").Result;
        if (string.IsNullOrEmpty(token))
        {
            MainPage = new AppShell_Auth();
        }
        else
        {
            MainPage = new AppShell();
        }
    }
}