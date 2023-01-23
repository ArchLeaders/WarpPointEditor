using Dock.Model.ReactiveUI.Controls;

namespace WarpPointEditor.ViewModels;

public class SettingsViewModel : Document
{
	public SettingsViewModel()
	{
		Id = nameof(SettingsViewModel);
		Title = "Settings";
		CanFloat = false;
	}
}
