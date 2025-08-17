// Services/InstallService.cs
using RTXLauncherAUI.Models; // Assuming you put the report/exception classes here
using System;
using System.IO;
using System.Threading.Tasks;

namespace RTXLauncherAUI.Services;

public class GarrysModInstallService
{
	// Note: The logic for restarting as admin and showing dialogs will be moved
	// to the ViewModel, which will catch the SymlinkFailedException.

	public string GetInstallType(string? path)
	{
		if (string.IsNullOrEmpty(path)) return "unknown";
		// ... your existing GetInstallType logic ...
		return "unknown";
	}

	/// <summary>
	/// Creates a new Garry's Mod installation by copying and symlinking from a vanilla install.
	/// </summary>
	/// <param name="vanillaPath">The path to the source Garry's Mod installation.</param>
	/// <param name="newInstallPath">The path where the new RTX installation will be created.</param>
	/// <param name="progress">An IProgress object to report progress back to the UI.</param>
	public async Task CreateNewGmodInstallAsync(string vanillaPath, string newInstallPath, IProgress<InstallProgressReport> progress)
	{
		await Task.Run(() =>
		{
			int totalSteps = 12;
			int currentStep = 0;

			void Report(string message, int step)
			{
				progress.Report(new InstallProgressReport { Message = message, Percentage = (step * 100) / totalSteps });
			}

			Report($"Creating RTX install...", currentStep++);

			// ... all of your existing PerformInstallation() logic goes here ...
			// Replace every call to LogProgress(...) with a call to Report(...)
			// Example:
			// Report("Copying bin folder...", currentStep++);
			// CopyDirectory(Path.Combine(vanillaPath, "bin"), Path.Combine(newInstallPath, "bin"));

			// IMPORTANT: Refactor symlink creation
			// The old CreateDirectorySymbolicLink method needs to be changed.
			// Instead of showing a MessageBox, it should just throw the custom exception.

			// ... inside the loop that creates symlinks ...
			// CreateFileSymbolicLink(targetPath, vpkFile);
		});
	}

	private void CreateFileSymbolicLink(string path, string pathToTarget)
	{
		try
		{
			File.CreateSymbolicLink(path, pathToTarget);
		}
		catch (Exception ex)
		{
			// The service's job is to report failure, not to solve it.
			// The ViewModel will catch this and ask the user what to do.
			throw new SymlinkFailedException(
				$"Failed to create symlink. Administrator privileges may be required.",
				Path.GetFileName(path)
			);
		}
	}
	// ... (repeat for CreateDirectorySymbolicLink) ...

	// Helper method for copying directories (this can remain private)
	private void CopyDirectory(string sourceDir, string destinationDir)
	{
		// ... your existing CopyDirectory logic ...
	}
}