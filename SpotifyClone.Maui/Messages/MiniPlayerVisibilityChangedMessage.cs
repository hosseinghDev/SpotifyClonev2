using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SpotifyClone.Maui.Messages
{
    public class MiniPlayerVisibilityChangedMessage : ValueChangedMessage<bool>
    {
        public MiniPlayerVisibilityChangedMessage(bool isVisible) : base(isVisible)
        {
        }
    }
}