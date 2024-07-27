using MudBlazor;

namespace SafeShare.Client.Internal.Helpers;
public static class DialogHelper
{
    public static DialogOptions
    DialogOptions()
    {
        return new()
        {
            ClassBackground = "my-custom-class",
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            CloseButton = true,
            Position = DialogPosition.Center
        };
    }

    public static DialogOptions
    DialogOptionsNoCloseButton()
    {
        return new()
        {
            ClassBackground = "my-custom-class",
            CloseOnEscapeKey = false,
            DisableBackdropClick = true,
            CloseButton = false,
            Position = DialogPosition.Center
        };
    }
}