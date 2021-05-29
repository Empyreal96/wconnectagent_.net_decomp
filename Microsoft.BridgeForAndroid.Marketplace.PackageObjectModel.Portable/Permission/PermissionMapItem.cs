// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission.PermissionMapItem
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission
{
  public sealed class PermissionMapItem
  {
    public PermissionMapItem(
      PermissionType type,
      IReadOnlyCollection<AppxCapability> mappedCapabilities)
    {
      this.PermissionType = type;
      if (type == PermissionType.Present)
        this.MappedCapabilities = mappedCapabilities;
      LoggerCore.Log("Mapped Capability should not be proivided for Permission of type {0}", (object) type);
    }

    public PermissionType PermissionType { get; private set; }

    public IReadOnlyCollection<AppxCapability> MappedCapabilities { get; private set; }
  }
}
