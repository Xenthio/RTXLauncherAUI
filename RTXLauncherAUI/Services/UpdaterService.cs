// Services/UpdaterService.cs
using RTXLauncherAUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UpdaterService
{
	// This method now returns data instead of updating UI directly
	public async Task<List<UpdateSource>> GetAvailableUpdatesAsync()
	{
		// ... Move your logic from PopulateUpdateSourcesComboBox() here ...
		// Fetch from GitHubAPI, create UpdateSource objects, and return the list.
		var sources = new List<UpdateSource>();
		// ... your fetching logic ...
		return sources;
	}

	public async Task DownloadAndInstallUpdateAsync(UpdateSource source, IProgress<string> progress)
	{
		progress.Report("Starting download...");
		// ... Move your logic from InstallLauncherUpdateButton_Click here ...
		// Use HttpClient to download, extract, create the batch file, and run it.
		// Use the IProgress<T> parameter to report progress back to the ViewModel.
		progress.Report("Update complete. Restarting...");
	}
}