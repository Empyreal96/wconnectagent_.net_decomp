// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types.XmlElementChunk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Types
{
  internal abstract class XmlElementChunk : XmlItemChunk
  {
    protected XmlElementChunk()
    {
      this.Namespace = uint.MaxValue;
      this.Name = 0U;
    }

    protected internal uint Namespace { get; set; }

    protected internal uint Name { get; set; }
  }
}
