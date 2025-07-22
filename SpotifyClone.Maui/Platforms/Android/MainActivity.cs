using Android.App;
using Android.Content.PM;

namespace SpotifyClone.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, UsesCleartextTraffic = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}