// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.Portable.PortableApkToAppxConverter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
  public class PortableApkToAppxConverter
  {
    protected const string StoreManifestFileName = "StoreManifest.xml";
    protected const string ConfigFileName = "config.xml";
    protected const string AppInsightsConfigFileName = "ApplicationInsights.config";
    protected const string XboxServicesConfigFileName = "xboxservices.config";
    protected const string AppxManifestName = "AppxManifest.xml";
    protected const string DefaultLanguage = "en-US";
    private const string ApplicationIdPrefix = "aow";
    private const string AppNameResName = "AppName";
    private const string AppxResourcesFileName = "resources.pri";
    private IPackageInformation packInfo;

    public PortableApkToAppxConverter(
      ApkObjectModel apkObjectModel,
      IPortableRepositoryHandler repositoryHandler,
      IReadOnlyCollection<AppxPackageConfiguration> packageConfigs,
      IPackageInformation info)
      : this(apkObjectModel, repositoryHandler, packageConfigs, info, (PackageObjectDefaults) null)
    {
    }

    public PortableApkToAppxConverter(
      ApkObjectModel apkObjectModel,
      IPortableRepositoryHandler repositoryHandler,
      IReadOnlyCollection<AppxPackageConfiguration> packageConfigs,
      IPackageInformation info,
      PackageObjectDefaults packageObjectDefaults)
    {
      if (apkObjectModel == null)
        throw new ArgumentNullException(nameof (apkObjectModel));
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      if (packageConfigs == null || packageConfigs.Count == 0)
        throw new ArgumentException("Package configure list is NULL or empty", nameof (packageConfigs));
      this.ApkModel = apkObjectModel;
      this.Repository = repositoryHandler;
      this.PackageConfigs = packageConfigs;
      this.packInfo = info;
      this.PackageObjectDefaults = packageObjectDefaults;
    }

    public PackageObjectDefaults PackageObjectDefaults { get; private set; }

    protected ApkObjectModel ApkModel { get; private set; }

    protected IReadOnlyCollection<AppxPackageConfiguration> PackageConfigs { get; private set; }

    protected IPortableRepositoryHandler Repository { get; private set; }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need processor architecture in lower case")]
    public void GenerateOneAppxDirectory(AppxPackageConfiguration packageConfig)
    {
      if (!PortableUtilsServiceLocator.Initialized)
        throw new InvalidOperationException("Portable utilities not initialized.");
      if (packageConfig == null)
        throw new ArgumentNullException(nameof (packageConfig));
      this.PopulateImportantPackageInformationFromManifestMetadata();
      if (this.ApkModel.ManifestInfo.Application.Icon == null)
        this.ApkModel.InjectTestIconResource(this.PackageObjectDefaults.ApplicationIconResourceId, this.PackageObjectDefaults.ApplicationIconFilePath);
      this.CopyAppxBoilerplate(packageConfig);
      string projectRootFolder = this.Repository.GetAppxProjectRootFolder(packageConfig);
      string str = PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolder, "AppxManifest.xml");
      LoggerCore.Log("Manifest File path is {0}", (object) str);
      ManifestWriter manifestWriter = new ManifestWriter(packageConfig.PackageType, str, str);
      LoggerCore.Log("Project Root Folder path {0}", (object) projectRootFolder);
      StringsWriter stringsWriter = new StringsWriter(projectRootFolder);
      AssetsWriter assetsWriter = new AssetsWriter(projectRootFolder);
      this.WritePackageName(manifestWriter);
      this.WritePackagePublisher(manifestWriter);
      this.WritePackagePublisherDisplayName(manifestWriter);
      this.WriteApplicationId(manifestWriter);
      manifestWriter.ApkName = Path.GetFileName(this.Repository.RetrievePackageFilePath());
      this.WriteVersion(manifestWriter);
      manifestWriter.ProcessorArchitecture = packageConfig.PackageArch.ToString().ToLowerInvariant();
      this.WriteAppName(manifestWriter, stringsWriter);
      foreach (string languageQualifier in (IEnumerable<string>) stringsWriter.AllLanguageQualifiers)
      {
        LoggerCore.Log("Adding Language Qualifier {0}", (object) languageQualifier);
        manifestWriter.AddLanguage(languageQualifier);
      }
      manifestWriter.AddLanguage("en-US");
      this.WriteCapabilites(manifestWriter);
      this.WriteShareTargetExtension(manifestWriter);
      PortableApkToAppxConverter.WriteFileTypeAssociationExtension(manifestWriter);
      using (ImageAssetsConverter imageAssetsConverter = new ImageAssetsConverter(this.ApkModel, packageConfig.PackageType, manifestWriter, assetsWriter, this.Repository, this.PackageObjectDefaults))
      {
        imageAssetsConverter.WriteImageAssets();
        this.WriteBackgroundColor(manifestWriter, imageAssetsConverter.CalculatedBackgroundColor);
      }
      this.WriteInitialScreenOrientation(packageConfig.PackageType, manifestWriter);
      this.WritePackageProtocolExtension(manifestWriter);
      this.WriteBackgroundTaskExtension(manifestWriter);
      manifestWriter.WriteToFile();
      stringsWriter.WriteReswFiles();
      this.WriteConfigFile(PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolder, "config.xml"));
      this.ConvertMicrosoftServicesConfig(projectRootFolder);
      this.WriteWindowsStoreProxyFile(PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolder, "WindowsStoreProxy.xml"));
      this.GeneratePriFiles(projectRootFolder, manifestWriter);
      stringsWriter.CleanupAllReswFiles();
    }

    protected static void WriteStoreManifestFile(string storeManifestFilePath) => new StoreManifestWriter(storeManifestFilePath).WriteToFile();

    protected static void WriteFileTypeAssociationExtension(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
    }

    protected static void WriteShareTargetExtensionHelper(
      ManifestWriter manifestWriter,
      IReadOnlyList<ManifestIntentFilter> filters)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (filters == null)
        throw new ArgumentNullException(nameof (filters));
      foreach (ManifestIntentFilter filter in (IEnumerable<ManifestIntentFilter>) filters)
      {
        foreach (string action in (IEnumerable<string>) filter.Actions)
        {
          if (string.Compare(action, "android.intent.action.SEND", StringComparison.Ordinal) == 0 || string.Compare(action, "android.intent.action.SENDTO", StringComparison.Ordinal) == 0 || string.Compare(action, "android.intent.action.SEND_MULTIPLE", StringComparison.Ordinal) == 0)
          {
            foreach (ManifestIntentFilterData intentFilterData in (IEnumerable<ManifestIntentFilterData>) filter.Data)
            {
              if (!string.IsNullOrWhiteSpace(intentFilterData.MimeType))
              {
                LoggerCore.Log("Action: {0}, MimeType: {1}.", (object) action, (object) intentFilterData.MimeType);
                if (string.Compare(intentFilterData.MimeType, "*/*", StringComparison.OrdinalIgnoreCase) == 0)
                {
                  manifestWriter.AddShareTargetDataFormat("Html");
                  manifestWriter.AddShareTargetDataFormat("Text");
                  manifestWriter.AddShareTargetDataFormat("Uri");
                  manifestWriter.AddShareTargetDataFormat("Bitmap");
                }
                else if (string.Compare(intentFilterData.MimeType, "text/html", StringComparison.OrdinalIgnoreCase) == 0)
                  manifestWriter.AddShareTargetDataFormat("Html");
                else if (string.Compare(intentFilterData.MimeType, "text/plain", StringComparison.OrdinalIgnoreCase) == 0)
                {
                  manifestWriter.AddShareTargetDataFormat("Text");
                  manifestWriter.AddShareTargetDataFormat("Uri");
                }
                else if (intentFilterData.MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                {
                  manifestWriter.AddShareTargetDataFormat("Html");
                  manifestWriter.AddShareTargetDataFormat("Text");
                  manifestWriter.AddShareTargetDataFormat("Uri");
                }
                else if (intentFilterData.MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                  manifestWriter.AddShareTargetDataFormat("Bitmap");
                else
                  LoggerCore.Log("Mime Type {0} for the action {1} is not known.", (object) intentFilterData.MimeType, (object) action);
              }
              else
                LoggerCore.Log("We ignore action of {0} with empty mimetype.", (object) action);
            }
          }
        }
      }
    }

    protected void CopyAppxBoilerplate(AppxPackageConfiguration packageConfig)
    {
      string projectRootFolder = this.Repository.GetAppxProjectRootFolder(packageConfig);
      PortableUtilsServiceLocator.FileUtils.RecursivelyCopyDirectory(this.Repository.GetAppxEntryAppTemplatePath(packageConfig), projectRootFolder);
      string str = this.Repository.RetrievePackageFilePath();
      string fileName = Path.GetFileName(str);
      string destination = Path.Combine(new string[2]
      {
        projectRootFolder,
        fileName
      });
      PortableUtilsServiceLocator.FileUtils.CopyFile(str, destination, true);
    }

    protected void WriteApplicationId(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (string.IsNullOrWhiteSpace(this.packInfo.PackageIdentityName))
        return;
      Match match = !PortableApkToAppxConverter.DoesPackageNameHaveUnsupportedChars(this.packInfo.PackageIdentityName) ? Regex.Matches("aow" + this.packInfo.PackageIdentityName.Replace(".", ".aow"), "([A-Za-z][A-Za-z0-9]*)(\\.[A-Za-z][A-Za-z0-9]*)*")[0] : throw new ConverterException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SPACE or Underscore not allowed in Package Identity name. Provided identity name is {0}", new object[1]
      {
        (object) this.packInfo.PackageIdentityName
      }));
      manifestWriter.ApplicationId = match.ToString();
      LoggerCore.Log("Application Id: {0}", (object) manifestWriter.ApplicationId);
    }

    protected void WriteAppName(ManifestWriter manifestWriter, StringsWriter stringsWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (stringsWriter == null)
        throw new ArgumentNullException(nameof (stringsWriter));
      ManifestStringResource resource = this.ApkModel.ManifestInfo.Application.Label != null || this.PackageObjectDefaults == null ? this.ApkModel.ManifestInfo.Application.Label : this.PackageObjectDefaults.ApplicationNameResource;
      manifestWriter.AppName = this.ExtractResourceValue(resource, stringsWriter);
      LoggerCore.Log("Application Name: {0}", (object) manifestWriter.AppName);
    }

    protected void WriteBackgroundColor(
      ManifestWriter manifestWriter,
      Color? calculatedBackgroundColor)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (this.ApkModel.ManifestInfo.Application.BackgroundColorData != null)
      {
        ManifestStringResource manifestStringResource = this.ApkModel.ManifestInfo.Application.BackgroundColorData.Value;
        manifestWriter.BackgroundColor = manifestStringResource.Content;
      }
      else if (calculatedBackgroundColor.HasValue)
      {
        Color? nullable = calculatedBackgroundColor;
        Color transparent = Color.Transparent;
        int num = !nullable.HasValue ? 0 : (nullable.GetValueOrDefault() == transparent ? 1 : 0);
        manifestWriter.BackgroundColor = num == 0 ? calculatedBackgroundColor.ToString() : "transparent";
      }
      else
        manifestWriter.BackgroundColor = "#000000";
      LoggerCore.Log("Background Color: {0}", (object) manifestWriter.BackgroundColor);
    }

    protected void WriteCapabilites(ManifestWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      foreach (ManifestUsesPermission usesPermission in (IEnumerable<ManifestUsesPermission>) this.ApkModel.ManifestInfo.UsesPermissions)
      {
        PermissionMapItem permissionMapItem = PermissionMap.MapPermission(usesPermission.Name.Content);
        if (permissionMapItem.PermissionType == PermissionType.Present)
        {
          if (permissionMapItem.MappedCapabilities != null)
          {
            foreach (AppxCapability mappedCapability in (IEnumerable<AppxCapability>) permissionMapItem.MappedCapabilities)
            {
              if (mappedCapability.CapabilityType == CapabilityType.Software)
                writer.AddSoftwareCapability(mappedCapability.CapabilityName);
              else
                writer.AddDeviceCapability(mappedCapability.CapabilityName);
            }
          }
        }
        else
          LoggerCore.Log("{0} is not mapped because no mapping is present.", (object) usesPermission.Name.Content);
      }
    }

    protected void WriteConfigFile(string configFilePath) => new ConfigWriter(configFilePath)
    {
      AndroidPackageId = PortableApkToAppxConverter.GetRawString(this.ApkModel.ManifestInfo.PackageNameResource)
    }.WriteToFile();

    protected void ConvertMicrosoftServicesConfig(string projectRootFolderPath)
    {
      if (this.ApkModel.ApkConfigFile != null && this.ApkModel.ApkConfigFile.Analytics != null)
      {
        string outputFilePath = PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolderPath, "ApplicationInsights.config");
        string analyticsKey = this.ApkModel.ApkConfigFile.Analytics.AnalyticsKey;
        if (!string.IsNullOrEmpty(analyticsKey))
          new AppInsightsConfigWriter(outputFilePath)
          {
            InstrumentationKey = analyticsKey
          }.WriteToFile();
      }
      if (this.ApkModel.ApkConfigFile == null || this.ApkModel.ApkConfigFile.GameServices == null)
        return;
      new GameServicesConfigWriter(this.ApkModel.ApkConfigFile.GameServices).WriteToFile(PortableUtilsServiceLocator.FileUtils.PathCombine(projectRootFolderPath, "xboxservices.config"));
    }

    protected void WriteWindowsStoreProxyFile(string destinationWindowsProxyFile)
    {
      string fileFromZip = PortableZipUtils.ExtractFileFromZip(this.Repository.RetrievePackageFilePath(), "res/raw/WindowsStoreProxy.xml", this.Repository.RetrievePackageExtractionPath());
      if (fileFromZip == null)
        return;
      LoggerCore.Log("Found WindowsStoreProxy.xml file, copying over to appx side");
      PortableUtilsServiceLocator.FileUtils.CopyFile(fileFromZip, destinationWindowsProxyFile, true);
    }

    protected void WriteInitialScreenOrientation(
      AppxPackageType appxType,
      ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      IReadOnlyList<ManifestActivity> activities = this.ApkModel.ManifestInfo.Application.Activities;
      ICollection<ManifestActivity> supportingActivityList = (ICollection<ManifestActivity>) new List<ManifestActivity>();
      ApkScreenOrientationType mainActivityOrientation = ApkScreenOrientationType.Undeclared;
      foreach (ManifestActivity manifestActivity in (IEnumerable<ManifestActivity>) activities)
      {
        if (manifestActivity.HasMainActivity)
          mainActivityOrientation = manifestActivity.ScreenOrientation;
        else if (manifestActivity.ScreenOrientation != ApkScreenOrientationType.Undeclared)
          supportingActivityList.Add(manifestActivity);
      }
      foreach (AppxScreenOrientationType screenOrientation in PortableApkToAppxConverter.GetInitialScreenOrientations(mainActivityOrientation, supportingActivityList))
        manifestWriter.AddInitialScreenOrientations(screenOrientation);
    }

    protected void WritePackageName(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (!string.IsNullOrWhiteSpace(this.packInfo.PackageIdentityName))
        manifestWriter.PackageIdentityName = !PortableApkToAppxConverter.DoesPackageNameHaveUnsupportedChars(this.packInfo.PackageIdentityName) ? this.packInfo.PackageIdentityName : throw new ConverterException("SPACE or Underscore not allowed in Package Identity name");
      LoggerCore.Log("Package Name: {0}", (object) manifestWriter.PackageIdentityName);
    }

    protected void WritePackagePublisherDisplayName(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (!string.IsNullOrWhiteSpace(this.packInfo.PackagePublisherDisplayName))
        manifestWriter.PackagePublisherDisplayName = this.packInfo.PackagePublisherDisplayName;
      LoggerCore.Log("Publisher Display Name: {0}", (object) manifestWriter.PackagePublisherDisplayName);
    }

    protected void WritePackagePublisher(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      if (!string.IsNullOrWhiteSpace(this.packInfo.PackagePublisher))
        manifestWriter.PackagePublisher = PortableApkToAppxConverter.IsValidPublisherName(this.packInfo.PackagePublisher) ? this.packInfo.PackagePublisher : throw new ConverterException("Package Publisher Name is not well formed.");
      LoggerCore.Log("Package Publisher: {0}", (object) manifestWriter.PackagePublisher);
    }

    protected void WriteShareTargetExtension(ManifestWriter manifestWriter)
    {
      foreach (ManifestActivity activity in (IEnumerable<ManifestActivity>) this.ApkModel.ManifestInfo.Application.Activities)
        PortableApkToAppxConverter.WriteShareTargetExtensionHelper(manifestWriter, activity.Filters);
      foreach (ManifestActivityAlias activityAlias in (IEnumerable<ManifestActivityAlias>) this.ApkModel.ManifestInfo.Application.ActivityAliases)
        PortableApkToAppxConverter.WriteShareTargetExtensionHelper(manifestWriter, activityAlias.Filters);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Matching APPX manifest.")]
    protected void WritePackageProtocolExtension(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      string protocolName = "a+" + CryptoHelper.ComputeMD5HashAsHexadecimal(Encoding.UTF8.GetBytes(PortableApkToAppxConverter.GetRawString(this.ApkModel.ManifestInfo.PackageNameResource).ToLowerInvariant())).ToLowerInvariant();
      manifestWriter.AddProtocolExtension(protocolName, "optional");
    }

    protected void WriteBackgroundTaskExtension(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      LoggerCore.Log("Adding system event background task.");
      manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "systemEvent");
      LoggerCore.Log("Adding general event background task.");
      manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "general");
      if (this.ApkModel.ManifestInfo.HasPermission("com.google.android.c2dm.permission.RECEIVE") || this.ApkModel.ManifestInfo.HasPermission("com.microsoft.services.pushnotification.permission.RECEIVE"))
      {
        LoggerCore.Log("Adding push notification background task.");
        manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "pushNotification");
      }
      if (this.ApkModel.ManifestInfo.HasPermission("android.permission.ACCESS_FINE_LOCATION"))
      {
        LoggerCore.Log("Adding location background task.");
        manifestWriter.AddBackgroundTaskExtension("BackgroundTask.MainTask", "Arcadia.exe", "location");
      }
      LoggerCore.Log("Adding background task for {0}.", (object) "BackgroundTask.OutOfProcTask");
      manifestWriter.AddBackgroundTaskExtension("BackgroundTask.OutOfProcTask", (string) null, "general");
      LoggerCore.Log("Adding in process server for {0}.", (object) "BackgroundTask.MainTask");
      manifestWriter.AddInProcessServerExtension("AoWBackgroundTask.dll", "BackgroundTask.MainTask", "both");
      LoggerCore.Log("Adding in process server for {0}.", (object) "BackgroundTask.OutOfProcTask");
      manifestWriter.AddInProcessServerExtension("AoWBackgroundTask.dll", "BackgroundTask.OutOfProcTask", "both");
      LoggerCore.Log("Adding restricted capabilities for graphics improvements.");
      manifestWriter.AddRestrictedCapability("previewUiComposition");
      LoggerCore.Log("Adding restricted capabilities for graphics improvements.");
      manifestWriter.AddRestrictedCapability("previewUiComposition");
    }

    protected void WriteVersion(ManifestWriter manifestWriter)
    {
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      string rawString = PortableApkToAppxConverter.GetRawString(this.ApkModel.ManifestInfo.VersionCode);
      uint result = 0;
      manifestWriter.ApkVersion = uint.TryParse(rawString, out result) ? rawString : throw new ArgumentException("Invalid version code from APK: " + (object) result);
      uint num1 = result >> 16 & (uint) ushort.MaxValue;
      uint num2 = result & (uint) ushort.MaxValue;
      manifestWriter.AppxVersion = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}", (object) num1, (object) num2, (object) 0U, (object) 0U);
      LoggerCore.Log("APK Version: {0} --> APPX Version: {1}", (object) manifestWriter.ApkVersion, (object) manifestWriter.AppxVersion);
    }

    protected void GeneratePriFiles(string projectRootFolderPath, ManifestWriter manifestWriter)
    {
      if (string.IsNullOrEmpty(projectRootFolderPath))
        throw new ArgumentException("Must not be null or empty.", nameof (projectRootFolderPath));
      if (manifestWriter == null)
        throw new ArgumentNullException(nameof (manifestWriter));
      new PackageResourceIndexMaker(this.Repository.RetrieveMakePriToolPath(), manifestWriter.PackageIdentityName, "en-US").Run(this.Repository.RetrieveMakePriConfigFilePath(), projectRootFolderPath, Path.Combine(new string[2]
      {
        projectRootFolderPath,
        "resources.pri"
      }));
    }

    protected void PopulateImportantPackageInformationFromManifestMetadata()
    {
      if (this.ApkModel == null || this.ApkModel.ManifestInfo == null || (this.ApkModel.ManifestInfo.Application == null || this.ApkModel.ManifestInfo.Application.MetadataElements == null))
        return;
      foreach (ManifestApplicationMetadata metadataElement in (IEnumerable<ManifestApplicationMetadata>) this.ApkModel.ManifestInfo.Application.MetadataElements)
      {
        if (metadataElement.Value != null && !string.IsNullOrWhiteSpace(metadataElement.Value.Content) && !metadataElement.Value.IsResource)
        {
          if (metadataElement.Name.Equals("com.microsoft.windows.package.identity.name", StringComparison.OrdinalIgnoreCase))
          {
            this.packInfo.PackageIdentityName = metadataElement.Value.Content;
            LoggerCore.Log("Package Identity from APK Meta-data {0}", (object) this.packInfo.PackageIdentityName);
          }
          else if (metadataElement.Name.Equals("com.microsoft.windows.package.identity.publisher", StringComparison.OrdinalIgnoreCase))
          {
            this.packInfo.PackagePublisher = metadataElement.Value.Content;
            LoggerCore.Log("Package publisher from APK Meta-data {0}", (object) this.packInfo.PackagePublisher);
          }
          else if (metadataElement.Name.Equals("com.microsoft.windows.package.properties.publisherdisplayname", StringComparison.OrdinalIgnoreCase))
          {
            this.packInfo.PackagePublisherDisplayName = metadataElement.Value.Content;
            LoggerCore.Log("Package publisher display name from APK Meta-data {0}", (object) this.packInfo.PackagePublisherDisplayName);
          }
        }
        else
          LoggerCore.Log("Provided Meta-data {0}'s value is either NULL or has been localized. This is not a supported scenario.", (object) metadataElement.Name);
      }
    }

    private static bool DoesPackageNameHaveUnsupportedChars(string apkPackageName) => new Regex("[_ ]+").IsMatch(apkPackageName);

    private static HashSet<AppxScreenOrientationType> GetInitialScreenOrientations(
      ApkScreenOrientationType mainActivityOrientation,
      ICollection<ManifestActivity> supportingActivityList)
    {
      if (ScreenOrientationMap.GetContradictionRotatingActivites(mainActivityOrientation, supportingActivityList).Count > 0)
      {
        LoggerCore.Log("No APPX rotation will be enforced since there are contradicting screen orientation.");
        return new HashSet<AppxScreenOrientationType>();
      }
      ScreenOrientationItem screenOrientationItem = ScreenOrientationMap.MapActivityOrientation(mainActivityOrientation);
      if (screenOrientationItem != null)
        return screenOrientationItem.PossibleScreenOrientationTypes;
      LoggerCore.Log("No APPX rotation will be enforced since the main activity's orientation is not found.");
      return new HashSet<AppxScreenOrientationType>();
    }

    private static string GetRawString(ManifestStringResource manifestValue)
    {
      if (manifestValue.IsResource)
        throw new ConverterException("The field isn't expected be a reference to resource");
      return !string.IsNullOrEmpty(manifestValue.Content) ? manifestValue.Content : throw new ConverterException("The field must be an non-empty string");
    }

    private static bool IsValidPublisherName(string publisherName) => new Regex("(CN|L|O|OU|E|C|S|STREET|T|G|I|SN|DC|SERIALNUMBER|(OID\\.(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))+))=(([^,+=\"<>#;])+|\".*\")(, ((CN|L|O|OU|E|C|S|STREET|T|G|I|SN|DC|SERIALNUMBER|(OID\\.(0|[1-9][0-9]*)(\\.(0|[1-9][0-9]*))+))=(([^,+=\"<>#;])+|\".*\")))*").IsMatch(publisherName);

    private static string MakeResStringReference(string resName) => "ms-resource:" + resName;

    private string ExtractResourceValue(
      ManifestStringResource resource,
      StringsWriter stringsWriter)
    {
      if (!resource.IsResource)
        return resource.Content;
      ApkResource resource1 = ApkResourceHelper.GetResource(resource, this.ApkModel.Resources);
      string name = CultureInfo.CurrentCulture.Name;
      foreach (ApkResourceValue apkResourceValue in (IEnumerable<ApkResourceValue>) resource1.Values)
      {
        if (stringsWriter != null && !string.IsNullOrWhiteSpace(apkResourceValue.Value))
          stringsWriter.AddString("AppName", apkResourceValue.Value, apkResourceValue.Config.Locale);
      }
      return PortableApkToAppxConverter.MakeResStringReference("AppName");
    }
  }
}
