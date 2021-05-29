// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder.XmlNamespaceMapItem
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
  internal sealed class XmlNamespaceMapItem
  {
    public XmlNamespaceMapItem(uint prefix)
    {
      this.Prefix = prefix;
      this.Count = 1U;
    }

    public uint Prefix { get; private set; }

    public uint Count { get; set; }
  }
}
