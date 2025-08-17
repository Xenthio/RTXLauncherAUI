using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RTXLauncherAUI.ViewModels;

public partial class AdvancedInstallViewModel : PageViewModel
{
	[ObservableProperty] private string? _vanillaInstallPath = "C:/.../GarrysMod";
	[ObservableProperty] private string? _vanillaInstallType = "gmod_x86-64";
	[ObservableProperty] private string? _rtxInstallPath = "C:/.../GarrysModRTX";
	[ObservableProperty] private string? _rtxInstallType = "gmod_x86-64_rtx";
	[ObservableProperty] private bool _isBusy;

	// THE SCALABLE LIST OF PACKAGES
	public ObservableCollection<InstallablePackageViewModel> Packages { get; } = new();

	public AdvancedInstallViewModel() // In a real app, you'd inject services here
	{
		Header = "Advanced Install";

		// To add a new package, you just add it to this list!
		Packages.Add(new RemixPackageViewModel());
		Packages.Add(new PatcherPackageViewModel());
		Packages.Add(new FixesPackageViewModel());
		Packages.Add(new OptiScalerPackageViewModel());

		// Initialize all packages
		_ = InitializePackages();
	}

	private async Task InitializePackages()
	{
		foreach (var package in Packages)
		{
			await package.InitializeAsync();
		}
	}


	[RelayCommand]
	private async Task UpdateInstall()
	{
		IsBusy = true;
		try
		{
			// TODO: Call your GarrysModUpdateService to show the update dialog
			// For example: await _updateService.ShowUpdateDialogAsync();

			// Simulate work for now
			await Task.Delay(2000);
		}
		finally
		{
			IsBusy = false;
		}
	}

	[RelayCommand]
	private async Task CreateInstall()
	{
		IsBusy = true;
		try
		{
			// TODO: Call your GarrysModInstallService to create the installation
			// For example: await _installService.CreateRTXInstallAsync();

			// Simulate work for now
			await Task.Delay(2000);
		}
		finally
		{
			IsBusy = false;
		}
	}
}

// ===================================================================
//      EXAMPLE IMPLEMENTATIONS OF SPECIFIC PACKAGE VIEWMODELS
// ===================================================================

public partial class RemixPackageViewModel : InstallablePackageViewModel
{
	public RemixPackageViewModel() { Title = "NVIDIA RTX Remix"; }

	protected override async Task LoadSources()
	{
		// In a real app, this would come from a service or config file
		Sources.Add("(OFFICIAL) NVIDIAGameWorks/rtx-remix");
		Sources.Add("sambow23/dxvk-remix-gmod");
	}

	protected override async Task LoadReleases()
	{
		IsBusy = true;
		Releases.Clear();
		// TODO: Call your GitHubService to fetch releases for the SelectedSource
		// and populate the Releases collection.
		// For now, we'll add placeholder data.
		Releases.Add(new GitHubRelease { Name = "v0.4.1" });
		Releases.Add(new GitHubRelease { Name = "v0.4.0" });
		SelectedRelease = Releases.Count > 0 ? Releases[0] : null;
		IsBusy = false;
	}

	protected override async Task Install()
	{
		IsBusy = true;
		// TODO: Call your PackageInstallService to install the SelectedRelease
		await Task.Delay(2000); // Simulate work
		IsBusy = false;
	}
}

public partial class PatcherPackageViewModel : InstallablePackageViewModel
{
	public PatcherPackageViewModel()
	{
		Title = "Binary Patches";
		ButtonText = "Apply Patches"; // Custom button text
	}

	protected override Task LoadSources() { /* ... Load patcher sources ... */ return Task.CompletedTask; }
	protected override Task LoadReleases() { /* Patches don't have releases, so this might be empty */ return Task.CompletedTask; }
	protected override async Task Install() { /* ... Call PatchingService ... */ }
}

public partial class FixesPackageViewModel : InstallablePackageViewModel
{
	public FixesPackageViewModel() { Title = "Fixes Package"; }
	protected override Task LoadSources() { /* ... Load fixes sources ... */ return Task.CompletedTask; }
	protected override Task LoadReleases() { /* ... Load fixes releases ... */ return Task.CompletedTask; }
	protected override async Task Install() { /* ... Call PackageInstallService ... */ }
}

public partial class OptiScalerPackageViewModel : InstallablePackageViewModel
{
	public OptiScalerPackageViewModel() { Title = "AMD Support - OptiScaler"; }
	protected override Task LoadSources() { /* ... Load OptiScaler sources ... */ return Task.CompletedTask; }
	protected override Task LoadReleases() { /* ... Load OptiScaler releases ... */ return Task.CompletedTask; }
	protected override async Task Install() { /* ... Call PackageInstallService ... */ }
}

// A placeholder for the GitHubRelease object
public class GitHubRelease
{
	public string Name { get; set; } = string.Empty;
	// Add other properties like Body, Assets, etc.
}