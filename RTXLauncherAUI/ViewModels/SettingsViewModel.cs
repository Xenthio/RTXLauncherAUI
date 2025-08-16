using CommunityToolkit.Mvvm.ComponentModel;
namespace RTXLauncherAUI.ViewModels; // Use a dedicated folder for ViewModels

// Make it a partial class and inherit from ObservableObject
public partial class SettingsViewModel : PageViewModel
{
	// For every property you want to bind to, create a private field
	// and annotate it with [ObservableProperty]. The source generator
	// will automatically create a public property with the correct
	// change notification logic.

	// --- Resolution GroupBox ---

	// This will generate a public bool property named "UseCustomResolution"
	[ObservableProperty]
	private bool _useCustomResolution;

	// This will generate a public int property named "Width"
	[ObservableProperty]
	private int _width = 1920;

	// This will generate a public int property named "Height"
	[ObservableProperty]
	private int _height = 1080;


	// --- Garry's Mod GroupBox ---

	[ObservableProperty]
	private bool _rtxInstalled; // Used to enable/disable the "RTX On" checkbox

	[ObservableProperty]
	private bool _loadWorkshopAddons = true;


	// --- Miscellaneous Page ---

	[ObservableProperty]
	private bool _toolsMode;

	[ObservableProperty]
	private bool _disableChromium;

	[ObservableProperty]
	private int _dxLevel = 90;

	[ObservableProperty]
	private bool _consoleEnabled = true;

	[ObservableProperty]
	private bool _developerMode;

	[ObservableProperty]
	private string? _customLaunchOptions;

	// You would continue this pattern for all other settings...
}