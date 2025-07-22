using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class CommentsPage : ContentPage
{
    private readonly CommentsViewModel _viewModel;
    public CommentsPage(CommentsViewModel vm)
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