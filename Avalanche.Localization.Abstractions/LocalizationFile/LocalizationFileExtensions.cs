// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Runtime.CompilerServices;

/// <summary>Extension methods for <see cref="ILocalized{T}"/></summary>
public static class LocalizationFileExtensions
{
    /// <summary>Open <paramref name="localizationFile"/> to resource.</summary>
    /// <exception cref="IOException">On unexpected error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream Open(this ILocalizationFile localizationFile) => localizationFile.TryOpen(out Stream? stream) ? stream : throw new IOException($"Could not open {localizationFile.FileName}");

    /// <summary>Read file</summary>
    /// <exception cref="IOException">On unexpected error.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadFully(this ILocalizationFile localizationFile)
    {
        // Try open
        if (!localizationFile.TryOpen(out Stream? stream)) throw new IOException($"Could not open {localizationFile.FileName}");
        //
        try
        {
            // Read
            byte[] data = ReadFully(stream);
            // 
            return data;
        } finally
        {
            stream?.Dispose();
        }
    }

    /// <summary>Read bytes from <paramref name="stream"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadFully(Stream stream)
    {
        if (stream == null) return null!;

        // Get length
        long length;
        try
        {
            length = stream.Length;
        }
        catch (NotSupportedException)
        {
            // Cannot get length
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        // Assert fits to byte[]
        if (length > int.MaxValue) throw new IOException("Stream length over 2GB");

        // Create buffer
        int _len = (int)length;
        byte[] data = new byte[_len];

        // Read chunks
        int ix = 0;
        while (ix < _len)
        {
            int count = stream.Read(data, ix, _len - ix);

            // "returns zero (0) if the end of the stream has been reached."
            if (count == 0) break;

            ix += count;
        }
        if (ix == _len) return data;
        throw new IOException("Failed to read stream fully");
    }

}
