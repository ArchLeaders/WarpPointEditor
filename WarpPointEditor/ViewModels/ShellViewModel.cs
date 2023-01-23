using Avalonia.Controls;
using WarpPointEditor.Views;

namespace WarpPointEditor.ViewModels;

public class ShellViewModel : ReactiveObject
{
    private readonly UserControl _default = new AppView();
    private UserControl? _content;
    public UserControl? Content {
        get => _content ?? _default;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
}
