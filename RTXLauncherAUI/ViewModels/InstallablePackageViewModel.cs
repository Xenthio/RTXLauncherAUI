using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RTXLauncherAUI.ViewModels;

// A base class for any package that can be installed from a GitHub source.
public abstract partial class InstallablePackageViewModel : ViewModelBase
{
	[ObservableProperty]
	private string? _title;

	[ObservableProperty]
	private string _buttonText = "Install/Update";

	[ObservableProperty]
	private bool _isBusy; // Use this to disable controls during operations

	// A simple derived property for easier binding
	public bool IsNotBusy => !IsBusy;

	// Properties for the ComboBoxes
	public ObservableCollection<string> Sources { get; } = new();
	public ObservableCollection<GitHubRelease> Releases { get; } = new();

	[ObservableProperty]
	private string? _selectedSource;

	[ObservableProperty]
	private GitHubRelease? _selectedRelease;

	// The command for the install button
	[RelayCommand]
	protected abstract Task Install();

	// Abstract methods to be implemented by specific package types
	protected abstract Task LoadSources();
	protected abstract Task LoadReleases();

	// This method is called when the ViewModel is created
	public async Task InitializeAsync()
	{
		await LoadSources();
		if (Sources.Count > 0)
		{
			SelectedSource = Sources[0];
		}
	}

	// When SelectedSource changes, automatically reload the releases
	async partial void OnSelectedSourceChanged(string? value)
	{
		await LoadReleases();
	}
}