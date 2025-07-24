using SpotifyClone.Maui.ViewModels;

namespace SpotifyClone.Maui.Views;

public partial class PlayerPage : ContentPage
{
    private readonly PlayerViewModel _viewModel;

    public PlayerPage(PlayerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Cleanup();
    }

    private void PositionSlider_DragCompleted(object sender, EventArgs e)
    {
        if (sender is Slider slider)
        {
            _viewModel.SeekCommand.Execute(slider.Value);
        }
    }
}