using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui;

public partial class AppShell : Shell
{
    public AppShell(GlobalAudioService globalAudioService)
    {
        InitializeComponent();

        // Set the binding context to the singleton service instance
        // This will now work correctly for the mini-player Frame.
        BindingContext = globalAudioService;

        Routing.RegisterRoute(nameof(PlayerPage), typeof(PlayerPage));
        Routing.RegisterRoute(nameof(PlaylistDetailPage), typeof(PlaylistDetailPage));
        Routing.RegisterRoute(nameof(SingerProfilePage), typeof(SingerProfilePage));
        Routing.RegisterRoute(nameof(CommentsPage), typeof(CommentsPage));
        Routing.RegisterRoute(nameof(SingerCommentsPage), typeof(SingerCommentsPage));
    }
}