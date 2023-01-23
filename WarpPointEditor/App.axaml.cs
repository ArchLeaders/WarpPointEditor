global using static WarpPointEditor.App;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using WarpPointEditor.Models;
using WarpPointEditor.ViewModels;
using WarpPointEditor.Views;

namespace WarpPointEditor
{
    public partial class App : Application
    {
        public static ShellViewModel Shell { get; set; } = new();
        public static FluentTheme Theme { get; set; } = new(new Uri("avares://BotwActorTool/Styles"));

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = WindowBuilder.Initialize(new ShellView())
                    .WithMenu(new ShellMenuModel())
                    .WithWindowColors("SystemChromeLowColor", "SystemChromeHighColor", 0.4)
                    .WithMinBounds(800, 450)
                    .Build();

#if DEBUG
                desktop.MainWindow.AttachDevTools();
#endif
                ApplicationLoader.Attach(this);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
