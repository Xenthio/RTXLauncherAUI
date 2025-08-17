using Avalonia.Controls;
using RTXLauncherAUI.ViewModels; // Import viewmodels

namespace RTXLauncherAUI;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		// The DataContext is now the main window's viewmodel
		var vm = new MainWindowViewModel();
		DataContext = vm;

		this.Closing += (sender, args) => vm.OnWindowClosing();
	}
}