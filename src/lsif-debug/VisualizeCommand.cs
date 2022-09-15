using System.CommandLine;
using System.Diagnostics;
using System.Text;
using Console = lsif_debug.ConsolePlus;

namespace lsif_debug
{
	internal class VisualizeCommand : Command
	{
		private new const string Name = "visualize";
		private new const string Description = "Visualize a linked LSIF file in VSCode.";

		public VisualizeCommand() : base(Name, Description)
		{
			var lsifArgument = new Argument<string>("lsif", "Path to a *.linked.lsif file or directory of *.linked.lsif files.");

			Add(lsifArgument);

			this.SetHandler(ExecuteAsync, lsifArgument);
		}

		private async Task ExecuteAsync(string? lsifPath)
		{
			if (lsifPath is null)
			{
				await this.InvokeAsync("-h");
				return;
			}

			var linkedLsif = LsifFileResolver.ResolveLinkedLsifFiles(lsifPath);
			if (linkedLsif.Count == 0)
			{
				Console.WriteError($"Failed to resolve any LSIF files at: '{lsifPath}'");
				return;
			}

			var codeExecutablePath = TryResolveCodeExecutablePath();
			if (codeExecutablePath is null)
			{
				Console.WriteError("Could not locate VSCode on your 'PATH'. Ensure that VSCode is installed.");
				return;
			}

			var cancellationToken = CancellationToken.None;
			var extensionInstalled = await EnsureExtensionInstalledAsync(codeExecutablePath, cancellationToken);
			if (!extensionInstalled)
			{
				Console.WriteError("Failed to visualize provided LSIF");
				return;
			}

			var success = await TryLaunchVSCodeForLSIFAsync(codeExecutablePath, linkedLsif, cancellationToken);
			if (!success)
			{
				Console.WriteError("Failed to launch VSCode visualizer");
			}

			Console.WriteSuccess($"Visualizing LSIF file(s) at: '{lsifPath}'");
		}

		private async Task<bool> EnsureExtensionInstalledAsync(string codeExecutablePath, CancellationToken cancellationToken)
		{
			var isExtensionInstalled = await IsExtensionInstalledAsync(codeExecutablePath, cancellationToken);
			if (isExtensionInstalled)
			{
				return true;
			}

			// Need to install the extension

			var pathToExtensionVsix = TryResolvePathToExtensionVsix();
			if (pathToExtensionVsix is null)
			{
				Console.WriteError("Could not locate path to LSIF visualizer extension");
				return false;
			}

			var success = await TryInstallExtensionAsync(codeExecutablePath, pathToExtensionVsix, cancellationToken);
			if (!success)
			{
				Console.WriteError("Failed to install VSCode LSIF visualizer extension, see above for details.");
				return false;
			}

			Console.WriteLine("Successfully installed visualizer extension!");
			return true;
		}

		private async Task<bool> TryLaunchVSCodeForLSIFAsync(string codeExecutablePath, IReadOnlyList<FileInfo> linkedLsif, CancellationToken cancellationToken)
		{
			Console.WriteLine("Visualizing LSIF:");
			var arguments = new StringBuilder();

			foreach (var lsif in linkedLsif)
			{
				var normalizedPath = lsif.FullName.Replace('\\', '/');
				arguments
					.Append("--folder-uri=\"lsif:///")
					.Append(normalizedPath)
					.Append("\" ");

				Console.WriteLine($"    - '{lsif.FullName}'");
			}

			var process = new Process()
			{
				StartInfo = new ProcessStartInfo(codeExecutablePath, arguments.ToString())
				{
					RedirectStandardOutput = true,
					RedirectStandardError = true,
				},
			};
			process.OutputDataReceived += (sender, args) => Console.WriteLine(args?.Data);
			process.ErrorDataReceived += (sender, args) => Console.WriteError(args?.Data);

			process.Start();
			await process.WaitForExitAsync(cancellationToken);

			return process.ExitCode == 0;
		}

		private async Task<bool> TryInstallExtensionAsync(string codeExecutablePath, string pathToExtensionVsix, CancellationToken cancellationToken)
		{
			Console.WriteLine("LSIF visualizer extension is not installed. Attempting to install into VSCode....");
			var process = new Process()
			{
				StartInfo = new ProcessStartInfo(codeExecutablePath, $"--install-extension {pathToExtensionVsix}")
				{
					RedirectStandardOutput = true,
					RedirectStandardError = true,
				},
			};
			process.OutputDataReceived += (sender, args) => Console.WriteLine(args?.Data);
			process.ErrorDataReceived += (sender, args) => Console.WriteError(args?.Data);

			process.Start();
			await process.WaitForExitAsync(cancellationToken);

			return process.ExitCode == 0;
		}

		private string? TryResolvePathToExtensionVsix()
		{
			var thisAssemblyLocation = typeof(VisualizeCommand).Assembly.Location ?? typeof(VisualizeCommand).Assembly.CodeBase;
			var thisAssemblyDirectory = Path.GetDirectoryName(thisAssemblyLocation);
			var extensionVsixPath = Path.Combine(thisAssemblyDirectory, "lsif-visualizer-extension-0.0.1.vsix");

			if (!File.Exists(extensionVsixPath))
			{
				return null;
			}

			return extensionVsixPath;
		}

		private async Task<bool> IsExtensionInstalledAsync(string codeExecutablePath, CancellationToken cancellationToken)
		{
			var extensionSeen = false;
			var process = new Process()
			{
				StartInfo = new ProcessStartInfo(codeExecutablePath, "--list-extensions")
				{
					RedirectStandardOutput = true,
				},
			};
			process.OutputDataReceived += (sender, args) =>
			{
				if (args.Data?.Contains("ms-vscode.lsif-visualizer-extension") == true)
				{
					extensionSeen = true;
				}
			};

			process.Start();
			await process.WaitForExitAsync(cancellationToken);

			return extensionSeen;
		}

		private static string? TryResolveCodeExecutablePath()
		{
			var path = Environment.GetEnvironmentVariable("PATH");
			if (path is null)
			{
				return null;
			}

			var splitPath = path.Split(';').Select(item => item.Trim()).ToList();
			for (var i = 0; i < splitPath.Count; i++)
			{
				var pathPiece = splitPath[i];
				var potentialExe = Path.Combine(pathPiece, "code.cmd");
				if (File.Exists(potentialExe))
				{
					return potentialExe;
				}
				potentialExe = Path.Combine(pathPiece, "code.sh");
				if (File.Exists(potentialExe))
				{
					return potentialExe;
				}
			}

			return null;
		}
	}
}
