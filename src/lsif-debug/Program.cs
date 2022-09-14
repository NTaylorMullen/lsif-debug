using lsif_debug;
using System.CommandLine;

internal class Program
{
	static async Task<int> Main(string[] args)
	{
		var cancellationToken = CancellationToken.None;
		var rootCommand = new RootCommand("Tool to allow you to better debug / understand LSIF output.");
		rootCommand.SetHandler(() => rootCommand.InvokeAsync("-h"));

		LinkCommand.AddTo(rootCommand);

		return await rootCommand.InvokeAsync(args);
	}

}
