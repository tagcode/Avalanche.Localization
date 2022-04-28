// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Collections;
using System.Text.Json.Nodes;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;

/// <summary>Flattens tree structured json file into flat lines of the intermediate format <![CDATA[KeyValuePair<string, MarkedText>]]>.</summary>
public abstract class LocalizationReaderJson : ReadOnlyAssignableClass, ILocalizationFileName, ILocalizationLinesReader
{
    /// <summary>Create xml document</summary>
    protected abstract JsonNode CreateNode();
    /// <summary>Filename</summary>
    protected string? filename;
    /// <summary>Filename</summary>
    public string? FileName { get => filename; set => this.AssertWritable().filename = value; }

    /// <summary>Convert <paramref name="value"/> to marked text.</summary>
    protected virtual MarkedText ConvertToMarkedText(JsonValue value)
        => new MarkedText(value.ToString(), filename, new TextPosition(), new TextPosition());

    /// <summary>Read lines</summary>
    /// <exception cref="Exception">On read error</exception>
    public virtual IEnumerator<IEnumerable<KeyValuePair<string, MarkedText>>> GetEnumerator()
    {
        // Create yaml stream
        JsonNode document = CreateNode();
        // Create Stack
        List<(JsonNode, LocalizationNode?)> queue = new(25);
        //
        List<JsonNode> tmp = new(20);
        List<KeyValuePair<string, JsonNode>> tmp2 = new(20);
        // Add documents into queue
        queue.Add((document, new LocalizationNode(null, default, null)));
        // Process queue
        while (queue.Count > 0)
        {
            // Get next item
            int ix = queue.Count - 1;
            //int ix = 0;
            (JsonNode json, LocalizationNode? node0) = queue[ix]; queue.RemoveAt(ix);
            // Open-ended key without value at higher level: Key = null
            string? openKey = node0 == null ? null : !node0.value.HasValue ? node0.key : null;
            // Queue children
            if (json is JsonValue value0)
            {
                // Got key and value
                if (openKey != null)
                {
                    // Key+Value
                    LocalizationNode node1 = new LocalizationNode(openKey, ConvertToMarkedText(value0), node0?.parent);
                    // Yield node
                    if (node1 != null) yield return node1.ToArray();
                }
                // Got value, but no key
                else
                {
                    // Yield node
                    if (node0 != null) yield return node0.ToArray();
                }
            }
            // [ value, ... ]
            else if (json is JsonArray array)
            {
                // Tmp buffer
                tmp.Clear();
                // Get children
                IList<JsonNode?> children = array;
                // Use parent of open-ended node
                LocalizationNode? node1 = null;
                // Concatenate all values into one node
                foreach (var child in children)
                {
                    // Null
                    if (child == null) continue;
                    // Not scalar
                    if (child is JsonValue value1)
                    {
                        // Append node
                        node1 = new LocalizationNode(openKey, ConvertToMarkedText(value1), node1 ?? node0?.parent);
                    }

                    // To be processed below
                    else { tmp.Add(child); }

                }
                // Yield node
                if (tmp.Count == 0 && node1 != null) { yield return node1.ToArray(); continue; }
                // Forward to non scalars                    
                for (int j = tmp.Count - 1; j >= 0; j--)
                {
                    // Get child
                    JsonNode child = tmp[j];
                    // Queue
                    queue.Add((child, node1 ?? node0));
                }
            }
            // { key: value, ... }
            else if (json is JsonObject mappingNode)
            {
                // Place here non-scalar values
                tmp2.Clear();
                // Use parent of open-ended node
                LocalizationNode? node1 = null;
                // Visit scalar nodes append them
                foreach (var kv in mappingNode)
                {
                    //
                    if (kv.Value == null) continue;
                    // 
                    if (kv.Value is JsonValue value2)
                    {
                        //
                        string key = kv.Key;
                        // Create node
                        node1 = new LocalizationNode(key, ConvertToMarkedText(value2), node1 ?? node0?.parent);
                    }
                    // Process later
                    else { tmp2.Add(kv!); }
                }
                // Yield node
                if (tmp2.Count == 0 && node1 != null) { yield return node1.ToArray(); continue; }
                // Visit non-scalar nodes
                for (int j = tmp2.Count - 1; j >= 0; j--)
                {
                    // Get key-value pair 
                    var kv = tmp2[j];
                    //
                    string key = kv.Key;
                    // Create node
                    LocalizationNode node2 = new LocalizationNode(key, default, node1 ?? node0?.parent);
                    // Enqueue
                    queue.Add((kv.Value, node2));
                }
            }
        }
    }

