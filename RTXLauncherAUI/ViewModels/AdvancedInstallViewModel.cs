using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RTXLauncherAUI.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RTXLauncherAUI.ViewModels;

public partial class AdvancedInstallViewModel : PageViewModel
{
	[ObservableProperty] private string? _vanillaInstallPath = "Error Fetching Path";
	[ObservableProperty] private string? _vanillaInstallType = "Error Fetching Install Type";
	[ObservableProperty] private string? _rtxInstallPath = "Error Fetching Path";
	[ObservableProperty] private string? _rtxInstallType = "Error Fetching Install Type";
	[ObservableProperty] private bool _isBusy;
	private readonly GitHubService _githubService;

	// THE SCALABLE LIST OF PACKAGES
	public ObservableCollection<InstallablePackageViewModel> Packages { get; } = new();

	public AdvancedInstallViewModel()
	{
		Header = "Advanced Install";

		_githubService = new GitHubService();

		// To add a new package, you just add it to this list!
		Packages.Add(new RemixPackageViewModel(_githubService));
		Packages.Add(new PatcherPackageViewModel(_githubService));
		Packages.Add(new FixesPackageViewModel(_githubService));
		Packages.Add(new OptiScalerPackageViewModel(_githubService));

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
	public RemixPackageViewModel(GitHubService githubService) : base(githubService)
	{
		Title = "NVIDIA RTX Remix";
	}
	protected override async Task LoadSources()
	{
		// In a real app, this would come from a service or config file
		Sources.Add("NVIDIAGameWorks/rtx-remix");
		Sources.Add("sambow23/dxvk-remix-gmod");
	}

	protected override async Task LoadReleases()
	{
		if (string.IsNullOrEmpty(SelectedSource)) return;

		IsBusy = true;
		Releases.Clear();
		try
		{
			var parts = SelectedSource.Split('/');
			var releases = await GitHubService.FetchReleasesAsync(parts[0], parts[1]);
			foreach (var release in releases)
			{
				Releases.Add(release);
			}
			SelectedRelease = Releases.FirstOrDefault();
		}
		catch (Exception ex)
		{
			// You can add an error message property to display in the UI
			System.Diagnostics.Debug.WriteLine($"Failed to load releases for {Title}: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
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
	public PatcherPackageViewModel(GitHubService githubService) : base(githubService)
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
	public FixesPackageViewModel(GitHubService githubService) : base(githubService) { Title = "Fixes Package"; }
	protected override Task LoadSources() { /* ... Load fixes sources ... */ return Task.CompletedTask; }
	protected override Task LoadReleases() { /* ... Load fixes releases ... */ return Task.CompletedTask; }
	protected override async Task Install() { /* ... Call PackageInstallService ... */ }
}

public partial class OptiScalerPackageViewModel : InstallablePackageViewModel
{
	public OptiScalerPackageViewModel(GitHubService githubService) : base(githubService) { Title = "AMD Support - OptiScaler"; }
	protected override Task LoadSources() { /* ... Load OptiScaler sources ... */ return Task.CompletedTask; }
	protected override Task LoadReleases() { /* ... Load OptiScaler releases ... */ return Task.CompletedTask; }
	protected override async Task Install() { /* ... Call PackageInstallService ... */ }
}
