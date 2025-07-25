using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly IServiceProvider _serviceProvider; 

        [ObservableProperty]
        string? username;

        [ObservableProperty]
        string? password;

        public RegisterViewModel(ApiService apiService, IServiceProvider serviceProvider) 
        {
            _apiService = apiService;
            _serviceProvider = serviceProvider; 
            Title = "Register";
        }

        [RelayCommand]
        async Task RegisterAsync()
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
                var registerData = new { Username, Password };
                var user = await _apiService.PostAsync<User>("api/auth/register", registerData);

                if (user != null && !string.IsNullOrEmpty(user.Token))
                {
                    await SecureStorage.SetAsync("auth_token", user.Token);
                    await _apiService.SetAuthToken();
                    //  Get the shell from the service provider
                    Application.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Registration Failed", "Username might be taken.", "OK");
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
    }
}