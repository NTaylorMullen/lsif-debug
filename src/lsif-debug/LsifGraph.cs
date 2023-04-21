using System.Text.Json;

namespace lsif_debug
{
    internal sealed class LsifGraph
    {
        public LsifGraph(
            Dictionary<int, EdgeOrVertex> verticiesById,
            Dictionary<int, EdgeOrVertex> edgesById,
            Dictionary<int, HashSet<EdgeOrVertex>> edgesByOutVertexId,
            Dictionary<int, HashSet<EdgeOrVertex>> edgesByInVertexId)
        {
            this.VerticiesById = verticiesById;
            this.EdgesById = edgesById;
            this.EdgesByOutVertexId = edgesByOutVertexId;
            this.EdgesByInVertexId = edgesByInVertexId;
        }

        public static LsifGraph FromLines(IReadOnlyList<string> lines)
        {
            var verticiesById = new Dictionary<int, EdgeOrVertex>();
            var edgesById = new Dictionary<int, EdgeOrVertex>();
            var edgesByOutVertexId = new Dictionary<int, HashSet<EdgeOrVertex>>();
            var edgesByInVertexId = new Dictionary<int, HashSet<EdgeOrVertex>>();

            // Create a lookup table for nodes + edges.
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                var edgeOrVertex = JsonSerializer.Deserialize<EdgeOrVertex>(line);

                if (edgeOrVertex is not null)
                {
                    if (edgeOrVertex.type == "vertex" &&
                        edgeOrVertex.id is not null)
                    {
                        verticiesById.Add(edgeOrVertex.id.Value, edgeOrVertex);
                    }
                    else if (edgeOrVertex.type == "edge")
                    {
                        var id = edgeOrVertex.id;
                        var outV = edgeOrVertex.outV;
                        var inV = edgeOrVertex.inV;
                        var inVs = edgeOrVertex.inVs;

                        if (id is not null)
                        {
                            edgesById.Add(id.Value, edgeOrVertex);
                        }

                        if (outV is not null)
                        {
                            if (!edgesByOutVertexId.TryGetValue(outV.Value, out var outVEdgesSet))
                            {
                                outVEdgesSet = edgesByOutVertexId[outV.Value] = new HashSet<EdgeOrVertex>();
                            }
                            outVEdgesSet.Add(edgeOrVertex);

                            if (inV is not null)
                            {
                                if (!edgesByInVertexId.TryGetValue(inV.Value, out var inVIdEdgesSet))
                                {
                                    inVIdEdgesSet = edgesByInVertexId[inV.Value] = new HashSet<EdgeOrVertex>();
                                }
                                inVIdEdgesSet.Add(edgeOrVertex);
                            }
                            else
                            {
                                foreach (var inV2 in inVs)
                                {
                                    if (!edgesByInVertexId.TryGetValue(inV2, out var inVIdEdgesSet))
                                    {
                                        inVIdEdgesSet = edgesByInVertexId[inV2] = new HashSet<EdgeOrVertex>();
                                    }
                                    inVIdEdgesSet.Add(edgeOrVertex);
                                }
                            }
                        }
                    }
                }
            }

            return new LsifGraph(verticiesById, edgesById, edgesByOutVertexId, edgesByInVertexId);
        }

        public IReadOnlyDictionary<int, EdgeOrVertex> VerticiesById { get; }

        public IReadOnlyDictionary<int, EdgeOrVertex> EdgesById { get; }

        public IReadOnlyDictionary<int, HashSet<EdgeOrVertex>> EdgesByOutVertexId { get; }

        public IReadOnlyDictionary<int, HashSet<EdgeOrVertex>> EdgesByInVertexId { get; }

        public record EdgeOrVertex(int? id, string type, string label, int? outV, int? inV, int[] inVs, Uri uri, Position start, Position end, string? identifier, string? name);

        public record Position(int? line, int? character);
    }
}
