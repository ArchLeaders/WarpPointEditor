using Avalonia;
using Avalonia.Generics.Dialogs;
using Avalonia.SettingsFactory;
using Avalonia.SettingsFactory.Core;
using Avalonia.SettingsFactory.ViewModels;
using Avalonia.Themes.Fluent;
using System.Reflection;

namespace WarpPointEditor.Views
{
    public partial class SettingsView : SettingsFactory, ISettingsValidator
    {
        private static readonly SettingsFactoryOptions _options = new() {
            AlertAction = (msg) => MessageBox.ShowDialog(msg),
            BrowseAction = async (title) => await new BrowserDialog(BrowserMode.OpenFolder).ShowDialog(),
        };

        public SettingsView() => InitializeComponent();
        public SettingsView(bool canCancel)
        {
            InitializeComponent();

            FocusDelegate.PointerPressed += (s, e) => FocusDelegate.Focus();
            FocusDelegate2.PointerPressed += (s, e) => FocusDelegate.Focus();

            AfterSaveEvent += () => {
                Config.Save();
                Shell.Content = null;
            };

            AfterCancelEvent += () => {
                ValidateSave();
                Shell.Content = null;
            };

            InitializeSettingsFactory(new SettingsFactoryViewModel(canCancel), this, Config, _options);
        }

        public bool? ValidateBool(string key, bool value)
        {
            return key switch {
                _ => null
            };
        }

        public bool? ValidateString(string key, string? value)
        {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }

            return key switch {
                "GameDir" => File.Exists($"{value}/Pack/Dungeon000.pack") && value.EndsWith("content"),
                "UpdateDir" => File.Exists($"{value}/Actor/Pack/ActorObserverByActorTagTag.sbactorpack") && value.EndsWith("content"),
                "DlcDir" => File.Exists($"{value}/Pack/AocMainField.pack") && value.EndsWith(Path.Combine("content", "0010")),
                "GameDirNx" => File.Exists($"{value}/Actor/Pack/ActorObserverByActorTagTag.sbactorpack") && File.Exists($"{value}/Pack/Dungeon000.pack") && value.EndsWith("romfs"),
                "DlcDirNx" => File.Exists($"{value}/Pack/AocMainField.pack") && value.EndsWith("romfs"),
                "Lang" => File.Exists($"{this["GameDir"]}/Pack/Bootup_{value}.pack") || File.Exists($"{this["GameDirNx"]}/Pack/Bootup_{value}.pack"),
                "Mode" => (value == "WiiU" && ValidateString("GameDir", this["GameDir"] as string) != (false | null) && ValidateString("UpdateDir", this["UpdateDir"] as string) != (false | null)) || (value == "Switch" && ValidateString("GameDirNx", this["GameDirNx"] as string) != (false | null)),
                "Theme" => ValidateTheme(value!),
                _ => null,
            };
        }

        public static bool? ValidateTheme(string value)
        {
            App.Theme.Mode = value == "Dark" ? FluentThemeMode.Dark : FluentThemeMode.Light;
            Application.Current!.Styles[0] = App.Theme;
            return null;
        }

        public string? ValidateSave()
        {
            Dictionary<string, bool?> validated = new();
            foreach (var prop in Config.GetType().GetProperties().Where(x => x.GetCustomAttributes<SettingAttribute>(false).Any())) {
                object? value = prop.GetValue(Config);

                if (value is bool boolean) {
                    validated.Add(prop.Name, ValidateBool(prop.Name, boolean));
                }
                else {
                    validated.Add(prop.Name, ValidateString(prop.Name, value as string));
                }
            }

            return ValidateSave(validated);
        }

        public string? ValidateSave(Dictionary<string, bool?> validated)
        {
            if (validated["GameDir"] == false) {
                return "The WiiU game path is invalid.\nPlease correct or delete it before saving.";
            }

            if (validated["UpdateDir"] == false) {
                return "The WiiU update path is invalid.\nPlease correct or delete it before saving.";
            }

            if (validated["DlcDir"] == false) {
                return "The WiiU DLC path is invalid.\nPlease correct or delete it before saving.";
            }

            if (validated["GameDirNx"] == false && validated["GameDir"] == false) {
                return "The Switch game/update path is invalid.\nPlease correct or delete it before saving.";
            }

            if (validated["DlcDirNx"] == false) {
                return "The Switch DLC path is invalid.\nPlease correct or delete it before saving.";
            }

            if (validated["GameDir"] == null && validated["UpdateDir"] == null && validated["GameDirNx"] == null) {
                return "No game path has been set for Switch or WiiU.\nPlease set one of them before saving.";
            }

            if (validated["GameDir"] == true && validated["UpdateDir"] == null) {
                return "The WiiU update path has not been set.\nPlease set it before saving.";
            }

            return null;
        }
    }
}