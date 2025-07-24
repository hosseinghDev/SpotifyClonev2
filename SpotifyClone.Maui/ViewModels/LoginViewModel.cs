using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly IServiceProvider _serviceProvider; // Inject the service provider

        [ObservableProperty]
        string? username;

        [ObservableProperty]
        string? password;

        public LoginViewModel(ApiService apiService, IServiceProvider serviceProvider) // <-- Add IServiceProvider
        {
            _apiService = apiService;
            _serviceProvider = serviceProvider; // <-- Initialize
            Title = "Login";
        }

        [RelayCommand]
        async Task LoginAsync()
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter both username and password.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var loginData = new { Username, Password };
                var user = await _apiService.PostAsync<User>("api/auth/login", loginData);

                if (user != null && !string.IsNullOrEmpty(user.Token))
                {
                    await SecureStorage.SetAsync("auth_token", user.Token);
                    await _apiService.SetAuthToken();
                    // CORRECTED: Get the shell from the service provider
                    Application.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Login Failed", "Invalid username or password.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(RegisterPage)}");
        }
    }
}