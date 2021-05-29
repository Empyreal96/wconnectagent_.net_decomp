// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.TraceLoggingMetadataCollector
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  internal class TraceLoggingMetadataCollector
  {
    private readonly TraceLoggingMetadataCollector.Impl impl;
    private readonly FieldMetadata currentGroup;
    private int bufferedArrayFieldCount = int.MinValue;

    internal TraceLoggingMetadataCollector() => this.impl = new TraceLoggingMetadataCollector.Impl();

    private TraceLoggingMetadataCollector(TraceLoggingMetadataCollector other, FieldMetadata group)
    {
      this.impl = other.impl;
      this.currentGroup = group;
    }

    internal EventFieldTags Tags { get; set; }

    internal int ScratchSize => (int) this.impl.scratchSize;

    internal int DataCount => (int) this.impl.dataCount;

    internal int PinCount => (int) this.impl.pinCount;

    private bool BeginningBufferedArray => this.bufferedArrayFieldCount == 0;

    public TraceLoggingMetadataCollector AddGroup(string name)
    {
      TraceLoggingMetadataCollector metadataCollector = this;
      if (name != null || this.BeginningBufferedArray)
      {
        FieldMetadata fieldMetadata = new FieldMetadata(name, TraceLoggingDataType.Struct, EventFieldTags.None, this.BeginningBufferedArray);
        this.AddField(fieldMetadata);
        metadataCollector = new TraceLoggingMetadataCollector(this, fieldMetadata);
      }
      return metadataCollector;
    }

    public void AddScalar(string name, TraceLoggingDataType type)
    {
      int size;
      switch (type & (TraceLoggingDataType) 31)
      {
        case TraceLoggingDataType.Int8:
        case TraceLoggingDataType.UInt8:
        case TraceLoggingDataType.Char8:
          size = 1;
          break;
        case TraceLoggingDataType.Int16:
        case TraceLoggingDataType.UInt16:
        case TraceLoggingDataType.Char16:
          size = 2;
          break;
        case TraceLoggingDataType.Int32:
        case TraceLoggingDataType.UInt32:
        case TraceLoggingDataType.Float:
        case TraceLoggingDataType.Boolean32:
        case TraceLoggingDataType.HexInt32:
          size = 4;
          break;
        case TraceLoggingDataType.Int64:
        case TraceLoggingDataType.UInt64:
        case TraceLoggingDataType.Double:
        case TraceLoggingDataType.FileTime:
        case TraceLoggingDataType.HexInt64:
          size = 8;
          break;
        case TraceLoggingDataType.Guid:
        case TraceLoggingDataType.SystemTime:
          size = 16;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (type));
      }
      this.impl.AddScalar(size);
      this.AddField(new FieldMetadata(name, type, this.Tags, this.BeginningBufferedArray));
    }

    public void AddBinary(string name, TraceLoggingDataType type)
    {
      switch (type & (TraceLoggingDataType) 31)
      {
        case TraceLoggingDataType.Binary:
        case TraceLoggingDataType.CountedUtf16String:
        case TraceLoggingDataType.CountedMbcsString:
          this.impl.AddScalar(2);
          this.impl.AddNonscalar();
          this.AddField(new FieldMetadata(name, type, this.Tags, this.BeginningBufferedArray));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (type));
      }
    }

    public void AddArray(string name, TraceLoggingDataType type)
    {
      switch (type & (TraceLoggingDataType) 31)
      {
        case TraceLoggingDataType.Utf16String:
        case TraceLoggingDataType.MbcsString:
        case TraceLoggingDataType.Int8:
        case TraceLoggingDataType.UInt8:
        case TraceLoggingDataType.Int16:
        case TraceLoggingDataType.UInt16:
        case TraceLoggingDataType.Int32:
        case TraceLoggingDataType.UInt32:
        case TraceLoggingDataType.Int64:
        case TraceLoggingDataType.UInt64:
        case TraceLoggingDataType.Float:
        case TraceLoggingDataType.Double:
        case TraceLoggingDataType.Boolean32:
        case TraceLoggingDataType.Guid:
        case TraceLoggingDataType.FileTime:
        case TraceLoggingDataType.HexInt32:
        case TraceLoggingDataType.HexInt64:
        case TraceLoggingDataType.Char8:
        case TraceLoggingDataType.Char16:
          if (this.BeginningBufferedArray)
            throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedNestedArraysEnums"));
          this.impl.AddScalar(2);
          this.impl.AddNonscalar();
          this.AddField(new FieldMetadata(name, type, this.Tags, true));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (type));
      }
    }

    public void BeginBufferedArray()
    {
      this.bufferedArrayFieldCount = this.bufferedArrayFieldCount < 0 ? 0 : throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedNestedArraysEnums"));
      this.impl.BeginBuffered();
    }

    public void EndBufferedArray()
    {
      this.bufferedArrayFieldCount = this.bufferedArrayFieldCount == 1 ? int.MinValue : throw new InvalidOperationException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_IncorrentlyAuthoredTypeInfo"));
      this.impl.EndBuffered();
    }

    public void AddCustom(string name, TraceLoggingDataType type, byte[] metadata)
    {
      if (this.BeginningBufferedArray)
        throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedCustomSerializedData"));
      this.impl.AddScalar(2);
      this.impl.AddNonscalar();
      this.AddField(new FieldMetadata(name, type, this.Tags, metadata));
    }

    internal byte[] GetMetadata()
    {
      byte[] metadata = new byte[this.impl.Encode((byte[]) null)];
      this.impl.Encode(metadata);
      return metadata;
    }

    private void AddField(FieldMetadata fieldMetadata)
    {
      this.Tags = EventFieldTags.None;
      ++this.bufferedArrayFieldCount;
      this.impl.fields.Add(fieldMetadata);
      if (this.currentGroup == null)
        return;
      this.currentGroup.IncrementStructFieldCount();
    }

    private class Impl
    {
      internal readonly List<FieldMetadata> fields = new List<FieldMetadata>();
      internal short scratchSize;
      internal sbyte dataCount;
      internal sbyte pinCount;
      private int bufferNesting;
      private bool scalar;

      public void AddScalar(int size)
      {
        if (this.bufferNesting != 0)
          return;
        if (!this.scalar)
          checked { ++this.dataCount; }
        this.scalar = true;
        checked { this.scratchSize += unchecked ((short) size); }
      }

      public void AddNonscalar()
      {
        if (this.bufferNesting != 0)
          return;
        this.scalar = false;
        checked { ++this.pinCount; }
        checked { ++this.dataCount; }
      }

      public void BeginBuffered()
      {
        if (this.bufferNesting == 0)
          this.AddNonscalar();
        ++this.bufferNesting;
      }

      public void EndBuffered() => --this.bufferNesting;

      public int Encode(byte[] metadata)
      {
        int pos = 0;
        foreach (FieldMetadata field in this.fields)
          field.Encode(ref pos, metadata);
        return pos;
      }
    }
  }
}
