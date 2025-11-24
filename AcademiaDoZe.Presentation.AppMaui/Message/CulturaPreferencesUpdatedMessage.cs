using CommunityToolkit.Mvvm.Messaging.Messages;
namespace AcademiaDoZe.Presentation.AppMaui.Message
{
    public sealed class CulturaPreferencesUpdatedMessage(string value) : ValueChangedMessage<string>(value)
    {
    }
}