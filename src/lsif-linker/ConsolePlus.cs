namespace lsif_linker
{
	internal static class ConsolePlus
	{
		public static void WriteLine() => Console.WriteLine();
		public static void WriteLine(string content) => Console.WriteLine(content);

		public static void WriteWarning(string content)
		{
			var originalColor = System.Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("[Warning] " + content);
			Console.ForegroundColor = originalColor;
		}

		public static void WriteError(string content)
		{
			var originalColor = System.Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[Error] " + content);
			Console.ForegroundColor = originalColor;
		}

		public static void WriteSuccess(string content)
		{
			var originalColor = System.Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("[Success] " + content);
			Console.ForegroundColor = originalColor;
		}
	}
}
