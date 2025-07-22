using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PlaylistDetailPage), typeof(PlaylistDetailPage));
        Routing.RegisterRoute(nameof(SingerProfilePage), typeof(SingerProfilePage));
        Routing.RegisterRoute(nameof(CommentsPage), typeof(CommentsPage));
        Routing.RegisterRoute(nameof(SingerCommentsPage), typeof(SingerCommentsPage));
    }
}