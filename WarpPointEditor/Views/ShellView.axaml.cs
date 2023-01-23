using Avalonia.Controls;

namespace WarpPointEditor.Views;

public partial class ShellView : Window
{
    public ShellView()
    {
        InitializeComponent();
        DataContext = Shell;
    }
}
