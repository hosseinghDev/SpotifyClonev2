using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.ViewModels;
using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Services
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<AudioPlayerService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<PlaylistViewModel>();
        builder.Services.AddTransient<UploadSongViewModel>();
        builder.Services.AddTransient<PlaylistDetailViewModel>();
        builder.Services.AddTransient<SingerProfileViewModel>();
        builder.Services.AddTransient<CommentsViewModel>();
        builder.Services.AddTransient<SingerCommentsViewModel>();

        // Views (Pages)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<PlaylistPage>();
        builder.Services.AddTransient<UploadSongPage>();
        builder.Services.AddTransient<PlaylistDetailPage>();
        builder.Services.AddTransient<SingerProfilePage>();
        builder.Services.AddTransient<CommentsPage>();
        builder.Services.AddTransient<SingerCommentsPage>();

        return builder.Build();
    }
}