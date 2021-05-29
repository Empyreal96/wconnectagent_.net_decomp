// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AppxPackage
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class AppxPackage
  {
    public AppxPackage(
      string packageName,
      string packageFullName,
      string packagePublisher,
      string packageLocation)
    {
      if (packageName == null)
        throw new ArgumentNullException(nameof (packageName));
      if (packageFullName == null)
        throw new ArgumentNullException(nameof (packageFullName));
      if (packagePublisher == null)
        throw new ArgumentNullException(nameof (packagePublisher));
      this.PackageName = packageName;
      this.PackageFullName = packageFullName;
      this.PackagePublisher = packagePublisher;
      this.PackageLocation = packageLocation;
    }

    public string PackageName { get; private set; }

    public string PackageFullName { get; private set; }

    public string PackagePublisher { get; private set; }

    public string PackageLocation { get; private set; }
  }
}
