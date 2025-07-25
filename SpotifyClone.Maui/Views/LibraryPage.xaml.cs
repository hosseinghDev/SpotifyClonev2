using SpotifyClone.Maui.ViewModels;
using System.Globalization;

namespace SpotifyClone.Maui.Views;

public partial class LibraryPage : ContentPage
{
    private readonly LibraryViewModel _viewModel;
    public LibraryPage(LibraryViewModel vm)
    {
        InitializeComponent();
        this.Resources.Add("IntToBoolConverter", new IntToBoolConverter());
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Load data every time the page appears to get the latest updates
        _viewModel.LoadLibraryDataCommand.Execute(null);
    }
}


public class IntToBoolConverter : IValueConverter
{
    // This will return true (visible) if the count is 0, and false (hidden) otherwise.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value == 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}