    /// <summary>Read lines</summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}()";

    /// <summary>File reader</summary>
    public class File : LocalizationReaderJson
    {
        /// <summary>Create uninitialized file</summary>
        public File() : base() { }
        /// <summary>Create <paramref name="filename"/> reader</summary>
        public File(string filename) : base()
        {
            this.filename = filename;
        }

        /// <summary>Open stream to associated file</summary>
        protected override JsonNode CreateNode()
        {
            // Create file reader
            string text = System.IO.File.ReadAllText(filename ?? throw new InvalidOperationException("null filename"));
            // Create json root node
            JsonNode document = JsonNode.Parse(text) ?? (JsonNode)""!;
            // Return node
            return document;
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}({FileName})";
    }

    /// <summary>File reader</summary>
    public class FromStream : LocalizationReaderJson
    {
        /// <summary>UTF-8 Stream</summary>
        protected Stream? stream;
        /// <summary>Initial position</summary>
        protected long position = 0L;
        /// <summary>UTF-8 Stream</summary>
        public virtual Stream? Stream { get => stream; set => this.AssertWritable().stream = value; }
        /// <summary>Initial position</summary>
        public virtual long Position { get => position; set => this.AssertWritable().position = value; }

        /// <summary>Create uninitialized file</summary>
        public FromStream() : base() { }
        /// <summary>Create <paramref name="stream"/> reader</summary>
        /// <param name="stream">UTF-8 stream. Caller must dispose <paramref name="stream"/>.</param>
        public FromStream(Stream stream) : base()
        {
            this.stream = stream;
            this.Position = stream.Position;
        }

        /// <summary>Open stream to associated file</summary>
        protected override JsonNode CreateNode()
        {
            // Move cursor
            stream!.Position = position;
            // Create json root node
            JsonNode document = JsonNode.Parse(stream) ?? (JsonNode)""!;
            // Return node
            return document;
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}";
    }

    /// <summary>Url reader</summary>
    public class Url : LocalizationReaderJson
    {
        /// <summary>Url to read</summary>
        protected string? documentUrl;
        /// <summary>Url to read</summary>
        public virtual string? DocumentUrl { get => documentUrl; set => this.AssertWritable().documentUrl = value; }

        /// <summary>Create uninitialized file</summary>
        public Url() : base() { }
        /// <summary>Create <paramref name="documentUrl"/> reader</summary>
        public Url(string documentUrl) : base()
        {
            this.documentUrl = documentUrl;
        }

        /// <summary>Open stream to associated file</summary>
        protected override JsonNode CreateNode()
        {
            // Download document
            string text = new HttpClient().GetStringAsync(documentUrl ?? throw new InvalidOperationException("null url")).Result;
            // Create json root node
            JsonNode document = JsonNode.Parse(text) ?? (JsonNode)""!;
            // Return node
            return document;
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}({DocumentUrl})";
    }

    /// <summary>Json Text reader</summary>
    public class Text : LocalizationReaderJson
    {
        /// <summary>Json text</summary>
        protected string? text;
        /// <summary>Json text</summary>
        public virtual string? JsonText { get => text; set => this.AssertWritable().text = value; }

        /// <summary>Create uninitialized</summary>
        public Text() : base() { }
        /// <summary>Create <paramref name="text"/> reader</summary>
        public Text(string text) : base()
        {
            this.text = text;
        }

        /// <summary>Open stream to associated file</summary>
        protected override JsonNode CreateNode()
        {
            // Create yaml stream
            JsonNode document = JsonNode.Parse(text ?? throw new InvalidOperationException("null")) ?? (JsonNode)""!;
            //
            return document;
        }
    }

    /// <summary>Json document reader</summary>
    public class Node : LocalizationReaderJson
    {
        /// <summary>Json document</summary>
        protected JsonNode? document;
        /// <summary>Json text</summary>
        public virtual JsonNode? JsonDocument { get => document; set => this.AssertWritable().document = value; }

        /// <summary>Create uninitialized</summary>
        public Node() : base() { }
        /// <summary>Create <paramref name="jsonDocument"/> reader</summary>
        public Node(JsonNode jsonDocument) : base()
        {
            this.document = jsonDocument;
        }

        /// <summary>Return document</summary>
        protected override JsonNode CreateNode() => document ?? throw new InvalidOperationException("null document");
    }
}

