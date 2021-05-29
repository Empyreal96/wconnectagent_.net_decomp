// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AndroidPackageResolverService
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class AndroidPackageResolverService
  {
    private IFactory factory;

    public AndroidPackageResolverService(IFactory factory) => this.factory = factory != null ? factory : throw new ArgumentNullException(nameof (factory));

    public AppxPackage ResolveAppxFromAndroidPackage(string androidPackageName)
    {
      if (string.IsNullOrWhiteSpace(androidPackageName))
        throw new ArgumentNullException(nameof (androidPackageName));
      try
      {
        string windowsPackage = this.factory.AowInstance.AndroidPackageToWindowsPackage(androidPackageName);
        if (windowsPackage == null)
        {
          LoggerCore.Log("Unable to resolve {0} to a Windows Appx Package.", (object) androidPackageName);
          return (AppxPackage) null;
        }
        LoggerCore.Log("Android Package Name: {0}.", (object) androidPackageName);
        LoggerCore.Log("Resolved APPX Full Package Name: {0}.", (object) windowsPackage);
        return this.FindPackageFromFullName(windowsPackage);
      }
      catch (Exception ex)
      {
        LoggerCore.Log(ex);
        throw new AndroidPackageResolveException(ex);
      }
    }

    private AppxPackage FindPackageFromFullName(string appxFullName)
    {
      IEnumerable<AppxPackage> source = this.factory.CreatePackageManager().FindPackages().Where<AppxPackage>((Func<AppxPackage, bool>) (m => string.Compare(m.PackageFullName, appxFullName, StringComparison.Ordinal) == 0));
      return source.Count<AppxPackage>() <= 1 ? source.FirstOrDefault<AppxPackage>() : throw new InvalidOperationException("Unexpected number of packages returned.");
    }
  }
}
