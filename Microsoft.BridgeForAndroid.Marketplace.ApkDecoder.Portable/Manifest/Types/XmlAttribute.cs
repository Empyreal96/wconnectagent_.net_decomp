// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Common;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Types;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal class XmlAttribute
  {
    public XmlAttribute()
    {
      this.Namespace = uint.MaxValue;
      this.Name = 0U;
      this.RawValue = 0U;
      this.Data = new ResourceValue();
    }

    public uint Namespace { get; protected set; }

    public uint Name { get; protected set; }

    public ResourceValue Data { get; protected set; }

    protected uint RawValue { get; set; }

    public void Parse(StreamDecoder streamDecoder)
    {
      this.Namespace = streamDecoder.ReadUint32();
      this.Name = streamDecoder.ReadUint32();
      this.RawValue = streamDecoder.ReadUint32();
      this.Data.Parse(streamDecoder);
    }
  }
}
