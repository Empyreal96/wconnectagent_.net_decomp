// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AppxDeployService
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
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class AppxDeployService
  {
    public AppxDeployService(IFactory factory) => this.Factory = factory != null ? factory : throw new ArgumentNullException(nameof (factory));

    public IFactory Factory { get; private set; }

    public async Task<PackageDeploymentResult> DeployAppxProjectAsync(
      IPortableRepositoryHandler repository)
    {
      string finalAppxPath = repository != null ? repository.GetAppxProjectRootFolder((AppxPackageConfiguration) null) : throw new ArgumentNullException(nameof (repository));
      if (!PortableUtilsServiceLocator.FileUtils.DirectoryExists(finalAppxPath))
        throw new InvalidOperationException("The APPX directory does not exist.");
      IPackageManager packageManager = this.Factory.CreatePackageManager();
      Uri manifestUri = new Uri(Path.Combine(new string[2]
      {
        finalAppxPath,
        "AppxManifest.xml"
      }));
      LoggerCore.Log("Installing APPX from Layout...");
      DateTime beforeTime = DateTime.Now;
      PackageDeploymentResult installResult = await packageManager.InstallAppFromFolderLayoutAsync(manifestUri, (IEnumerable<Uri>) null);
      TimeSpan afterTime = DateTime.Now.Subtract(beforeTime);
      LoggerCore.Log("Installation took: " + (object) afterTime.TotalSeconds + " second(s).");
      if (installResult.Error == null)
      {
        LoggerCore.Log("Installation result: Success");
        AppxDeployService.CleanUnnecessaryDirectories(repository);
      }
      else
      {
        LoggerCore.Log("Installation error code: {0}", (object) installResult.Error);
        LoggerCore.Log("Installation extended error code: {0}", (object) installResult.ExtendedError);
        AppxDeployService.CleanAllDirectories(repository);
      }
      return installResult;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Log Statement.", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)")]
    private static void CleanUnnecessaryDirectories(IPortableRepositoryHandler repository)
    {
      try
      {
        AppxDeployService.DeleteApkFile(repository);
        AppxDeployService.DeleteApkExtractionPath(repository);
        AppxDeployService.DeleteMakePriConfigFile(repository);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsIOException(ex))
        {
          LoggerCore.Log(LoggerCore.LogLevels.Error, "Error removing unnecessary files from APPX package location following successful installation.");
          LoggerCore.Log(ex);
        }
        else
          throw;
      }
    }

    private static void DeleteApkExtractionPath(IPortableRepositoryHandler repository)
    {
      IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
      string str = repository.RetrievePackageExtractionPath();
      if (!fileUtils.DirectoryExists(str))
        return;
      PortableUtilsServiceLocator.FileUtils.DeleteDirectory(str);
    }

    private static void DeleteApkFile(IPortableRepositoryHandler repository)
    {
      IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
      string filePath = repository.RetrievePackageFilePath();
      if (!fileUtils.FileExists(filePath))
        return;
      fileUtils.DeleteFile(filePath);
    }

    private static void DeleteMakePriConfigFile(IPortableRepositoryHandler repository)
    {
      IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
      string filePath = repository.RetrieveMakePriConfigFilePath();
      if (!fileUtils.FileExists(filePath))
        return;
      fileUtils.DeleteFile(filePath);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Log Statement.", MessageId = "Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore.Log(Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore+LogLevels,System.String)")]
    private static void CleanAllDirectories(IPortableRepositoryHandler repository)
    {
      try
      {
        repository.Clean();
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsIOException(ex))
        {
          LoggerCore.Log(LoggerCore.LogLevels.Error, "Error deleting APPX package location following unsuccessful installation.");
          LoggerCore.Log(ex);
        }
        else
          throw;
      }
    }
  }
}
