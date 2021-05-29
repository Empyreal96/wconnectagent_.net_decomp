// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ManifestString
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public sealed class ManifestString
  {
    public const string Sentinel = "@res:";

    public ManifestString(string xmlAttributeName, string xmlAttributeValue)
    {
      if (string.IsNullOrWhiteSpace(xmlAttributeName))
        throw new ArgumentException("Value must not be null or whitespace.", nameof (xmlAttributeName));
      if (string.IsNullOrWhiteSpace(xmlAttributeValue))
        throw new ArgumentException("Value must not be null or whitespace.", nameof (xmlAttributeValue));
      this.Content = !xmlAttributeValue.StartsWith("@res:", StringComparison.Ordinal) ? xmlAttributeValue : throw new DecoderValueMustBeStringException(xmlAttributeName, xmlAttributeValue);
    }

    public string Content { get; private set; }
  }
}
