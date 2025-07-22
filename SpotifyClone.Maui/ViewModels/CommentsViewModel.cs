using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using System.Collections.ObjectModel;

namespace SpotifyClone.Maui.ViewModels
{
    [QueryProperty(nameof(SongId), "songId")]
    public partial class CommentsViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] int songId;
        [ObservableProperty] ObservableCollection<Comment> comments;
        [ObservableProperty] string newCommentText;

        public CommentsViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Comments";
            Comments = new ObservableCollection<Comment>();
        }

        public async Task LoadComments()
        {
            if (IsBusy || SongId == 0) return;
            IsBusy = true;
            try
            {
                var result = await _apiService.GetAsync<List<Comment>>($"api/comments/song/{SongId}");
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
                var result = await _apiService.PostAsync<Comment>($"api/comments/song/{SongId}", new { Text = NewCommentText });
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