internal class MetadataPreamble : Vertex
{
	public static readonly MetadataPreamble Instance = new();

	private MetadataPreamble(): base("metaData")
	{
	}

	public string Version => "0.6.0-next.1";
}
