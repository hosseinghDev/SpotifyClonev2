using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Set the binding context to the instance from the DI container
        BindingContext = IPlatformApplication.Current.Services.GetService<GlobalAudioService>();

        Routing.RegisterRoute(nameof(PlayerPage), typeof(PlayerPage));
        Routing.RegisterRoute(nameof(PlaylistDetailPage), typeof(PlaylistDetailPage));
        Routing.RegisterRoute(nameof(SingerProfilePage), typeof(SingerProfilePage));
        Routing.RegisterRoute(nameof(CommentsPage), typeof(CommentsPage));
        Routing.RegisterRoute(nameof(SingerCommentsPage), typeof(SingerCommentsPage));
    }
}