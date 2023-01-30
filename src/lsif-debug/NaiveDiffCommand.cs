using System.CommandLine;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace lsif_debug
{
    internal sealed class NaiveDiffCommand : Command
    {
        private new const string Name = "naive-diff";
        private new const string Description = "Normalizes two LSIF files by removing unique IDs and sorting the lines so they may be more easily diffed for added or removed vertexes and edges.";

        public NaiveDiffCommand()
            : base(Name, Description)
        {
            var lsifArgument = new Argument<string>("lsif", "Path to a *.lsif file or directory of *.lsif files.");

            Add(lsifArgument);

            this.SetHandler(ExecuteAsync, lsifArgument);
        }

        private async Task ExecuteAsync(string lsifPath)
        {
            if (lsifPath is null)
            {
                await this.InvokeAsync("-h");
                return;
            }

            var lines = await File.ReadAllLinesAsync(lsifPath, CancellationToken.None);

            var lsifGraph = LsifGraph.FromLines(lines);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var node = JsonNode.Parse(line);

                if (node is not null)
                {
                    if ((node["type"]?.AsValue().TryGetValue<string>(out var type) ?? false) &&
                        type == "vertex")
                    {
                        if ((node["label"]?.AsValue().TryGetValue<string>(out var label) ?? false) &&
                            (label == "definitionResult" || label == "referenceResult"))
                        {
                            var id = node["id"].GetValue<int>();

                            var flattenedResults = new List<string>();

                            foreach (var edge in lsifGraph.EdgesByOutVertexId[id])
                            {
                                if (edge.inV is not null)
                                {
                                    var item = lsifGraph.VerticiesById[edge.inV.Value];
                                    if (item.label == "range")
                                    {
                                        flattenedResults.Add($"{item.start.line}, {item.start.character} to {item.end.line}, {item.end.character}");
                                    }
                                }
                                else
                                {
                                    foreach (var inV in edge.inVs)
                                    {
                                        var item = lsifGraph.VerticiesById[inV];
                                        if (item.label == "range")
                                        {
                                            flattenedResults.Add($"{item.start.line}, {item.start.character} to {item.end.line}, {item.end.character}");
                                        }
                                    }
                                }

                                node["flattenedResults"] = new JsonArray(flattenedResults.Select(result => JsonValue.Create(result)).ToArray());
                            }
                        }
                    }

                    if (node["id"] is not null)
                    {
                        node["id"] = "ID";
                    }

                    if (node["outV"] is not null)
                    {
                        node["outV"] = "OUTV";
                    }

                    if (node["inV"] is not null)
                    {
                        node["inV"] = "INV";
                    }

                    if (node["inVs"] is JsonArray inVsArray)
                    {
                        node["inVs"] = "inVs count: " + inVsArray.Count;
                    }

                    if (node["outVs"] is JsonArray outVsArray)
                    {
                        node["inVs"] = "inVs count: " + outVsArray.Count;
                    }

                    if (node["shard"] is not null)
                    {
                        node["shard"] = "SHARD";
                    }
                }

                lines[i] = node?.ToJsonString() ?? string.Empty;
            }

            Array.Sort(lines);

            await File.WriteAllLinesAsync(lsifPath + ".normalized.lsif", lines, CancellationToken.None);
        }
    }
}
