using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RTXLauncherAUI.Models;
using RTXLauncherAUI.Services;
using RTXLauncherAUI.Utilities;
using System;
using System.Threading.Tasks;
namespace RTXLauncherAUI.ViewModels; // Use a dedicated folder for ViewModels

// Make it a partial class and inherit from ObservableObject
public partial class SettingsViewModel : PageViewModel
{
	// The ViewModel HOLDS a reference to the Model.
	private readonly SettingsData _settingsData;

	private readonly QuickInstallService _quickInstallService;
	private readonly IMessenger _messenger;

	[ObservableProperty] private bool _isQuickInstallVisible;
	[ObservableProperty] private bool _isBusy;

	public SettingsViewModel(SettingsData settingsData, QuickInstallService quickInstallService, IMessenger messenger)
	{
		Header = "Settings";
		_quickInstallService = quickInstallService;
		_messenger = messenger;

		CheckInstallationStatus();
	}

	public void CheckInstallationStatus()
	{
		var installType = GarrysModUtility.GetInstallType(GarrysModUtility.GetThisInstallFolder());
		IsQuickInstallVisible = installType == "unknown";
	}

	[RelayCommand(CanExecute = nameof(CanRunQuickInstall))]
	private async Task RunQuickInstall()
	{
		var confirmed = await DialogUtility.ShowConfirmationAsync(
			"Quick Install Confirmation",
			"This will perform a complete installation with recommended settings.\n\n" +
			"• Create a new RTX installation (if needed)\n" +
			"• Install the latest recommended RTX Remix\n" +
			"• Apply recommended patches\n" +
			"• Install the latest recommended fixes package\n\n" +
			"Do you want to continue?");

		if (!confirmed) return;

		IsBusy = true;
		var progressHandle = new Progress<InstallProgressReport>(report => _messenger.Send(new ProgressReportMessage(report)));
		IProgress<InstallProgressReport> progress = progressHandle;

		try
		{
			await _quickInstallService.PerformQuickInstallAsync(progress);
			CheckInstallationStatus(); // Hide the panel on success
		}
		catch (Exception ex)
		{
			progress.Report(new InstallProgressReport { Message = $"FATAL ERROR: {ex.Message}", Percentage = 100 });
		}
		finally
		{
			IsBusy = false;
		}
	}
	private bool CanRunQuickInstall() => !IsBusy;

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