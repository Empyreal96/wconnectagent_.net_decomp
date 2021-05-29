// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AndroidPackageUninstallService
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class AndroidPackageUninstallService
  {
    private IFactory factory;

    public AndroidPackageUninstallService(IFactory factory) => this.factory = factory != null ? factory : throw new ArgumentNullException(nameof (factory));

    public async Task<AndroidPackageUninstallResult> UninstallAndroidPackageAsync(
      string androidPackageName)
    {
      if (string.IsNullOrWhiteSpace(androidPackageName))
        throw new ArgumentException("Must not be null or be whitespace.", "packageName");
      AndroidPackageResolverService resolverService = new AndroidPackageResolverService(this.factory);
      AppxPackage uninstallPackage = resolverService.ResolveAppxFromAndroidPackage(androidPackageName);
      AndroidPackageUninstallResult returnResult;
      if (uninstallPackage != null)
      {
        IPackageManager packageManager = this.factory.CreatePackageManager();
        LoggerCore.Log("Uninstalling Package. Package Name = {0} with Package Publisher = {1}.", (object) uninstallPackage.PackageName, (object) uninstallPackage.PackagePublisher);
        DateTime beforeTime = DateTime.Now;
        AppUninstallResult uninstallResult = await packageManager.UninstallAppsAsync(uninstallPackage);
        TimeSpan afterTime = DateTime.Now.Subtract(beforeTime);
        LoggerCore.Log("Uninstall Package Result: {0}. Took: {1} second(s).", (object) uninstallResult.ToString(), (object) afterTime.TotalSeconds);
        if (uninstallResult == AppUninstallResult.Success)
        {
          returnResult = AndroidPackageUninstallResult.Success;
          AndroidPackageUninstallService.RemovePackageDirectory(uninstallPackage.PackageLocation);
        }
        else
          returnResult = AndroidPackageUninstallResult.UninstallError;
      }
      else
        returnResult = AndroidPackageUninstallResult.NotFound;
      LoggerCore.Log("AndroidPackageUninstallService result: " + returnResult.ToString());
      return returnResult;
    }

    private static void RemovePackageDirectory(string packageLocation)
    {
      try
      {
        if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(packageLocation))
          return;
        PortableUtilsServiceLocator.FileUtils.DeleteDirectory(packageLocation);
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case IOException _:
          case UnauthorizedAccessException _:
            LoggerCore.Log(ex);
            break;
          default:
            throw;
        }
      }
    }
  }
}
