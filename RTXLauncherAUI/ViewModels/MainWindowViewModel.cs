// ViewModels/MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging; // <-- Import the Messenger
using RTXLauncherAUI.Models;
using RTXLauncherAUI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
namespace RTXLauncherAUI.ViewModels;
public partial class MainWindowViewModel : ViewModelBase
{
	private readonly IMessenger _messenger;

	// --- NEW: Properties for the Top Progress Bar ---

	[ObservableProperty]
	private int _progressValue;

	// The full log for the expandable view
	public ObservableCollection<string> FullLog { get; } = new();
	public ObservableCollection<string> CarouselLog { get; } = new();

	[ObservableProperty]
	private int _carouselIndex;

	[ObservableProperty]
	private bool _isLogVisible;

	// --- Existing Properties ---
	public ObservableCollection<PageViewModel> Pages { get; }
	[ObservableProperty]
	private PageViewModel? _selectedPage;

	public MainWindowViewModel()
	{
		// Use the default singleton messenger instance
		_messenger = WeakReferenceMessenger.Default;

		// ** SUBSCRIBE to progress messages **
		_messenger.Register<ProgressReportMessage>(this, (recipient, message) =>
		{
			// When a message is received, update our properties
			ProgressValue = message.Report.Percentage;
			FullLog.Add(message.Report.Message);
			CarouselLog.Insert(0, message.Report.Message);
			CarouselIndex = 0;
		});


		CarouselLog.Add("Welcome to the Garry's Mod RTX Launcher!");
		FullLog.Add("Welcome to the Garry's Mod RTX Launcher!");

		// --- Existing Page Setup ---
		// Pass the messenger instance down to the page ViewModels
		var gitHubService = new GitHubService();
		Pages = new ObservableCollection<PageViewModel>
		{
			new SettingsViewModel { Header = "Settings" },
			new MountingViewModel(),
			new AdvancedInstallViewModel(_messenger, gitHubService), // Pass messenger
            new AboutViewModel(_messenger, gitHubService)             // Pass messenger
        };
		_selectedPage = Pages.FirstOrDefault();
	}

	[RelayCommand]
	private async Task SaveLog()
	{
		// This is a placeholder. A real implementation would use a Dialog Service.
		var logContent = string.Join("\n", FullLog);
		// await dialogService.ShowSaveFileDialogAsync(logContent, "log.txt");
		System.Diagnostics.Debug.WriteLine("---- LOG SAVED ----\n" + logContent);
	}
}