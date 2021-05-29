// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.ResourceKey
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  internal sealed class ResourceKey
  {
    private const ushort ResTableEntryInBytes = 8;
    private const ushort ResTableMapEntryInBytes = 16;

    public ResourceKey()
    {
      this.Size = 0U;
      this.Flag = 0U;
      this.Key = 0U;
      this.Parent = 0U;
      this.Count = 0U;
    }

    public uint Size { get; private set; }

    public uint Flag { get; private set; }

    public uint Key { get; private set; }

    public uint Parent { get; private set; }

    public uint Count { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResourceKey - Size: {0}, Flag: {1}, Key: {2}, Parent: {3}, Count: {4}", (object) this.Size, (object) this.Flag, (object) this.Key, (object) this.Parent, (object) this.Count);

    public void Parse(StreamDecoder streamDecoder)
    {
      ushort num = streamDecoder.ReadUint16();
      switch (num)
      {
        case 8:
        case 16:
          this.Size = (uint) num;
          this.Flag = (uint) streamDecoder.ReadUint16();
          this.Key = streamDecoder.ReadUint32();
          if (this.Size != 16U)
            break;
          this.Parent = streamDecoder.ReadUint32();
          this.Count = streamDecoder.ReadUint32();
          break;
        default:
          throw new ApkDecoderResourcesException("Resource Table size in bytes has unexpected value: " + (object) num);
      }
    }

    public bool IsComplexValue() => ((int) this.Flag & 1) != 0;
  }
}
