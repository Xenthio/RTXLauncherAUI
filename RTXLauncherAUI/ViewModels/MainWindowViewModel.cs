using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace RTXLauncherAUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
	// A list of all available pages
	public ObservableCollection<PageViewModel> Pages { get; }

	// The currently selected page
	[ObservableProperty]
	private PageViewModel _selectedPage;

	public MainWindowViewModel()
	{
		// Create instances of all your pages
		Pages = new ObservableCollection<PageViewModel>
		{
			new SettingsViewModel { Header = "Settings" },
			new MountingViewModel(),
			new AdvancedInstallViewModel(),
			new AboutViewModel()
		};

		// Set the default page to be the first one
		_selectedPage = Pages[0];
	}
}