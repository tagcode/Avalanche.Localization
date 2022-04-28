// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Linq;

/// <summary>Flattens tree structured xml file into flat lines of the intermediate format <![CDATA[KeyValuePair<string, MarkedText>]]>.</summary>
public abstract class LocalizationReaderXml : ReadOnlyAssignableClass, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, ILocalizationFileName
{
    /// <summary>Create xml element</summary>
    protected abstract XElement CreateElement();
    /// <summary>Filename</summary>
    protected string? filename;
    /// <summary>Filename</summary>
    public string? FileName { get => filename; set => this.AssertWritable().filename = value; }

    /// <summary>Convert <paramref name="attribute"/> to <see cref="TextRange"/>.</summary>
    protected virtual TextRange ConvertToTextRange(XAttribute? attribute)
    {
        if (attribute == null) return default!;
        IXmlLineInfo startInfo = attribute, endInfo = attribute;

        if (attribute.NextAttribute != null) endInfo = attribute.NextAttribute;
        else if (attribute.Parent?.NextNode != null) endInfo = attribute.Parent.NextNode;

        TextPosition start = new TextPosition(startInfo.LineNumber, startInfo.LineNumber);
        TextPosition end = new TextPosition(endInfo.LineNumber, endInfo.LineNumber);
        return new TextRange(filename, start, end);
    }

    /// <summary>Convert <paramref name="node"/> to <see cref="TextRange"/>.</summary>
    protected virtual TextRange ConvertToTextRange(XNode? node)
    {
        if (node == null) return default!;
        IXmlLineInfo startInfo = node, endInfo = node;
        if (node.NextNode != null) endInfo = node.NextNode;
        TextPosition start = new TextPosition(startInfo.LineNumber, startInfo.LineNumber);
        TextPosition end = new TextPosition(endInfo.LineNumber, endInfo.LineNumber);
        return new TextRange(filename, start, end);
    }

    /// <summary>Convert <paramref name="firstNode"/>..<paramref name="endNode"/> to <see cref="TextRange"/>.</summary>
    protected virtual TextRange ConvertToTextRange(XNode? firstNode, XNode? endNode)
    {
        IXmlLineInfo? startInfo = firstNode, endInfo = endNode;
        if (endNode?.NextNode != null) endInfo = endNode.NextNode;
        TextPosition start = startInfo == null ? default : new TextPosition(startInfo.LineNumber, startInfo.LineNumber);
        TextPosition end = endInfo == null ? default : new TextPosition(endInfo.LineNumber, endInfo.LineNumber);
        return new TextRange(filename, start, end);
    }

    /// <summary>Convert <paramref name="attribute"/> to <see cref="MarkedText"/>.</summary>
    protected virtual MarkedText ConvertToMarkedText(XAttribute attribute) 
        => new MarkedText(attribute.Value, ConvertToTextRange(attribute));

    /// <summary>Read lines</summary>
    /// <exception cref="Exception">On read error</exception>
    public virtual IEnumerator<IEnumerable<KeyValuePair<string, MarkedText>>> GetEnumerator()
    {
        // Create xml stream
        XElement root = CreateElement();        
        // Create Stack
        List<(XNode?, LocalizationNode?)> queue = new(25);
        //
        StringBuilder sb = new StringBuilder();
        //
        List<XElement> tmp = new List<XElement>(20);
        // Add documents into queue
        queue.Add((root, new LocalizationNode(null, default, null)));
        // Process queue
        while (queue.Count > 0)
        {
            // Get next item
            int ix = queue.Count - 1;
            (XNode? xml, LocalizationNode? node0) = queue[ix]; queue.RemoveAt(ix);
            // No node
            if (xml == null) continue;
            // <Element>
            if (xml is XContainer container)
            {
                // Build node here
                LocalizationNode? node1 = node0;
                // Get attributes
                IEnumerable<XAttribute>? attributes = xml is XElement xelement ? xelement.Attributes() : null;
                // Concatenate XAttributes
                if (attributes != null)
                {
                    foreach(XAttribute attribute in attributes)
                    {
                        // Append node
                        node1 = new LocalizationNode(attribute.Name.LocalName, ConvertToMarkedText(attribute), node1 ?? node0);
                    }
                }
                //
                sb.Clear();
                tmp.Clear();
                XNode? firstTextNode = null!, lastTextNode = null!;
                // Visit children
                foreach(XNode child in container.Nodes())
                {
                    // text node
                    if (child is XText text0)
                    {
                        // Assign nodes
                        if (firstTextNode == null) firstTextNode = text0;
                        lastTextNode = text0;
                        // Append text
                        sb.Append(text0.Value.Trim());
                    }
                    // text node
                    else if (child is XElement xelement0) tmp.Add(xelement0);
                }
                // Yield concatenated text parts
                if (sb.Length>0 && container is XElement xelement1)
                {
                    // Append node
                    node1 = new LocalizationNode(xelement1.Name.LocalName, new MarkedText(sb.ToString(), ConvertToTextRange(firstTextNode, lastTextNode)), node1 ?? node0);
                    // Yield node
                    yield return node1.ToArray();
                }
                // Visit children
                for (int j=tmp.Count-1; j>=0; j--)
                {
                    // Enqueue
                    queue.Add((tmp[j], node1??node0));
                }
            }            
        }
    }

