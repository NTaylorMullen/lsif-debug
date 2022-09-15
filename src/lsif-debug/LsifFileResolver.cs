namespace lsif_debug
{
	internal static class LsifFileResolver
	{
		public static List<FileInfo> ResolveLinkedLsifFiles(string lsifPath)
			=> ResolveLsifFilesCore(lsifPath, linkedLsifFiles: true);
		public static List<FileInfo> ResolveLsifFiles(string lsifPath)
			=> ResolveLsifFilesCore(lsifPath, linkedLsifFiles: false);

		private static List<FileInfo> ResolveLsifFilesCore(string lsifPath, bool linkedLsifFiles)
		{
			var lsifFiles = new List<FileInfo>();
			if (Path.HasExtension(lsifPath))
			{
				// File
				lsifFiles.Add(new FileInfo(lsifPath));
			}
			else
			{
				// Directory
				var resolvedFiles = Directory.GetFiles(lsifPath, "*.lsif", SearchOption.AllDirectories);

				foreach (var filePath in resolvedFiles)
				{
					if (linkedLsifFiles)
					{
						if (!filePath.EndsWith(".linked.lsif", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
					}
					else
					{
						if (filePath.EndsWith(".linked.lsif", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
					}

					lsifFiles.Add(new FileInfo(filePath));
				}
			}

			return lsifFiles;
		}
	}
}
