// Services/PatchingService.cs

using RTXLauncherAUI.Models;
using RTXLauncherAUI.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RTXLauncherAUI.Services;

public class PatchingService
{
	private readonly HttpClient _httpClient;

	public PatchingService()
	{
		_httpClient = new HttpClient();
		_httpClient.DefaultRequestHeaders.Add("User-Agent", "RTXLauncher");
	}

	public async Task ApplyPatchesAsync(string owner, string repo, string filePath, string installPath, IProgress<InstallProgressReport> progress)
	{
		// 1. Fetch the patch file content
		progress.Report(new InstallProgressReport { Message = "Fetching patch definitions...", Percentage = 5 });
		var patchFileContent = await FetchPatchFileContentAsync(owner, repo, filePath);
		var patchDictionaries = PatchParser.ExtractPatchDictionaries(patchFileContent);

		// 2. Run the patching logic on a background thread
		await Task.Run(() =>
		{
			// This is the entire logic from your old PatchingSystem.ApplyPatches method.
			// It needs to be refactored to call progress.Report() instead of progressCallback().
			// Example:
			progress.Report(new InstallProgressReport { Message = "Parsing patch definitions...", Percentage = 10 });
			var (patches32, patches64) = PatchParser.ParsePatches(patchDictionaries);

			// ... Determine install type using GarrysModUtility ...
			var installType = GarrysModUtility.GetInstallType(installPath);
			// ... Select correct patch dictionary ...

			// ... Load files, apply patches, create backups, and write modified files ...
			// Each step should report its progress.
			progress.Report(new InstallProgressReport { Message = "Applying patches to client.dll...", Percentage = 50 });

			// ... on completion ...
			progress.Report(new InstallProgressReport { Message = "Patching complete!", Percentage = 100 });
		});
	}

	private async Task<string> FetchPatchFileContentAsync(string owner, string repo, string filePath)
	{
		var url = $"https://raw.githubusercontent.com/{owner}/{repo}/master/{filePath}";
		return await _httpClient.GetStringAsync(url);
	}

	// NOTE: All private helper methods from PatchingSystem.cs (like FindWithMask, HexStringToByteArray, etc.)
	// would be moved here as private methods of this service class.
}