// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.Portable.PackageObjectDefaults
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
  public class PackageObjectDefaults
  {
    public ManifestStringResource ApplicationNameResource { get; set; }

    public ManifestStringResource ApplicationIconResource { get; set; }

    public string ApplicationIconFilePath { get; set; }

    public uint ApplicationIconResourceId { get; set; }
  }
}
