// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types.ResourceValue
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types
{
  internal sealed class ResourceValue
  {
    public ResourceValue()
    {
      this.Size = (ushort) 0;
      this.Type = ResourceValueTypes.None;
      this.Data = 0U;
    }

    public ushort Size { get; private set; }

    public ResourceValueTypes Type { get; private set; }

    public uint Data { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResourceValue - Size: {0}, Type: {1}, Data: {2}", new object[3]
    {
      (object) this.Size,
      (object) this.Type,
      (object) this.Data
    });

    public void Parse(StreamDecoder streamDecoder)
    {
      this.Size = streamDecoder != null ? streamDecoder.ReadUint16() : throw new ArgumentNullException(nameof (streamDecoder));
      ++streamDecoder.Offset;
      this.Type = (ResourceValueTypes) streamDecoder.ReadByte();
      this.Data = streamDecoder.ReadUint32();
    }
  }
}
