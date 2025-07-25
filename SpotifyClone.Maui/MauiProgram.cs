using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.ViewModels;
using SpotifyClone.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace SpotifyClone.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Services
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        //builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton(AudioManager.Current);
        builder.Services.AddSingleton<GlobalAudioService>(); 

        // Tell the DI container about the AppShell
        builder.Services.AddSingleton<AppShell>(); 

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
        builder.Services.AddTransient<PlayerViewModel>();
        builder.Services.AddTransient<LibraryViewModel>();

        

        // Views (Pages)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<LibraryPage>();
        //builder.Services.AddTransient<PlaylistPage>();
        builder.Services.AddTransient<UploadSongPage>();
        builder.Services.AddTransient<PlaylistDetailPage>();
        builder.Services.AddTransient<SingerProfilePage>();
        builder.Services.AddTransient<CommentsPage>();
        builder.Services.AddTransient<SingerCommentsPage>();
        builder.Services.AddTransient<PlayerPage>();

        

        return builder.Build();
    }
}