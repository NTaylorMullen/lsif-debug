internal abstract class Vertex
{
	public Vertex(string label)
	{
		Label = label ?? throw new ArgumentNullException(nameof(label));
	}

	public string Label { get; }

	public string Type => "vertex";
}