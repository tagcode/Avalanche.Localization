// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Collections;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using YamlDotNet.Helpers;
using YamlDotNet.RepresentationModel;

/// <summary>Flattens tree structured yaml file into flat lines of the intermediate format <![CDATA[KeyValuePair<string, MarkedText>]]>.</summary>
public abstract class LocalizationReaderYaml : ReadOnlyAssignableClass, ILocalizationLinesReader
{
    /// <summary>Create yaml stream</summary>
    protected abstract IEnumerable<YamlNode> CreateNodes();
    /// <summary>Filename</summary>
    protected string? filename;
    /// <summary>Filename</summary>
    public string? FileName { get => filename; set => this.AssertWritable().filename = value; }

    /// <summary>Convert <paramref name="node"/> to <see cref="TextRange"/>.</summary>
    public TextRange ConvertToTextRange(YamlNode node)
    {
        TextPosition start = new TextPosition(node.Start.Line, node.Start.Column, node.Start.Index);
        TextPosition end = new TextPosition(node.End.Line, node.End.Column, node.End.Index);
        return new TextRange(filename, start, end);
    }

    /// <summary>Convert <paramref name="node"/> to <see cref="MarkedText"/>.</summary>
    protected virtual MarkedText ConvertToMarkedText(YamlScalarNode node) => new MarkedText(node.Value ?? "", ConvertToTextRange(node));

