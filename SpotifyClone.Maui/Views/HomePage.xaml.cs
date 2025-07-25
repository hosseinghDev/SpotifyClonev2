using SpotifyClone.Maui.ViewModels;
using System.Globalization;

namespace SpotifyClone.Maui.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        this.Resources.Add("BoolToHeartIconConverter", new BoolToHeartIconConverter());
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Always execute the command on appearing to get the latest songs.
        _viewModel.LoadSongsCommand.Execute(null);
    }
}

public class BoolToHeartIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "heart_filled.png" : "heart.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}