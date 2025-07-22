using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class SingerCommentsPage : ContentPage
{
    private readonly SingerCommentsViewModel _viewModel;
    public SingerCommentsPage(SingerCommentsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadComments();
    }
}