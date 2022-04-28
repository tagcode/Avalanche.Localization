// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities;

/// <summary></summary>
public class LocalizationFileInMemory : LocalizationFileBase
{
    /// <summary></summary>
    protected byte[] data = null!;
    /// <summary></summary>
    public byte[] Data { get => data; set => this.AssertWritable().data = value; }

    /// <summary></summary>
    public LocalizationFileInMemory() : base() { }
    /// <summary></summary>
    public LocalizationFileInMemory(byte[] data) : base() { this.Data = data; }

    /// <summary>Open stream to resource.</summary>
    /// <exception cref="InvalidOperationException"></exception>
    public override bool TryOpen([NotNullWhen(true)] out Stream? stream)
    {
        // Get snapshot
        var _data = Data;
        // No data
        if (_data == null) throw new InvalidOperationException($"No {nameof(Data)}");
        // Return stream
        stream = new MemoryStream(_data);
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        FNVHash32 hash = new FNVHash32();
        var _data = Data;
        if (_data != null) hash.HashIn((ReadOnlySpan<byte>)_data.AsSpan());
        return hash.Hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        if (obj is not LocalizationFileInMemory other) return false;
        var _data1 = this.data;
        var _data2 = other.data;
        if (_data1 == null && _data2 == null) return true;
        if (_data1 == null || _data2 == null) return false;
        if (!System.MemoryExtensions.SequenceEqual((ReadOnlySpan<byte>)_data1.AsSpan(), (ReadOnlySpan<byte>)_data2.AsSpan())) return false;
        return true;
    }
}

