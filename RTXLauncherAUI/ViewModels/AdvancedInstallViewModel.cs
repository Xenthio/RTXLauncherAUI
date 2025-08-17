using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RTXLauncherAUI.Models;
using RTXLauncherAUI.Services;
using RTXLauncherAUI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RTXLauncherAUI.ViewModels;

public partial class AdvancedInstallViewModel : PageViewModel
{
	[ObservableProperty] private string _vanillaInstallPath = "Error Fetching Path";
	[ObservableProperty] private string _vanillaInstallType = "Error Fetching Install Type";
	[ObservableProperty] private string _rtxInstallPath = "Error Fetching Path";
	[ObservableProperty] private string _rtxInstallType = "Error Fetching Install Type";
	[ObservableProperty] private bool _isBusy;
	private readonly IMessenger _messenger;
	private readonly GitHubService _githubService;
	private readonly GarrysModInstallService _garrysModInstallService;
	private readonly GarrysModUpdateService _garrysModUpdateService = new();

	// THE SCALABLE LIST OF PACKAGES
	public ObservableCollection<InstallablePackageViewModel> Packages { get; } = new();

	public AdvancedInstallViewModel(IMessenger messenger, GitHubService githubService)
	{
		Header = "Advanced Install";

		_githubService = githubService;
		_garrysModInstallService = new GarrysModInstallService();
		_garrysModUpdateService = new GarrysModUpdateService();
		_messenger = messenger;

		// To add a new package, you just add it to this list!
		Packages.Add(new RemixPackageViewModel(_githubService));
		Packages.Add(new PatcherPackageViewModel(_githubService));
		Packages.Add(new FixesPackageViewModel(_githubService));
		Packages.Add(new OptiScalerPackageViewModel(_githubService));

		// Initialize all packages
		_ = InitializePackages();

		RefreshInstallInfo();
	}

