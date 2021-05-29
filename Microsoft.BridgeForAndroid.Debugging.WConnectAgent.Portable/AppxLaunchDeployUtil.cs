// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AppxLaunchDeployUtil
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public static class AppxLaunchDeployUtil
  {
    public static async Task<string> InstallAppx(IFactory factory, string appxFilePath)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (appxFilePath == null)
        throw new ArgumentNullException(nameof (appxFilePath));
      LoggerCore.Log("Installing APPX at " + appxFilePath);
      IPortableRepositoryHandler repository = factory.CreateRepository();
      await repository.InitializeAsync((IPackageDetails) new ApkDetails(Path.GetFileName(appxFilePath)));
      AppxPackageConfiguration packageConfig = new AppxPackageConfiguration(AppxPackageType.Phone, AppxPackageArchitecture.Arm);
      string projectRoot = repository.GetAppxProjectRootFolder(packageConfig);
      LoggerCore.Log("Extracting APPX to " + projectRoot);
      PortableZipUtils.ExtractAllFromZip(appxFilePath, projectRoot);
      AppxDeployService deployer = new AppxDeployService(factory);
      AppxPackage package = AppxLaunchDeployUtil.GetPackage(Path.Combine(new string[2]
      {
        projectRoot,
        "AppxManifest.xml"
      }));
      if (package != null)
      {
        LoggerCore.Log("Attempting to uninstall " + package.PackageName);
        AppUninstallResult result = await factory.CreatePackageManager().UninstallAppsAsync(package);
        LoggerCore.Log("Unistall result:" + (object) result);
      }
      LoggerCore.Log("Deploying APPX");
      PackageDeploymentResult installResult = await deployer.DeployAppxProjectAsync(repository);
      string installResultText = AdbMessageStrings.FromPackageManagerInstallResult(installResult);
      if (installResult.Error == null)
      {
        LoggerCore.Log("Success: " + (object) installResult);
        return installResultText;
      }
      LoggerCore.Log("Error: " + (object) installResult);
      return installResultText;
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "User input", MessageId = "1#")]
    public static void LaunchUri(IFactory factory, string uri)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (uri == null)
        throw new ArgumentNullException(nameof (uri));
      factory.CreateUriLauncher().LaunchUri(new Uri(uri));
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Explicitly want to handle all exceptions")]
    private static AppxPackage GetPackage(string manifestPath)
    {
      LoggerCore.Log("Parsing manifest at " + manifestPath);
      try
      {
        string packageName = (string) null;
        string packagePublisher = (string) null;
        XNamespace xnamespace = (XNamespace) "http://schemas.microsoft.com/appx/2010/manifest";
        XDocument xdocument = XDocument.Load(manifestPath);
        LoggerCore.Log("Finding application node");
        XElement xelement = xdocument.Descendants(xnamespace + "Identity").FirstOrDefault<XElement>();
        if (xelement == null)
        {
          LoggerCore.Log("Identity node not present in the manifest");
          return (AppxPackage) null;
        }
        using (IEnumerator<XAttribute> enumerator = xelement.Attributes((XName) "Name").GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            packageName = enumerator.Current.Value;
            LoggerCore.Log("Found identity name: " + packageName);
          }
        }
        using (IEnumerator<XAttribute> enumerator = xelement.Attributes((XName) "Publisher").GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            packagePublisher = enumerator.Current.Value;
            LoggerCore.Log("Found publisher: " + packagePublisher);
          }
        }
        if (packageName != null)
        {
          if (packagePublisher != null)
            return new AppxPackage(packageName, string.Empty, packagePublisher, Path.GetDirectoryName(manifestPath));
        }
      }
      catch (Exception ex)
      {
        LoggerCore.Log(ex);
      }
      return (AppxPackage) null;
    }
  }
}
