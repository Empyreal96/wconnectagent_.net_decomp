// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.TypeSpecChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  internal sealed class TypeSpecChunk : Chunk
  {
    public TypeSpecChunk()
    {
      this.ChunkType = ChunkType.ResTableTypeSpecType;
      this.Id = 0U;
      this.EntryCount = 0U;
      this.EntryFlags = new List<uint>();
    }

    public uint Id { get; private set; }

    public uint EntryCount { get; private set; }

    public List<uint> EntryFlags { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TypeSpecChunk - Id: {0}, EntryCount: {1}", new object[2]
      {
        (object) this.Id,
        (object) this.EntryCount
      }));
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\nEntry Flags:\n");
      foreach (uint entryFlag in this.EntryFlags)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "\t{0}\n", new object[1]
        {
          (object) entryFlag
        });
      return stringBuilder.ToString();
    }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      this.Id = (uint) streamDecoder.ReadByte();
      streamDecoder.Offset += 3U;
      this.EntryCount = streamDecoder.ReadUint32();
      for (uint index = 0; index < this.EntryCount; ++index)
        this.EntryFlags.Add(streamDecoder.ReadUint32());
    }
  }
}
