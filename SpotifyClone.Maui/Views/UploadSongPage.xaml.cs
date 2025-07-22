using SpotifyClone.Maui.ViewModels;
using System.Globalization;

namespace SpotifyClone.Maui.Views;

public partial class UploadSongPage : ContentPage
{
    public UploadSongPage(UploadSongViewModel vm)
    {
        InitializeComponent();
        this.Resources.Add("IsNotNullOrEmptyConverter", new IsNotNullOrEmptyConverter());
        BindingContext = vm;
    }
}

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !string.IsNullOrEmpty(value as string);
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}