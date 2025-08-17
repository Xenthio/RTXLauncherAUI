// ViewModels/MountingViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace RTXLauncherAUI.ViewModels;

public partial class MountingViewModel : PageViewModel
{
	[ObservableProperty] private bool _mountHl2Rtx;
	[ObservableProperty] private bool _mountPortalRtx;
	[ObservableProperty] private bool _mountPortalPreludeRtx;
	[ObservableProperty] private bool _mountPortal2Rtx;
	[ObservableProperty] private bool _mountDarkMessiahRtx;

	public MountingViewModel()
	{
		Header = "Content Mounting";
	}
}