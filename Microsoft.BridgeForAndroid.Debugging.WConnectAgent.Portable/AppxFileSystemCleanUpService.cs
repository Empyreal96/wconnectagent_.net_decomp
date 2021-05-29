// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AppxFileSystemCleanUpService
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class AppxFileSystemCleanUpService
  {
    private IPortableFileUtils fileUtils;
    private IPackageManager packageManager;
    private IAgentConfiguration agentConfig;
    private HashSet<string> allPackagesLocations;
    private IList<string> packageLocationsToDelete;

    public AppxFileSystemCleanUpService(IFactory factory)
    {
      this.packageManager = factory != null ? factory.CreatePackageManager() : throw new ArgumentNullException(nameof (factory));
      this.fileUtils = factory.CreatePortableFileUtils();
      this.agentConfig = factory.AgentConfiguration;
      this.packageLocationsToDelete = (IList<string>) new List<string>();
      this.allPackagesLocations = new HashSet<string>();
    }

    private bool ShouldCleanup => PortableUtilsServiceLocator.FileUtils.DirectoryExists(this.agentConfig.AppxLayoutRoot);

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "No need to handle errors on clean up.")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Log Statement.", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Log Statement.", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(System.String)")]
    public void RemoveStaleAppxLayout()
    {
      try
      {
        if (!this.ShouldCleanup)
        {
          LoggerCore.Log("Skipped packages cleanup as the APPX target directory doesn't exist.");
        }
        else
        {
          LoggerCore.Log("Starting deletion of stale packages...");
          this.BuildAllPackageLocations();
          this.BuildStaleAppxPackageList();
          this.DeleteStalePackages();
          LoggerCore.Log("Finished deletion of stale packages.");
        }
      }
      catch (Exception ex)
      {
        LoggerCore.Log(LoggerCore.LogLevels.Error, ex);
      }
    }

    private void BuildAllPackageLocations()
    {
      IList<AppxPackage> packages = this.packageManager.FindPackages();
      LoggerCore.Log("Package Manager returned {0} installed package(s).", (object) packages.Count);
      foreach (AppxPackage appxPackage in (IEnumerable<AppxPackage>) packages)
      {
        if (appxPackage.PackageLocation != null)
          this.allPackagesLocations.Add(Path.GetDirectoryName(appxPackage.PackageLocation.ToLower()));
      }
    }

    private void BuildStaleAppxPackageList()
    {
      foreach (string directory in this.fileUtils.GetDirectories(this.agentConfig.AppxLayoutRoot))
      {
        if (this.IsPackageLocationStale(directory))
          this.packageLocationsToDelete.Add(directory.ToLower());
      }
      LoggerCore.Log("Found {0} stale package(s).", (object) this.packageLocationsToDelete.Count);
    }

    private bool IsPackageLocationStale(string packageFolder) => !this.allPackagesLocations.Contains(packageFolder.ToLower());

    private void DeleteStalePackages()
    {
      foreach (string directoryPath in (IEnumerable<string>) this.packageLocationsToDelete)
        IOUtils.RemoveDirectory(directoryPath);
    }
  }
}
