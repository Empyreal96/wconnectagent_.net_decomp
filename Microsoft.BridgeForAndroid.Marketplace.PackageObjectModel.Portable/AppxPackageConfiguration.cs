// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.AppxPackageConfiguration
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System.Runtime.Serialization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
  [DataContract]
  public sealed class AppxPackageConfiguration
  {
    public AppxPackageConfiguration(
      AppxPackageType packageType,
      AppxPackageArchitecture packageArch)
    {
      this.PackageType = packageType;
      this.PackageArch = packageArch;
    }

    [DataMember]
    public AppxPackageType PackageType { get; private set; }

    [DataMember]
    public AppxPackageArchitecture PackageArch { get; private set; }
  }
}
