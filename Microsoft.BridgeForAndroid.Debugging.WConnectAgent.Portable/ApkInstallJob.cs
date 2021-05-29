// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkInstallJob
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Converter;
using Microsoft.Arcadia.Marketplace.Converter.Portable;
using Microsoft.Arcadia.Marketplace.Decoder.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ApkInstallJob : ShellChannelJob
  {
    private const string TestApplicationIconFileName = "msftwbfa-apktestapp.png";
    private const string TestApplicationIconAssemblyResourceName = "Microsoft.Arcadia.Debugging.AdbAgent.Portable.apktestapp.png";
    private ShellPmInstallParam param;
    private IFactory factory;
    private IPortableFileUtils fileUtils = PortableUtilsServiceLocator.FileUtils;
    private AppxPackageType appxPackageType;
    private ApkObjectModel apkModel;
    private string correlationId;

    public ApkInstallJob(
      ShellPmInstallParam param,
      IFactory factory,
      AppxPackageType appxPackageType)
    {
      if (param == null)
        throw new ArgumentNullException(nameof (param));
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      this.param = param;
      this.factory = factory;
      this.appxPackageType = appxPackageType;
      this.IsWithinInteractiveShell = param.FromInteractiveShell;
    }

    protected override async Task<string> OnExecuteShellCommand() => await this.InstallAppAsync();

    private static string FormatAppInstallOutput(string apkFilePath, string message) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tpkg: {0}\r\n{1}\r\n", new object[2]
    {
      string.IsNullOrEmpty(apkFilePath) ? (object) "null" : (object) apkFilePath,
      (object) message
    });

    private AppxPackageConfiguration CreateAppxConfiguration(
      SystemArchitecture architecture)
    {
      if (architecture == SystemArchitecture.Arm)
        return new AppxPackageConfiguration(this.appxPackageType, AppxPackageArchitecture.Arm);
      if (architecture == SystemArchitecture.X86)
        return new AppxPackageConfiguration(this.appxPackageType, AppxPackageArchitecture.X86);
      if (architecture == SystemArchitecture.X64)
        return new AppxPackageConfiguration(this.appxPackageType, AppxPackageArchitecture.X64);
      throw new InvalidOperationException("The supplied CPU architecture is not supported in an APPX container");
    }

    private async Task<string> InstallAppAsync()
    {
      if (string.IsNullOrEmpty(this.param.ApkFilePath))
        return ApkInstallJob.FormatAppInstallOutput((string) null, "Error: no package specified");
      IPortableRepositoryHandler repository = (IPortableRepositoryHandler) null;
      this.correlationId = Guid.NewGuid().ToString();
      bool installedSuccessfully = false;
      EtwLogger.Instance.StartingApkInstall(this.correlationId, this.param.ApkFilePath);
      try
      {
        repository = this.factory.CreateRepository();
        await repository.InitializeAsync((IPackageDetails) new ApkDetails(Path.GetFileName(this.param.ApkFilePath)));
        EtwLogger.Instance.StartingApkSync(this.correlationId, this.param.ApkFilePath);
        ApkInstallJob.PullAPkResult result = await this.ObtainApkAsync(this.param.ApkFilePath, repository.RetrievePackageFilePath());
        switch (result)
        {
          case ApkInstallJob.PullAPkResult.Success:
            EtwLogger.Instance.ApkSyncSuccess(this.correlationId);
            ISystemInformation systemInfo = this.factory.CreateSystemInformation();
            if (systemInfo.Architecture == SystemArchitecture.Other)
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [INTERNAL_AGENT_ERROR]");
            AppxPackageConfiguration packageConfig = this.CreateAppxConfiguration(systemInfo.Architecture);
            try
            {
              EtwLogger.Instance.ApkConverting(this.correlationId);
              PackageInformation packageInformation = await this.ConvertApk(packageConfig, repository);
              EtwLogger.Instance.ApkConverted(this.correlationId);
            }
            catch (ApkDecoderManifestException ex)
            {
              EtwLogger.Instance.ApkConversionFailure(this.correlationId, this.BuildAdbFailMessageFromException((Exception) ex, "Failure [MANIFEST_DECODER_ERROR]"));
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [MANIFEST_DECODER_ERROR]");
            }
            catch (ApkDecoderResourcesException ex)
            {
              EtwLogger.Instance.ApkConversionFailure(this.correlationId, this.BuildAdbFailMessageFromException((Exception) ex, "Failure [RESOURCES_DECODER_ERROR]"));
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [RESOURCES_DECODER_ERROR]");
            }
            catch (ApkDecoderConfigException ex)
            {
              EtwLogger.Instance.ApkConversionFailure(this.correlationId, this.BuildAdbFailMessageFromException((Exception) ex, "Failure [CONFIGURE_DECODER_ERROR]"));
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [CONFIGURE_DECODER_ERROR]");
            }
            catch (DecoderManifestNoApplicationException ex)
            {
              EtwLogger.Instance.ApkConversionFailure(this.correlationId, this.BuildAdbFailMessageFromException((Exception) ex, "Failure [MANIFEST_DECODER_ERROR_APPLICATION_ELEMENT_NOT_FOUND]"));
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [MANIFEST_DECODER_ERROR_APPLICATION_ELEMENT_NOT_FOUND]");
            }
            catch (DecoderManifestNoVersionCodeException ex)
            {
              EtwLogger.Instance.ApkConversionFailure(this.correlationId, this.BuildAdbFailMessageFromException((Exception) ex, "Failure [MANIFEST_DECODER_ERROR_VERSION_NOT_FOUND]"));
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [MANIFEST_DECODER_ERROR_VERSION_NOT_FOUND]");
            }
            catch (ConverterException ex)
            {
              EtwLogger.Instance.ApkConversionFailure(this.correlationId, this.BuildAdbFailMessageFromException((Exception) ex, "Failure [CONVERTER_ERROR]"));
              return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [CONVERTER_ERROR]");
            }
            bool reinstall = false;
            if (this.param.Options != null)
            {
              foreach (string option in (IEnumerable<string>) this.param.Options)
              {
                if (string.Compare(option, "-r", StringComparison.Ordinal) == 0)
                {
                  reinstall = true;
                  break;
                }
              }
            }
            if (reinstall)
            {
              AndroidPackageUninstallResult result1 = new AndroidPackageUninstallService(this.factory).UninstallAndroidPackageAsync(this.apkModel.ManifestInfo.PackageNameResource.Content).Result;
              switch (result1)
              {
                case AndroidPackageUninstallResult.Success:
                case AndroidPackageUninstallResult.NotFound:
                  break;
                default:
                  EtwLogger.Instance.AppxUninstallFailure(this.correlationId, AdbMessageStrings.FromAndroidUninstallResult(result1));
                  return AdbMessageStrings.FromAndroidUninstallResult(result1);
              }
            }
            EtwLogger.Instance.StartAppxInstall(this.correlationId);
            AppxDeployService deployer = new AppxDeployService(this.factory);
            PackageDeploymentResult installResult = deployer.DeployAppxProjectAsync(repository).Result;
            string installResultString = AdbMessageStrings.FromPackageManagerInstallResult(installResult);
            string formatAppInstallOutput = ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, installResultString);
            if (installResult.Error == null)
            {
              EtwLogger.Instance.AppxInstalled(this.correlationId);
              installedSuccessfully = true;
              return formatAppInstallOutput;
            }
            EtwLogger.Instance.AppxInstallFailure(this.correlationId, formatAppInstallOutput);
            return formatAppInstallOutput;
          case ApkInstallJob.PullAPkResult.Error_FileNotFound:
            EtwLogger.Instance.ApkSyncFailure(this.correlationId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}", new object[2]
            {
              (object) result,
              (object) "Failure [INSTALL_FAILED_INVALID_URI]"
            }));
            return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [INSTALL_FAILED_INVALID_URI]");
          default:
            EtwLogger.Instance.ApkSyncFailure(this.correlationId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}", new object[2]
            {
              (object) result,
              (object) "Failure [INTERNAL_AGENT_ERROR]"
            }));
            return ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, "Failure [INTERNAL_AGENT_ERROR]");
        }
      }
      catch (Exception ex)
      {
        string errorMessage = this.BuildAdbFailMessageFromException(ex, "Failure [INTERNAL_AGENT_ERROR]");
        EtwLogger.Instance.LogError(errorMessage);
        return errorMessage;
      }
      finally
      {
        if (repository != null)
        {
          if (!installedSuccessfully)
          {
            try
            {
              repository.Clean();
            }
            catch (Exception ex)
            {
              LoggerCore.Log(ex);
            }
          }
        }
      }
    }

    private async Task<PackageInformation> ConvertApk(
      AppxPackageConfiguration packageConfig,
      IPortableRepositoryHandler handler)
    {
      MobilePortableApkDecoder decoder = new MobilePortableApkDecoder(handler, this.correlationId);
      await decoder.DecodeAsync();
      PackageInformation package = new PackageInformation()
      {
        PackageIdentityName = AppxUtilities.BuildAppxPackageIdentity(decoder.ObjModel),
        PackagePublisher = "CN=Microsoft",
        PackagePublisherDisplayName = AppxUtilities.BuildAppxPackagePublisherDisplayName()
      };
      this.apkModel = decoder.ObjModel;
      List<AppxPackageConfiguration> packagesConfigCollection = new List<AppxPackageConfiguration>();
      packagesConfigCollection.Add(packageConfig);
      PackageObjectDefaults packageObjectDefaults = new PackageObjectDefaults();
      packageObjectDefaults.ApplicationNameResource = new ManifestStringResource("Test Application");
      packageObjectDefaults.ApplicationIconFilePath = PortableUtilsServiceLocator.FileUtils.PathCombine(handler.RetrievePackageExtractionPath(), "msftwbfa-apktestapp.png");
      packageObjectDefaults.ApplicationIconResource = new ManifestStringResource("@res:19901024");
      packageObjectDefaults.ApplicationIconResourceId = 428871716U;
      await this.ExtractTestApplicationIcon(packageObjectDefaults);
      PortableApkToAppxConverter converter = new PortableApkToAppxConverter(decoder.ObjModel, handler, (IReadOnlyCollection<AppxPackageConfiguration>) packagesConfigCollection, (IPackageInformation) package, packageObjectDefaults);
      converter.GenerateOneAppxDirectory(packageConfig);
      return package;
    }

    private async Task<ApkInstallJob.PullAPkResult> ObtainApkAsync(
      string apkFilePathOnAndroid,
      string localFilePath)
    {
      IAdbChannel fileSyncChannel = await this.Configuration.RemoteChannelManager.OpenChannelAsync("sync:\0");
      if (fileSyncChannel == null)
        return ApkInstallJob.PullAPkResult.Errors_Others;
      try
      {
        AdbFileSyncStatPacketFromClient stat = new AdbFileSyncStatPacketFromClient(apkFilePathOnAndroid);
        bool succeeded = await stat.WriteAsync(fileSyncChannel.StreamWriter);
        if (!succeeded)
          return ApkInstallJob.PullAPkResult.Errors_Others;
        AdbFileSyncPacket packet = await AdbFileSyncPacket.ReadAsync(fileSyncChannel.StreamReader, AdbFileSyncPacket.Direction.FromServer);
        if (packet == null || !(packet is AdbFileSyncStatPacketFromServer statResponse2))
          return ApkInstallJob.PullAPkResult.Errors_Others;
        if (statResponse2.Mode == 0U)
          return ApkInstallJob.PullAPkResult.Error_FileNotFound;
        bool obtainedFile = false;
        ApkInstallJob.PullAPkResult pullResult = ApkInstallJob.PullAPkResult.Errors_Others;
        if (this.factory.AgentConfiguration.EnableInterception && this.TryCopyFromSnifferCache(statResponse2, apkFilePathOnAndroid, localFilePath))
        {
          obtainedFile = true;
          pullResult = ApkInstallJob.PullAPkResult.Success;
        }
        if (!obtainedFile)
          pullResult = await this.SyncFromAndroid(fileSyncChannel, apkFilePathOnAndroid, localFilePath);
        AdbFileSyncQuitPacket quit = new AdbFileSyncQuitPacket();
        int num = await quit.WriteAsync(fileSyncChannel.StreamWriter) ? 1 : 0;
        return pullResult;
      }
      finally
      {
        this.Configuration.RemoteChannelManager.CloseChannel(fileSyncChannel);
      }
    }

    private async Task<ApkInstallJob.PullAPkResult> SyncFromAndroid(
      IAdbChannel fileSyncChannel,
      string apkFilePathOnAndroid,
      string localFilePath)
    {
      AdbFileSyncReceivePacket recv = new AdbFileSyncReceivePacket(apkFilePathOnAndroid);
      bool succeeded = await recv.WriteAsync(fileSyncChannel.StreamWriter);
      if (!succeeded)
        return ApkInstallJob.PullAPkResult.Errors_Others;
      using (Stream fileStream = this.fileUtils.OpenOrCreateFileStream(localFilePath))
      {
        while (true)
        {
          AdbFileSyncPacket p = await AdbFileSyncPacket.ReadAsync(fileSyncChannel.StreamReader, AdbFileSyncPacket.Direction.FromServer);
          switch (p)
          {
            case null:
              goto label_9;
            case AdbFileSyncDataPacket dataPacket4:
              await fileStream.WriteAsync(dataPacket4.Data, 0, dataPacket4.Data.Length);
              continue;
            case AdbFileSyncFailPacket _:
              goto label_8;
            case AdbFileSyncDonePacket _:
              goto label_13;
            default:
              continue;
          }
        }
label_8:
        return ApkInstallJob.PullAPkResult.Errors_Others;
label_9:
        return ApkInstallJob.PullAPkResult.Errors_Others;
      }
label_13:
      return ApkInstallJob.PullAPkResult.Success;
    }

    private bool TryCopyFromSnifferCache(
      AdbFileSyncStatPacketFromServer statResponse,
      string apkFilePathOnAndroid,
      string localFilePath)
    {
      string sniffedDirectory = this.factory.AgentConfiguration.LocalDataSniffedDirectory;
      string linuxDirectoryName = IOUtils.GetLinuxDirectoryName(apkFilePathOnAndroid);
      LoggerCore.Log("Local Intercept Path: {0}.", (object) sniffedDirectory);
      LoggerCore.Log("Remote Linux Directory Path: {0}.", (object) linuxDirectoryName);
      if (string.Compare(linuxDirectoryName, this.factory.AgentConfiguration.RemoteDataSnifferDirectory, StringComparison.Ordinal) != 0)
      {
        LoggerCore.Log("Path for pm install {0} is not on the intercept white list. Falling back to SYNC.", (object) linuxDirectoryName);
        return false;
      }
      PathSanitizer pathSanitizer = new PathSanitizer(Path.Combine(new string[2]
      {
        sniffedDirectory,
        Path.GetFileName(apkFilePathOnAndroid)
      }));
      if (!pathSanitizer.IsWithinFolder(sniffedDirectory))
      {
        LoggerCore.Log("The filePath {0} does not exist within the local cache directory.", (object) pathSanitizer.Path);
        return false;
      }
      try
      {
        if (PortableUtilsServiceLocator.FileUtils.FileExists(pathSanitizer.Path))
        {
          if (PortableUtilsServiceLocator.FileUtils.GetFileSize(pathSanitizer.Path) == (long) statResponse.Size)
          {
            PortableUtilsServiceLocator.FileUtils.CopyFile(pathSanitizer.Path, localFilePath, true);
            LoggerCore.Log("Successfully copy intercepted file {0} to {1}.", (object) pathSanitizer.Path, (object) localFilePath);
            return true;
          }
          LoggerCore.Log("Mismatch between file in Android Environment and copy in cache directory.");
        }
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsIOException(ex))
          LoggerCore.Log(ex);
        else
          throw;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Reviewed.", MessageId = "Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkInstallJob.FormatAppInstallOutput(System.String,System.String)")]
    private string BuildAdbFailMessageFromException(Exception ex, string reasonForFailure) => ApkInstallJob.FormatAppInstallOutput(this.param.ApkFilePath, !NativeMethods.IsDiskspaceFullException(ex) ? reasonForFailure : "Failure [OUT_OF_DISK_SPACE]");

    private async Task ExtractTestApplicationIcon(PackageObjectDefaults defaults)
    {
      Assembly currentAssembly = typeof (ApkInstallJob).GetTypeInfo().Assembly;
      using (Stream resourceStream = currentAssembly.GetManifestResourceStream("Microsoft.Arcadia.Debugging.AdbAgent.Portable.apktestapp.png"))
      {
        if (resourceStream == null)
          throw new InvalidOperationException("Could not find test application icon in resources.");
        using (Stream fileStream = PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(defaults.ApplicationIconFilePath))
          await resourceStream.CopyToAsync(fileStream);
      }
    }

    private enum PullAPkResult
    {
      Success,
      Error_FileNotFound,
      Errors_Others,
    }
  }
}
