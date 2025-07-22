using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class SingerProfilePage : ContentPage
{
    private readonly SingerProfileViewModel _viewModel;
    public SingerProfilePage(SingerProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadSingerDetails();
    }
}