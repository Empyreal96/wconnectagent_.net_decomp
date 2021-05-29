// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ManifestStringResource
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public sealed class ManifestStringResource
  {
    public const string Sentinel = "@res:";

    public ManifestStringResource(string content)
    {
      this.Content = content != null ? content : throw new ArgumentNullException(nameof (content));
      this.IsResource = false;
      this.ResourceId = 0U;
      if (content.Length <= "@res:".Length || string.Compare(content.Substring(0, "@res:".Length), "@res:", StringComparison.CurrentCultureIgnoreCase) != 0)
        return;
      this.IsResource = true;
      this.ResourceId = Convert.ToUInt32(content.Substring("@res:".Length), 16);
    }

    public string Content { get; private set; }

    public bool IsResource { get; private set; }

    public uint ResourceId { get; private set; }
  }
}
