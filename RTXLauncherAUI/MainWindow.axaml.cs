using Avalonia.Controls;
using RTXLauncherAUI.ViewModels; // Import viewmodels

namespace RTXLauncherAUI;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		// The DataContext is now the main window's viewmodel
		DataContext = new MainWindowViewModel();
	}
}