    /// <summary>Read lines</summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}()";

    /// <summary>File reader</summary>
    public class File : LocalizationReaderXml
    {
        /// <summary>Create uninitialized file</summary>
        public File() : base() { }
        /// <summary>Create <paramref name="filename"/> reader</summary>
        public File(string filename) : base()
        {
            this.filename = filename;
        }

        /// <summary>Open stream to associated file</summary>
        protected override XElement CreateElement()
        {
            // Create file reader
            using var reader = new StreamReader(filename ?? throw new InvalidOperationException("null filename"));
            // Create xml stream
            XDocument document = XDocument.Load(reader);
            //
            return document.Root ?? throw new InvalidOperationException("No root element");
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}({FileName})";
    }

    /// <summary>File reader</summary>
    public class FromStream : LocalizationReaderXml
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
        protected override XElement CreateElement()
        {
            // Move cursor
            stream!.Position = position;
            // Create xml stream
            XDocument document = XDocument.Load(stream);
            //
            return document.Root ?? throw new InvalidOperationException("No root element");
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}";
    }

    /// <summary>Url reader</summary>
    public class Url : LocalizationReaderXml
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
        protected override XElement CreateElement()
        {
            // Create xml stream
            XDocument document = XDocument.Load(uri: documentUrl ?? throw new InvalidOperationException("null url"));
            //
            return document.Root ?? throw new InvalidOperationException("No root element");
        }

        /// <summary>Print information</summary>
        public override string ToString() => $"{GetType().Name}({DocumentUrl})";
    }

    /// <summary>Xml Text reader</summary>
    public class Text : LocalizationReaderXml
    {
        /// <summary>Xml text</summary>
        protected string? text;
        /// <summary>Xml text</summary>
        public virtual string? XmlText { get => text; set => this.AssertWritable().text = value; }

        /// <summary>Create uninitialized</summary>
        public Text() : base() { }
        /// <summary>Create <paramref name="text"/> reader</summary>
        public Text(string text) : base()
        {
            this.text = text;
        }

        /// <summary>Open stream to associated file</summary>
        protected override XElement CreateElement()
        {
            // Create file reader
            using var reader = new StringReader(text ?? throw new InvalidOperationException("null"));
            // Create xml stream
            XDocument document = XDocument.Load(reader);
            //
            return document.Root ?? throw new InvalidOperationException("No root element");
        }
    }

    /// <summary>Xml document reader</summary>
    public class Document : LocalizationReaderXml
    {
        /// <summary>Xml document</summary>
        protected XDocument? document;
        /// <summary>Xml text</summary>
        public virtual XDocument? XmlDocument { get => document; set => this.AssertWritable().document = value; }

        /// <summary>Create uninitialized</summary>
        public Document() : base() { }
        /// <summary>Create <paramref name="document"/> reader</summary>
        public Document(XDocument document) : base()
        {
            this.document = document;
        }

        /// <summary>Return document</summary>
        protected override XElement CreateElement() => document?.Root ?? throw new InvalidOperationException("null document");
    }

    /// <summary><see cref="XElement"/> reader</summary>
    public class Element : LocalizationReaderXml
    {
        /// <summary>Xml element</summary>
        protected XElement? element;
        /// <summary>Xml text</summary>
        public virtual XElement? XmlElement { get => element; set => this.AssertWritable().element = value; }

        /// <summary>Create uninitialized</summary>
        public Element() : base() { }
        /// <summary>Create <paramref name="element"/> reader</summary>
        public Element(XElement element) : base()
        {
            this.element = element;
        }

        /// <summary>Return document</summary>
        protected override XElement CreateElement() => element ?? throw new InvalidOperationException("null document");
    }
}

