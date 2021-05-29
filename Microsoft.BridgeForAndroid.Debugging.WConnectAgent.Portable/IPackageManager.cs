// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.IPackageManager
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  [ComVisible(false)]
  public interface IPackageManager
  {
    IList<AppxPackage> FindPackages();

    Task<PackageDeploymentResult> InstallAppFromFolderLayoutAsync(
      Uri manifestUri,
      IEnumerable<Uri> dependencyPackageUris);

    Task<AppUninstallResult> UninstallAppsAsync(AppxPackage package);
  }
}
