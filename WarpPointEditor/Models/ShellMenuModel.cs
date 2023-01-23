using Avalonia.Controls;
using Avalonia.Generics.Dialogs;
using Avalonia.MenuFactory.Attributes;
using Material.Icons;
using System.Diagnostics;
using WarpPointEditor.Core.Extensions;
using WarpPointEditor.ViewModels;

namespace WarpPointEditor.Models;

public class ShellMenuModel
{
    //
    // File

    [Menu("Open Mod", "_File", Icon = MaterialIconKind.FolderOpen, HotKey = "Ctrl + O")]
    public static void OpenMod()
    {

    }

    [Menu("New Mod", "_File", Icon = MaterialIconKind.CreateNewFolder, HotKey = "Ctrl + N")]
    public static void NewMod()
    {

    }

    [Menu("Save Mod", "_File", Icon = MaterialIconKind.FloppyDisc, HotKey = "Ctrl + S", IsSeparator = true)]
    public static void SaveMod()
    {

    }

    [Menu("Quit", "_File", Icon = MaterialIconKind.ExitToApp, IsSeparator = true)]
    public static async Task Quit()
    {
        if (DockFactory.Root?.VisibleDockables?.Count > 1) {
            if (await MessageBox.ShowDialog("You may have unsaved changes. Are you sure you wish to exit?", "Warning", DialogButtons.YesNo) != DialogResult.Yes) {
                return;
            }
        }

        Environment.Exit(0);
    }

    //
    // Tools

    [Menu("Settings", "_Tools", Icon = MaterialIconKind.CogBox)]
    public static void Settings()
    {
        DockFactory.AddDocument(new SettingsViewModel());
    }

    // 
    // About

    [Menu("Wiki", "_About", Icon = MaterialIconKind.HelpOutline)]
    public static async Task Help()
    {
        await BrowserExtension.OpenUrl("https://github.com/ArchLeaders/WarpPointEditor/wiki");
    }

    [Menu("Discord", "_About", Icon = MaterialIconKind.LinkVariant)]
    public static async Task Discord()
    {
        await BrowserExtension.OpenUrl("https://discord.gg/8Saj6tTkNB");
    }

    [Menu("Report Issue", "_About", Icon = MaterialIconKind.Bug, IsSeparator = true)]
    public static async Task ReportIssue()
    {
        await BrowserExtension.OpenUrl("https://github.com/ArchLeaders/WarpPointEditor/issues/new");
    }
}
