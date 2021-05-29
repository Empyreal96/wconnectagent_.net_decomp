// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.FieldMetadata
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Text;

namespace Microsoft.Diagnostics.Tracing
{
  internal class FieldMetadata
  {
    private readonly string name;
    private readonly int nameSize;
    private readonly EventFieldTags tags;
    private readonly byte[] custom;
    private readonly ushort fixedCount;
    private byte inType;
    private byte outType;

    public FieldMetadata(
      string name,
      TraceLoggingDataType type,
      EventFieldTags tags,
      bool variableCount)
      : this(name, type, tags, variableCount ? (byte) 64 : (byte) 0, (ushort) 0, (byte[]) null)
    {
    }

    public FieldMetadata(
      string name,
      TraceLoggingDataType type,
      EventFieldTags tags,
      ushort fixedCount)
      : this(name, type, tags, (byte) 32, fixedCount)
    {
    }

    public FieldMetadata(
      string name,
      TraceLoggingDataType type,
      EventFieldTags tags,
      byte[] custom)
      : this(name, type, tags, (byte) 96, custom == null ? (ushort) 0 : checked ((ushort) custom.Length), custom)
    {
    }

    private FieldMetadata(
      string name,
      TraceLoggingDataType dataType,
      EventFieldTags tags,
      byte countFlags,
      ushort fixedCount = 0,
      byte[] custom = null)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name), "This usually means that the object passed to Write is of a type that does not support being used as the top-level object in an event, e.g. a primitive or built-in type.");
      Statics.CheckName(name);
      int num = (int) (dataType & (TraceLoggingDataType) 31);
      this.name = name;
      this.nameSize = Encoding.UTF8.GetByteCount(this.name) + 1;
      this.inType = (byte) ((uint) num | (uint) countFlags);
      this.outType = (byte) ((int) dataType >> 8 & (int) sbyte.MaxValue);
      this.tags = tags;
      this.fixedCount = fixedCount;
      this.custom = custom;
      if (countFlags != (byte) 0)
      {
        if (num == 0)
          throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedArrayOfNil"));
        if (num == 14)
          throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedArrayOfBinary"));
        if (num == 1 || num == 2)
          throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_NotSupportedArrayOfNullTerminatedString"));
      }
      if ((this.tags & (EventFieldTags) 268435455) != EventFieldTags.None)
        this.outType |= (byte) 128;
      if (this.outType == (byte) 0)
        return;
      this.inType |= (byte) 128;
    }

    public void IncrementStructFieldCount()
    {
      this.inType |= (byte) 128;
      ++this.outType;
      if (((int) this.outType & (int) sbyte.MaxValue) == 0)
        throw new NotSupportedException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_TooManyFields"));
    }

    public void Encode(ref int pos, byte[] metadata)
    {
      if (metadata != null)
        Encoding.UTF8.GetBytes(this.name, 0, this.name.Length, metadata, pos);
      pos += this.nameSize;
      if (metadata != null)
        metadata[pos] = this.inType;
      ++pos;
      if (((int) this.inType & 128) != 0)
      {
        if (metadata != null)
          metadata[pos] = this.outType;
        ++pos;
        if (((int) this.outType & 128) != 0)
          Statics.EncodeTags((int) this.tags, ref pos, metadata);
      }
      if (((int) this.inType & 32) == 0)
        return;
      if (metadata != null)
      {
        metadata[pos] = (byte) this.fixedCount;
        metadata[pos + 1] = (byte) ((uint) this.fixedCount >> 8);
      }
      pos += 2;
      if (96 != ((int) this.inType & 96) || this.fixedCount == (ushort) 0)
        return;
      if (metadata != null)
        Buffer.BlockCopy((Array) this.custom, 0, (Array) metadata, pos, (int) this.fixedCount);
      pos += (int) this.fixedCount;
    }
  }
}
