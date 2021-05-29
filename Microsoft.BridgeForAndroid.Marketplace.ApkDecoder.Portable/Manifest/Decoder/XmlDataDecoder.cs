// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder.XmlDataDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder
{
  internal sealed class XmlDataDecoder
  {
    private readonly string defaultAndroidNamespaceUri = "http://schemas.android.com/apk/res/android";
    private string defaultNamespacePrefix;

    public XmlDataDecoder(IReadOnlyList<string> stringPool, IReadOnlyList<uint> resourceIds)
    {
      this.StringPool = stringPool;
      this.ResourceIds = resourceIds;
      this.XmlnsUriToPrefix = new Dictionary<uint, XmlNamespaceMapItem>();
      this.XmlnsShow = new Dictionary<uint, uint>();
      this.IndentCount = 0U;
    }

    public IReadOnlyList<string> StringPool { get; private set; }

    public IReadOnlyList<uint> ResourceIds { get; private set; }

    public Dictionary<uint, XmlNamespaceMapItem> XmlnsUriToPrefix { get; set; }

    public Dictionary<uint, uint> XmlnsShow { get; set; }

    public uint IndentCount { get; set; }

    public string IndentString => new string(' ', (int) this.IndentCount * 2);

    public string DefaultNamespacePrefix
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.defaultNamespacePrefix))
          this.defaultNamespacePrefix = this.StringPool[(int) this.XmlnsUriToPrefix.Single<KeyValuePair<uint, XmlNamespaceMapItem>>((Func<KeyValuePair<uint, XmlNamespaceMapItem>, bool>) (keyValue => this.StringPool[(int) keyValue.Key].Equals(this.defaultAndroidNamespaceUri))).Value.Prefix];
        return this.defaultNamespacePrefix;
      }
    }
  }
}
