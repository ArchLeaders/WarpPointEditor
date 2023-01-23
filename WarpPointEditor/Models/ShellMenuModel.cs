using Avalonia.Controls;
using Avalonia.MenuFactory.Attributes;
using Material.Icons;
using WarpPointEditor.Views;

namespace WarpPointEditor.Models;

public class ShellMenuModel
{
    [Menu("Settings", "_Tools", Icon = MaterialIconKind.CogBox)]
    public static void Settings()
    {

    }
}
