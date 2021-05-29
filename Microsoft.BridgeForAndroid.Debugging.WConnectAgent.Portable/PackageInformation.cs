// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.PackageInformation
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class PackageInformation : IPackageInformation
  {
    public string PackageIdentityName { get; set; }

    public string PackagePublisher { get; set; }

    public string PackagePublisherDisplayName { get; set; }
  }
}