    /// <summary>Read lines</summary>
    /// <exception cref="Exception">On read error</exception>
    public virtual IEnumerator<IEnumerable<KeyValuePair<string, MarkedText>>> GetEnumerator()
    {
        // Create yaml stream
        IEnumerable<YamlNode> rootNodes = CreateNodes();
        // Create Stack
        List<(YamlNode, LocalizationNode?)> queue = new(25);
        //
        List<YamlNode> tmp = new List<YamlNode>(20);
        // Add documents into queue
        foreach (YamlNode rootNode in rootNodes) queue.Add((rootNode, new LocalizationNode(null, default, null)));
        // Process queue
        while (queue.Count > 0)
        {
            // Get next item
            int ix = queue.Count - 1;
            //int ix = 0;
            (YamlNode yaml, LocalizationNode? node0) = queue[ix]; queue.RemoveAt(ix);
            // Open-ended key without value at higher level: Key = null
            string? openKey = node0 == null ? null : !node0.value.HasValue ? node0.key : null;
            // Queue children
            if (yaml is YamlScalarNode scalarNode0)
            {
                // Got key and value
                if (openKey != null)
                {
                    // Key+Value
                    LocalizationNode node1 = new LocalizationNode(openKey, ConvertToMarkedText(scalarNode0), node0?.parent);
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
            else if (yaml is YamlSequenceNode sequenceNode)
            {
                // Get children
                IList<YamlNode> children = sequenceNode.Children;
                // Tmp buffer
                tmp.Clear();
                // Use parent of open-ended node
                LocalizationNode? node1 = null;
                // Concatenate all values into one node
                foreach (var child in children)
                {
                    // Not scalar
                    if (child is YamlScalarNode scalarNode1)
                    {
                        // Append node
                        node1 = new LocalizationNode(openKey, ConvertToMarkedText(scalarNode1), node1 ?? node0?.parent);
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
                    YamlNode child = tmp[j];
                    // Queue
                    queue.Add((child, node1 ?? node0));
                }
            }
            // { key: value, ... }
            else if (yaml is YamlMappingNode mappingNode)
            {
                // Get children
                IOrderedDictionary<YamlNode, YamlNode> children = mappingNode.Children;
                //
                int nonScalarCount = 0;
                // Use parent of open-ended node
                LocalizationNode? node1 = null;
                // Visit scalar nodes append them
                foreach (var kv in children)
                {
                    //
                    string? key = kv.Key is YamlScalarNode key0 ? key0.Value : null;
                    // 
                    if (kv.Value is not YamlScalarNode scalarNode2) { nonScalarCount++; continue; }
                    // Create node
                    node1 = new LocalizationNode(key, ConvertToMarkedText(scalarNode2), node1 ?? node0?.parent);
                }
                // Yield node
                if (nonScalarCount == 0 && node1 != null) { yield return node1.ToArray(); continue; }
                // Visit non-scalar nodes
                for (int j = children.Count - 1; j >= 0; j--)
                {
                    // Get key-value pair 
                    var kv = children[j];
                    //
                    string? key = kv.Key is YamlScalarNode key0 ? key0.Value : null;
                    // 
                    if (kv.Value is YamlScalarNode) continue;
                    // Create node
                    LocalizationNode node2 = new LocalizationNode(key, default, node1 ?? node0?.parent);
                    // Queue value node
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
    public class File : LocalizationReaderYaml, ILocalizationFileName
    {
        /// <summary>Create uninitialized file</summary>
        public File() : base() { }
        /// <summary>Create <paramref name="filename"/> reader</summary>
        public File(string filename) : base()
        {
            this.filename = filename;
        }

        /// <summary>Open stream to associated file</summary>
        protected override IEnumerable<YamlNode> CreateNodes()
        {
            // Create file reader
            using var reader = new StreamReader(filename ?? throw new InvalidOperationException("null filename"));
            // Create yaml stream
            YamlStream yamlStream = new YamlStream();
            // Load file into stream
            yamlStream.Load(reader);
            // Return root nodes
            return yamlStream.Documents.Select(d => d.RootNode).ToArray();
        }
        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}({FileName})";
    }

    /// <summary>File reader</summary>
    public class FromStream : LocalizationReaderYaml
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
        protected override IEnumerable<YamlNode> CreateNodes()
        {
            // Move cursor
            stream!.Position = position;
            // Create file reader
            using var reader = new StreamReader(stream);
            // Create yaml stream
            YamlStream yamlStream = new YamlStream();
            // Load file into stream
            yamlStream.Load(reader);
            // Return root nodes
            return yamlStream.Documents.Select(d => d.RootNode).ToArray();
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}";
    }

    /// <summary>Url reader</summary>
    public class Url : LocalizationReaderYaml
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
        protected override IEnumerable<YamlNode> CreateNodes()
        {
            // Download document
            string text = new HttpClient().GetStringAsync(documentUrl ?? throw new InvalidOperationException("null url")).Result;
            // Create reader
            StringReader reader = new StringReader(text);
            // Create yaml stream
            YamlStream yamlStream = new YamlStream();
            // Load file into stream
            yamlStream.Load(reader);
            // Return root nodes
            return yamlStream.Documents.Select(d => d.RootNode).ToArray();
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}({DocumentUrl})";
    }


    /// <summary>Yaml Text reader</summary>
    public class Text : LocalizationReaderYaml
    {
        /// <summary>Yaml text</summary>
        protected string? yamlText;
        /// <summary>Yaml text</summary>
        public virtual string? YamlText { get => yamlText; set => this.AssertWritable().yamlText = value; }

        /// <summary>Create uninitialized</summary>
        public Text() : base() { }
        /// <summary>Create <paramref name="yamlText"/> reader</summary>
        public Text(string yamlText) : base()
        {
            this.yamlText = yamlText;
        }

        /// <summary>Open stream to associated file</summary>
        protected override IEnumerable<YamlNode> CreateNodes()
        {
            // No assigned file
            if (yamlText == null) throw new ArgumentNullException(nameof(YamlText));
            // Create file reader
            using var reader = new StringReader(yamlText);
            // Create yaml stream
            YamlStream yamlStream = new YamlStream();
            // Load file into stream
            yamlStream.Load(reader);
            // Return root nodes
            return yamlStream.Documents.Select(d => d.RootNode).ToArray();
        }
    }

    /// <summary>Yaml document reader</summary>
    public class Document : LocalizationReaderYaml
    {
        /// <summary>Yaml document</summary>
        protected YamlDocument? yamlDocument;
        /// <summary>Yaml text</summary>
        public virtual YamlDocument? YamlDocument { get => yamlDocument; set => this.AssertWritable().yamlDocument = value; }

        /// <summary>Create uninitialized</summary>
        public Document() : base() { }
        /// <summary>Create <paramref name="yamlDocument"/> reader</summary>
        public Document(YamlDocument yamlDocument) : base()
        {
            this.yamlDocument = yamlDocument;
        }

        /// <summary>Open stream to associated file</summary>
        protected override IEnumerable<YamlNode> CreateNodes()
        {
            // No assigned file
            if (yamlDocument == null) throw new ArgumentNullException(nameof(YamlDocument));
            // Create yaml stream
            YamlStream yamlStream = new YamlStream(yamlDocument);
            // Return root nodes
            return yamlStream.Documents.Select(d => d.RootNode).ToArray();
        }
    }

    /// <summary>Yaml node reader</summary>
    public class Node : LocalizationReaderYaml
    {
        /// <summary>Yaml node</summary>
        protected YamlNode? node;
        /// <summary>Yaml text</summary>
        public virtual YamlNode? YamlRoot { get => node; set => this.AssertWritable().node = value; }

        /// <summary>Create uninitialized</summary>
        public Node() : base() { }
        /// <summary>Create <paramref name="node"/> reader</summary>
        public Node(YamlNode node) : base()
        {
            this.node = node;
        }

        /// <summary>Open stream to associated file</summary>
        protected override IEnumerable<YamlNode> CreateNodes()
        {
            // No assigned file
            if (node == null) throw new ArgumentNullException(nameof(YamlRoot));
            // Return root nodes
            return new YamlNode[] { node };
        }
    }
}

