// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission.AppxCapability
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission
{
  public sealed class AppxCapability
  {
    public AppxCapability(string capabilityName, CapabilityType type)
    {
      this.CapabilityName = !string.IsNullOrEmpty(capabilityName) ? capabilityName : throw new ArgumentException("Capability name must be provided", nameof (capabilityName));
      this.CapabilityType = type;
      LoggerCore.Log("APPX Capability - Name: {0}, Type: {1}");
    }

    public string CapabilityName { get; private set; }

    public CapabilityType CapabilityType { get; private set; }
  }
}
