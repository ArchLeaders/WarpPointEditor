using Dock.Model.ReactiveUI.Controls;
using WarpPointEditor.Core.Extensions;

namespace WarpPointEditor.ViewModels
{
    public class HomeViewModel : Document
    {
        public HomeViewModel()
        {
            Title = "Home";
            CanFloat = false;
            CanClose = false;
            CanPin = false;
        }

        public static async Task VersionLink()
        {
            await BrowserExtension.OpenUrl("https://github.com/ArchLeaders/WarpPointEditor/releases/");
        }
    }
}