	private void RefreshInstallInfo()
	{
		// Refresh the install info

		_vanillaInstallPath = GarrysModUtility.GetVanillaInstallFolder();
		_vanillaInstallType = GarrysModUtility.GetInstallType(_vanillaInstallPath);

		if (_vanillaInstallType == "unknown") _vanillaInstallType = "Not installed / not found";

		_rtxInstallPath = GarrysModUtility.GetThisInstallFolder();
		_rtxInstallType = GarrysModUtility.GetInstallType(_rtxInstallPath);

		if (_rtxInstallType == "unknown")
		{
			_rtxInstallType = "There's no install here, create one!";
			//CreateInstallButton.Enabled = true;
			//UpdateInstallButton.Enabled = false;
		}
		else
		{
			//CreateInstallButton.Enabled = false;
			//UpdateInstallButton.Enabled = true;
		}

		// Update visibility of the QuickInstallGroup
		//UpdateQuickInstallGroupVisibility();
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

		// Use the utility to get the paths
		var vanillaPath = GarrysModUtility.GetVanillaInstallFolder();
		var newInstallPath = GarrysModUtility.GetThisInstallFolder();

		if (string.IsNullOrEmpty(vanillaPath))
		{
			// TODO: Show an error dialog to the user
			IsBusy = false;
			return;
		}

		// Set up progress reporting
		var progress = new Progress<InstallProgressReport>(report =>
		{
			_messenger.Send(new ProgressReportMessage(report));
		});

		try
		{
			await _garrysModInstallService.CreateNewGmodInstallAsync(vanillaPath, newInstallPath, progress);
			// TODO: Show a "Success!" dialog
		}
		catch (SymlinkFailedException ex)
		{
			// This is where you handle the specific error.
			// You would show a dialog asking the user if they want to retry as admin.
			//InstallProgressText = $"Error: {ex.Message}";
		}
		catch (Exception ex)
		{
			// Handle all other installation errors
			//InstallProgressText = $"An unexpected error occurred: {ex.Message}";
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
	// --- 1. Add your sources dictionary as a private field ---
	private readonly Dictionary<string, (string Owner, string Repo)> _remixSources = new()
	{
		{ "(OFFICIAL) NVIDIAGameWorks/rtx-remix", ("NVIDIAGameWorks", "rtx-remix") },
		{ "sambow23/dxvk-remix-gmod", ("sambow23", "dxvk-remix-gmod") },
	};

	public RemixPackageViewModel(GitHubService githubService) : base(githubService)
	{
		Title = "NVIDIA RTX Remix";
	}

	// --- 2. Implement LoadSources to read from the dictionary ---
	protected override Task LoadSources()
	{
		Sources.Clear();
		foreach (var sourceName in _remixSources.Keys)
		{
			Sources.Add(sourceName);
		}
		return Task.CompletedTask;
	}

	// --- 3. Implement LoadReleases to use the selected source ---
	protected override async Task LoadReleases()
	{
		if (string.IsNullOrEmpty(SelectedSource) || !_remixSources.TryGetValue(SelectedSource, out var sourceInfo))
		{
			Releases.Clear();
			return;
		}

		IsBusy = true;
		Releases.Clear();
		try
		{
			var releases = await GitHubService.FetchReleasesAsync(sourceInfo.Owner, sourceInfo.Repo);
			foreach (var release in releases.OrderByDescending(r => r.PublishedAt))
			{
				Releases.Add(release);
			}
			SelectedRelease = Releases.FirstOrDefault();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Remix] Failed to load releases: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	protected override async Task Install()
	{
		if (SelectedRelease == null) return;
		IsBusy = true;
		System.Diagnostics.Debug.WriteLine($"Installing Remix: {SelectedRelease.Name}");
		// TODO: Call your PackageInstallService here
		await Task.Delay(2000);
		IsBusy = false;
	}
}

public partial class PatcherPackageViewModel : InstallablePackageViewModel
{
	// --- 1. Add your sources dictionary ---
	private readonly Dictionary<string, (string Owner, string Repo, string FilePath)> _patchSources = new()
	{
		{ "BlueAmulet/SourceRTXTweaks", ("BlueAmulet", "SourceRTXTweaks", "applypatch.py") },
		{ "sambow23/SourceRTXTweaks", ("sambow23", "SourceRTXTweaks", "applypatch.py") },
		{ "Xenthio/SourceRTXTweaks (outdated, here to test multiple repos)", ("Xenthio", "SourceRTXTweaks", "applypatch.py") }
	};

	public PatcherPackageViewModel(GitHubService githubService) : base(githubService)
	{
		Title = "Binary Patches";
		ButtonText = "Apply Patches"; // Set a custom button text
	}

	// --- 2. Implement LoadSources ---
	protected override Task LoadSources()
	{
		Sources.Clear();
		foreach (var sourceName in _patchSources.Keys)
		{
			Sources.Add(sourceName);
		}
		return Task.CompletedTask;
	}

	// --- 3. Patches don't have releases, so this is empty ---
	protected override Task LoadReleases()
	{
		// We can hide the "Releases" ComboBox in the UI for this package type
		// by binding its IsVisible property to a boolean on this ViewModel.
		// For now, we just do nothing.
		Releases.Clear();
		return Task.CompletedTask;
	}

	protected override async Task Install()
	{
		if (string.IsNullOrEmpty(SelectedSource) || !_patchSources.TryGetValue(SelectedSource, out var sourceInfo)) return;

		IsBusy = true;
		System.Diagnostics.Debug.WriteLine($"Applying patches from: {SelectedSource}");
		// TODO: Create a PatchingService and call it here, passing the sourceInfo
		// For example: await _patchingService.ApplyPatchesAsync(sourceInfo.Owner, sourceInfo.Repo, sourceInfo.FilePath);
		await Task.Delay(2000);
		IsBusy = false;
	}
}

public partial class FixesPackageViewModel : InstallablePackageViewModel
{
	// --- 1. Add your sources dictionary ---
	private readonly Dictionary<string, (string Owner, string Repo, string InstallType)> _packageSources = new()
	{
		{ "Xenthio/gmod-rtx-fixes-2 (Any)", ("Xenthio", "gmod-rtx-fixes-2", "Any") },
		{ "Xenthio/RTXFixes (gmod_main)", ("Xenthio", "RTXFixes", "gmod_main") }
	};

	public FixesPackageViewModel(GitHubService githubService) : base(githubService)
	{
		Title = "Fixes Package";
	}

	// --- 2. Implement LoadSources ---
	protected override Task LoadSources()
	{
		Sources.Clear();
		foreach (var sourceName in _packageSources.Keys)
		{
			Sources.Add(sourceName);
		}
		return Task.CompletedTask;
	}

	// --- 3. Implement LoadReleases ---
	protected override async Task LoadReleases()
	{
		if (string.IsNullOrEmpty(SelectedSource) || !_packageSources.TryGetValue(SelectedSource, out var sourceInfo))
		{
			Releases.Clear();
			return;
		}

		IsBusy = true;
		Releases.Clear();
		try
		{
			var releases = await GitHubService.FetchReleasesAsync(sourceInfo.Owner, sourceInfo.Repo);
			foreach (var release in releases.OrderByDescending(r => r.PublishedAt))
			{
				Releases.Add(release);
			}
			SelectedRelease = Releases.FirstOrDefault();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[Fixes] Failed to load releases: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	protected override async Task Install()
	{
		if (SelectedRelease == null) return;
		IsBusy = true;
		System.Diagnostics.Debug.WriteLine($"Installing Fixes: {SelectedRelease.Name}");
		// TODO: Call your PackageInstallService here
		await Task.Delay(2000);
		IsBusy = false;
	}
}

public partial class OptiScalerPackageViewModel : InstallablePackageViewModel
{
	// --- 1. Add your sources dictionary for OptiScaler ---
	private readonly Dictionary<string, (string Owner, string Repo)> _optiScalerSources = new()
	{
		{ "sambow23/OptiScaler-Releases", ("sambow23", "OptiScaler-Releases") }
	};

	public OptiScalerPackageViewModel(GitHubService githubService) : base(githubService)
	{
		Title = "AMD Support - OptiScaler";
	}

	// --- 2. Implement LoadSources to read from the dictionary ---
	protected override Task LoadSources()
	{
		Sources.Clear();
		foreach (var sourceName in _optiScalerSources.Keys)
		{
			Sources.Add(sourceName);
		}
		return Task.CompletedTask;
	}

	// --- 3. Implement LoadReleases to use the selected source ---
	protected override async Task LoadReleases()
	{
		if (string.IsNullOrEmpty(SelectedSource) || !_optiScalerSources.TryGetValue(SelectedSource, out var sourceInfo))
		{
			Releases.Clear();
			return;
		}

		IsBusy = true;
		Releases.Clear();
		try
		{
			var releases = await GitHubService.FetchReleasesAsync(sourceInfo.Owner, sourceInfo.Repo);
			foreach (var release in releases.OrderByDescending(r => r.PublishedAt))
			{
				Releases.Add(release);
			}
			SelectedRelease = Releases.FirstOrDefault();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"[OptiScaler] Failed to load releases: {ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	protected override async Task Install()
	{
		if (SelectedRelease == null) return;
		IsBusy = true;
		System.Diagnostics.Debug.WriteLine($"Installing OptiScaler: {SelectedRelease.Name}");
		// TODO: Call your PackageInstallService here
		await Task.Delay(2000);
		IsBusy = false;
	}
}