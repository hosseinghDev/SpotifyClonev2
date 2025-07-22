using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using System.Collections.ObjectModel;

namespace SpotifyClone.Maui.ViewModels
{
    [QueryProperty(nameof(SingerId), "singerId")]
    public partial class SingerCommentsViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] int singerId;
        [ObservableProperty] ObservableCollection<Comment> comments;
        [ObservableProperty] string newCommentText;

        public SingerCommentsViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Artist Comments";
            Comments = new ObservableCollection<Comment>();
        }

        public async Task LoadComments()
        {
            if (IsBusy || SingerId == 0) return;
            IsBusy = true;
            try
            {
                var result = await _apiService.GetAsync<List<Comment>>($"api/comments/singer/{SingerId}");
                if (result != null)
                {
                    Comments.Clear();
                    foreach (var comment in result) Comments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not load comments: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task AddComment()
        {
            if (string.IsNullOrWhiteSpace(NewCommentText)) return;
            IsBusy = true;
            try
            {
                await _apiService.SetAuthToken();
                var result = await _apiService.PostAsync<Comment>($"api/comments/singer/{SingerId}", new { Text = NewCommentText });
                if (result != null)
                {
                    Comments.Insert(0, result);
                    NewCommentText = string.Empty;
                }
                else await Shell.Current.DisplayAlert("Error", "Could not post comment. Please log in.", "OK");
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