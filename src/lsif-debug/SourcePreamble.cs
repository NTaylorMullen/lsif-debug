internal class SourcePreamble : Vertex
{
	public SourcePreamble(Uri workspaceRootUri): base (label: "source")
	{
		WorkspaceRoot = workspaceRootUri.ToString();
	}

	public string WorkspaceRoot { get; }
}
