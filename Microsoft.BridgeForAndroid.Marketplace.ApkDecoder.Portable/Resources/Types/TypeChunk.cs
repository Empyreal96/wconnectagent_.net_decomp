// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.TypeChunk
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
  internal sealed class TypeChunk : Chunk
  {
    private const uint ResTableTypeNoEntry = 4294967295;

    public TypeChunk()
    {
      this.ChunkType = ChunkType.ResTableTypeType;
      this.Id = 0U;
      this.EntryCount = 0U;
      this.EntriesStart = 0U;
      this.Config = new ResourceConfig();
      this.ResourceItems = new Dictionary<uint, ResourceItem>();
    }

    public uint Id { get; private set; }

    public uint EntryCount { get; private set; }

    public uint EntriesStart { get; private set; }

    public ResourceConfig Config { get; private set; }

    public Dictionary<uint, ResourceItem> ResourceItems { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TypeChunk - Id: {0}, EntryCount: {1}, EntriesStart: {2}, Config: {3}, Resource Items Count: {4}\n", (object) this.Id, (object) this.EntryCount, (object) this.EntriesStart, (object) this.Config, (object) this.ResourceItems.Count));
      stringBuilder.AppendLine("Resource Items:");
      foreach (KeyValuePair<uint, ResourceItem> resourceItem in this.ResourceItems)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", new object[2]
        {
          (object) resourceItem.Key,
          (object) resourceItem.Value
        });
      return stringBuilder.ToString();
    }

    protected override void ParseBody(StreamDecoder streamDecoder)
    {
      this.Id = (uint) streamDecoder.ReadByte();
      streamDecoder.Offset += 3U;
      this.EntryCount = streamDecoder.ReadUint32();
      this.EntriesStart = streamDecoder.ReadUint32();
      this.Config.Parse(streamDecoder);
      for (uint key = 0; key < this.EntryCount; ++key)
      {
        uint num = streamDecoder.ReadUint32();
        if (num != uint.MaxValue)
        {
          uint offset = streamDecoder.Offset;
          streamDecoder.Offset = this.BaseOffset + this.EntriesStart + num;
          ResourceItem resourceItem = new ResourceItem();
          resourceItem.Parse(streamDecoder);
          this.ResourceItems.Add(key, resourceItem);
          streamDecoder.Offset = offset;
        }
      }
    }
  }
}
