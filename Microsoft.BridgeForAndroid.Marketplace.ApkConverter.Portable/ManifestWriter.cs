// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.ManifestWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Parsers;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  public sealed class ManifestWriter
  {
    private const string AppxUapPrefix = "uap";
    private const string AppxMobilePrefix = "mobile";
    private const string RestrictedCapabilitiesPrefix = "rescap";
    private const int ProtocolPrefixMaxLength = 39;
    private static HashSet<string> uapCapabilities = new HashSet<string>()
    {
      "documentsLibrary",
      "picturesLibrary",
      "videosLibrary",
      "musicLibrary",
      "enterpriseAuthentication",
      "sharedUserCertificates",
      "removableStorage",
      "appointments",
      "contacts",
      "phoneCall"
    };
    private AppxPackageType appxType;
    private string templateFilePath;
    private string outputFilePath;
    private HashSet<string> languages;
    private HashSet<string> shareTargetDataFormats;
    private Dictionary<string, string> protocolExtensions;
    private HashSet<string> softwareCapabilities;
    private HashSet<string> deviceCapabilities;
    private HashSet<string> restrictedCapabilities;
    private HashSet<AppxScreenOrientationType> initialScreenOrientations;
    private List<FileTypeAssociation> fileTypesExtensions;
    private Dictionary<string, ManifestWriter.BackgroundTaskInfo> backgroundTaskExtensions;
    private Dictionary<string, List<ManifestWriter.InProcessServerClassInfo>> inProcessServerExtensions;

    public ManifestWriter(AppxPackageType type, string templateFilePath, string outputFilePath)
    {
      if (string.IsNullOrWhiteSpace(templateFilePath))
        throw new ArgumentException("templateFilePath is null or empty");
      if (string.IsNullOrWhiteSpace(outputFilePath))
        throw new ArgumentException("manifestFilePath is null or empty");
      this.appxType = type;
      this.templateFilePath = templateFilePath;
      this.outputFilePath = outputFilePath;
      this.PackageIdentityName = string.IsNullOrWhiteSpace(this.PackageIdentityName) ? "ClientPackageName" : this.PackageIdentityName;
      this.PackagePublisher = string.IsNullOrWhiteSpace(this.PackagePublisher) ? "CN=developer" : this.PackagePublisher;
      this.PackagePublisherDisplayName = string.IsNullOrWhiteSpace(this.PackagePublisherDisplayName) ? "DeveloperDisplayName" : this.PackagePublisherDisplayName;
      this.languages = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.shareTargetDataFormats = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.protocolExtensions = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.softwareCapabilities = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.deviceCapabilities = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.restrictedCapabilities = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.initialScreenOrientations = new HashSet<AppxScreenOrientationType>();
      this.fileTypesExtensions = new List<FileTypeAssociation>();
      this.backgroundTaskExtensions = new Dictionary<string, ManifestWriter.BackgroundTaskInfo>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.inProcessServerExtensions = new Dictionary<string, List<ManifestWriter.InProcessServerClassInfo>>((IEqualityComparer<string>) StringComparer.Ordinal);
    }

    public string PackageIdentityName { get; set; }

    public string PackagePhoneIdentity { get; set; }

    public string ApplicationId { get; set; }

    public string PackagePublisher { get; set; }

    public string PackagePublisherDisplayName { get; set; }

    public string ApkName { get; set; }

    public string ApkVersion { get; set; }

    public string AppxVersion { get; set; }

    public string ProcessorArchitecture { get; set; }

    public string AppName { get; set; }

    public string StoreLogo { get; set; }

    public string AppLogo { get; set; }

    public string TileLogoSmall { get; set; }

    public string TileLogoMedium { get; set; }

    public string TileLogoWide { get; set; }

    public string TileLogoLarge { get; set; }

    public string SplashScreen { get; set; }

    public string BackgroundColor { get; set; }

    public void AddLanguage(string languageQualifier)
    {
      if (!LanguageQualifier.IsValidLanguageQualifier(languageQualifier))
        throw new ConverterException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid Language Qualifier {0} or not supported in Windows.", new object[1]
        {
          (object) languageQualifier
        }));
      this.languages.Add(languageQualifier);
    }

    public void AddShareTargetDataFormat(string dataFormat)
    {
      if (string.IsNullOrWhiteSpace(dataFormat))
        throw new ArgumentException("Data format shouldn't be null or empty", nameof (dataFormat));
      this.shareTargetDataFormats.Add(dataFormat);
    }

    public void AddProtocolExtension(string protocolName, string returnResults)
    {
      if (string.IsNullOrWhiteSpace(protocolName))
        throw new ArgumentException("Protocol name shouldn't be null or empty", nameof (protocolName));
      if (protocolName.Length > 39)
        throw new ArgumentNullException("Protocol name must be less than or equal to " + (object) 39 + " character(s).");
      this.protocolExtensions.Add(protocolName, returnResults);
    }

    public void AddFileTypeAssociation(FileTypeAssociation fileTypeAssocition) => this.fileTypesExtensions.Add(fileTypeAssocition);

    public void AddSoftwareCapability(string softwareCapability)
    {
      if (string.IsNullOrWhiteSpace(softwareCapability))
        throw new ArgumentException("softwareCapability must be provided.", nameof (softwareCapability));
      LoggerCore.Log("Adding software capability {0}", (object) softwareCapability);
      this.softwareCapabilities.Add(softwareCapability);
    }

    public void AddDeviceCapability(string deviceCapability)
    {
      if (string.IsNullOrWhiteSpace(deviceCapability))
        throw new ArgumentException("deviceCapability must be provided.", nameof (deviceCapability));
      LoggerCore.Log("Adding device capability {0}", (object) deviceCapability);
      this.deviceCapabilities.Add(deviceCapability);
    }

    public void AddRestrictedCapability(string restrictedCapability)
    {
      if (string.IsNullOrWhiteSpace(restrictedCapability))
        throw new ArgumentException("restrictedCapability must be provided.", nameof (restrictedCapability));
      LoggerCore.Log("Adding restricted capability {0}", (object) restrictedCapability);
      this.restrictedCapabilities.Add(restrictedCapability);
    }

    public void AddInitialScreenOrientations(AppxScreenOrientationType initialScreenOrientation)
    {
      LoggerCore.Log("Adding initial screen orientation {0}", (object) initialScreenOrientation);
      this.initialScreenOrientations.Add(initialScreenOrientation);
    }

    public void AddBackgroundTaskExtension(string entryPoint, string executable, string taskType)
    {
      if (string.IsNullOrWhiteSpace(entryPoint))
        throw new ArgumentException("entryPoint must be provided", nameof (entryPoint));
      if (string.IsNullOrWhiteSpace(taskType))
        throw new ArgumentException("taskType must be provided", nameof (taskType));
      ManifestWriter.BackgroundTaskInfo backgroundTaskInfo;
      if (!this.backgroundTaskExtensions.TryGetValue(entryPoint, out backgroundTaskInfo))
      {
        backgroundTaskInfo = new ManifestWriter.BackgroundTaskInfo(executable);
        this.backgroundTaskExtensions.Add(entryPoint, backgroundTaskInfo);
      }
      backgroundTaskInfo.TaskTypes.Add(taskType);
    }

    public void AddInProcessServerExtension(
      string path,
      string activatableClassId,
      string threadingMode)
    {
      List<ManifestWriter.InProcessServerClassInfo> processServerClassInfoList;
      if (!this.inProcessServerExtensions.TryGetValue(path, out processServerClassInfoList))
      {
        processServerClassInfoList = new List<ManifestWriter.InProcessServerClassInfo>();
        this.inProcessServerExtensions.Add(path, processServerClassInfoList);
      }
      processServerClassInfoList.Add(new ManifestWriter.InProcessServerClassInfo(activatableClassId, threadingMode));
    }

    public void WriteToFile()
    {
      LoggerCore.Log("Writing APPX manifest file to " + this.templateFilePath);
      LoggerCore.Log("APPX type is " + this.appxType.ToString());
      XmlDocWriter writer = new XmlDocWriter(this.templateFilePath, InputType.FilePath);
      this.AddNamespaces(writer);
      AppxManifestDefs manifestDefs = new AppxManifestDefs(XmlConstants.XmlManifestDefaultPrefix, "mobile", "uap");
      this.WriteFields(writer, manifestDefs);
      this.WriteLanguages(writer, manifestDefs);
      ManifestWriter.VerifyExistingExtensions(writer, manifestDefs);
      this.InjectProjectAMobileExtensionSubelements(writer, manifestDefs);
      this.WriteShareTargetExtension(writer, manifestDefs);
      this.WriteProtocolExtensions(writer, manifestDefs);
      this.WriteFileTypeExtension(writer, manifestDefs);
      this.WriteBackgroundTaskExtensions(writer, manifestDefs);
      this.WriteCapabilities(writer, manifestDefs);
      this.WriteInProcessServerExtensions(writer, manifestDefs);
      writer.WriteToFile(this.outputFilePath);
      LoggerCore.Log("Finished writing APPX manifest file");
    }

    private static void VerifyExistingExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
        return;
      if (writer.QueryQualifyingChildElements(manifestDefs.ExtensionsElementPath, (IReadOnlyCollection<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("Category", "windows.fileTypeAssociation"),
        new KeyValuePair<string, string>("Category", "windows.shareTarget")
      }))
        throw new ConverterException("Extension elements for File Type Association or Share Target already exists.");
    }

    private static void WriteOneField(XmlDocWriter writer, ManifestWriter.FieldWriteInfo info)
    {
      if (string.IsNullOrWhiteSpace(info.Value))
        throw new ArgumentException("The value to write is null or empty", nameof (info));
      LoggerCore.Log("Writing one field:");
      if (string.IsNullOrWhiteSpace(info.AttributeName))
      {
        LoggerCore.Log(info.ElementPath + "/[.] = " + info.Value);
        writer.SetElementInnerText(info.ElementPath, info.Value);
      }
      else
      {
        LoggerCore.Log(info.ElementPath + "/@" + info.AttributeName + " = " + info.Value);
        writer.SetElementAttribute(info.ElementPath, info.AttributeName, info.Value);
      }
    }

    private static bool IsUapCapability(string capability) => ManifestWriter.uapCapabilities.Contains(capability);

    private void AddNamespaces(XmlDocWriter writer)
    {
      writer.AddDefaultNamespace(XmlConstants.XmlManifestDefaultPrefix, "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
      writer.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");
      writer.AddNamespace("mobile", "http://schemas.microsoft.com/appx/manifest/mobile/windows10");
      if (this.restrictedCapabilities.Count <= 0)
        return;
      writer.AddNamespace("rescap", "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities");
    }

    private void InjectProjectAMobileExtensionSubelements(
      XmlDocWriter writer,
      AppxManifestDefs manifestDefs)
    {
      LoggerCore.Log("Writing mobile:projectA payload name and version fields");
      string elementPath1 = manifestDefs.MobileProjectAExtensionElementPath + "/" + XmlUtilites.MakeElementPath("mobile", manifestDefs.PayloadNameElementName);
      writer.SetElementInnerText(elementPath1, this.ApkName);
      LoggerCore.Log("Payload Name: {0} - {1}", (object) elementPath1, (object) this.ApkName);
      string elementPath2 = manifestDefs.MobileProjectAExtensionElementPath + "/" + XmlUtilites.MakeElementPath("mobile", manifestDefs.PayloadVersionElementName);
      writer.SetElementInnerText(elementPath2, this.ApkVersion);
      LoggerCore.Log("Payload Version: {0} - {1}", (object) elementPath2, (object) this.ApkVersion);
    }

    private void WriteFileTypeExtension(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (this.fileTypesExtensions == null || this.fileTypesExtensions.Count == 0)
      {
        LoggerCore.Log("No file type association extension to be injected into the xml.");
      }
      else
      {
        if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
          writer.AddChildElement(manifestDefs.ApplicationPath, (string) null, manifestDefs.ExtensionsElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
        writer.AddChildElement(manifestDefs.ExtensionsElementPath, "uap", manifestDefs.ExtensionElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.CategoryAttributeName] = manifestDefs.FileTypeTypeAssociateCategory
        }, (string) null);
        foreach (FileTypeAssociation fileTypesExtension in this.fileTypesExtensions)
        {
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          dictionary[manifestDefs.NameAttribute] = fileTypesExtension.Name;
          string parentPath1 = manifestDefs.UapExtensionElementPath + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[@{0}='{1}']", new object[2]
          {
            (object) manifestDefs.CategoryAttributeName,
            (object) manifestDefs.FileTypeTypeAssociateCategory
          });
          writer.AddChildElement(parentPath1, "uap", manifestDefs.FileTypeAssociationElementName, (IReadOnlyDictionary<string, string>) dictionary, (string) null);
          string parentPath2 = manifestDefs.FileTypeAssociationElementPath + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[@{0}='{1}']", new object[2]
          {
            (object) manifestDefs.NameAttribute,
            (object) fileTypesExtension.Name
          });
          writer.AddChildElement(parentPath2, "uap", manifestDefs.SupportedFileTypesElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
          string parentPath3 = parentPath2 + "/" + XmlUtilites.MakeElementPath("uap", manifestDefs.SupportedFileTypesElementName);
          writer.AddChildElement(parentPath3, "uap", manifestDefs.FileTypeElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
          string elementPath = parentPath3 + "/" + XmlUtilites.MakeElementPath("uap", manifestDefs.FileTypeElementName);
          writer.SetElementInnerText(elementPath, fileTypesExtension.FileType);
        }
      }
    }

    private void WriteFields(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      this.WriteRequiredFields(writer, manifestDefs);
      this.WriteOptionalPackageRelatedFields(writer, manifestDefs);
      this.WriteOptionalFieldsInDefaultTile(writer, manifestDefs);
      this.WriteOptionalFieldsInSplashScreen(writer, manifestDefs);
      this.WriteOptionalInitialRotationPreference(writer, manifestDefs);
    }

    private void WriteOptionalPackageRelatedFields(
      XmlDocWriter writer,
      AppxManifestDefs manifestDefs)
    {
      ManifestWriter.FieldWriteInfo[] fieldWriteInfoArray = new ManifestWriter.FieldWriteInfo[4]
      {
        new ManifestWriter.FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.NameAttribute, this.PackageIdentityName),
        new ManifestWriter.FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.PackagePublisherAttribute, this.PackagePublisher),
        new ManifestWriter.FieldWriteInfo(manifestDefs.PropertiesPublisherDisplayNamePath, (string) null, this.PackagePublisherDisplayName),
        new ManifestWriter.FieldWriteInfo(manifestDefs.ApplicationPath, manifestDefs.ApplicationIdAttribute, this.ApplicationId)
      };
      foreach (ManifestWriter.FieldWriteInfo info in fieldWriteInfoArray)
      {
        if (!string.IsNullOrWhiteSpace(info.Value))
          ManifestWriter.WriteOneField(writer, info);
      }
    }

    private void WriteRequiredFields(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      ManifestWriter.FieldWriteInfo[] fieldWriteInfoArray = new ManifestWriter.FieldWriteInfo[8]
      {
        new ManifestWriter.FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.VersionAttribute, this.AppxVersion),
        new ManifestWriter.FieldWriteInfo(manifestDefs.IdentityPath, manifestDefs.ProcessorArchitectureAttribute, this.ProcessorArchitecture),
        new ManifestWriter.FieldWriteInfo(manifestDefs.PropertiesDisplayNamePath, (string) null, this.AppName),
        new ManifestWriter.FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.DisplayNameAttribute, this.AppName),
        new ManifestWriter.FieldWriteInfo(manifestDefs.PropertiesLogoPath, (string) null, this.StoreLogo),
        new ManifestWriter.FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.AppLogoAttribute, this.AppLogo),
        new ManifestWriter.FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.MediumTileLogoAttribute, this.TileLogoMedium),
        new ManifestWriter.FieldWriteInfo(manifestDefs.VisualElementsPath, manifestDefs.BackgroundColorAttributeName, this.BackgroundColor)
      };
      foreach (ManifestWriter.FieldWriteInfo info in fieldWriteInfoArray)
      {
        if (string.IsNullOrWhiteSpace(info.Value))
          throw new ConverterException("Required field hasn't been set");
        ManifestWriter.WriteOneField(writer, info);
      }
    }

    private void WriteOptionalFieldsInDefaultTile(
      XmlDocWriter writer,
      AppxManifestDefs manifestDefs)
    {
      List<ManifestWriter.FieldWriteInfo> fieldWriteInfoList = new List<ManifestWriter.FieldWriteInfo>();
      if (!string.IsNullOrWhiteSpace(this.TileLogoSmall))
      {
        ManifestWriter.FieldWriteInfo fieldWriteInfo = new ManifestWriter.FieldWriteInfo(manifestDefs.DefaultTilePath, manifestDefs.SmallTileLogoAttributeName, this.TileLogoSmall);
        fieldWriteInfoList.Add(fieldWriteInfo);
      }
      if (!string.IsNullOrWhiteSpace(this.TileLogoWide))
      {
        ManifestWriter.FieldWriteInfo fieldWriteInfo = new ManifestWriter.FieldWriteInfo(manifestDefs.DefaultTilePath, manifestDefs.WideTileLogoAttributeName, this.TileLogoWide);
        fieldWriteInfoList.Add(fieldWriteInfo);
      }
      if (!string.IsNullOrWhiteSpace(this.TileLogoLarge))
      {
        ManifestWriter.FieldWriteInfo fieldWriteInfo = new ManifestWriter.FieldWriteInfo(manifestDefs.DefaultTilePath, manifestDefs.LargeTileLogoAttributeName, this.TileLogoLarge);
        fieldWriteInfoList.Add(fieldWriteInfo);
      }
      if (fieldWriteInfoList.Count <= 0)
        return;
      if (!writer.HasElement(manifestDefs.DefaultTilePath))
        writer.AddChildElement(manifestDefs.VisualElementsPath, manifestDefs.DefaultTileElementPrefix, manifestDefs.DefaultTileElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      foreach (ManifestWriter.FieldWriteInfo info in fieldWriteInfoList)
        ManifestWriter.WriteOneField(writer, info);
    }

    private void WriteOptionalInitialRotationPreference(
      XmlDocWriter writer,
      AppxManifestDefs manifestDefs)
    {
      if (this.initialScreenOrientations.Count == 0)
        return;
      writer.AddChildElement(manifestDefs.VisualElementsPath, manifestDefs.InitialRotationPreferenceElementPrefix, manifestDefs.InitialRotationPreferenceElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      foreach (AppxScreenOrientationType screenOrientation in this.initialScreenOrientations)
        writer.AddChildElement(manifestDefs.InitialRotationPreferencePath, manifestDefs.RotationElementPrefix, manifestDefs.RotationElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.RotationPreferenceAttributeName] = ScreenOrientationMap.GetAppxScreenOrientationName(screenOrientation)
        }, (string) null);
    }

    private void WriteOptionalFieldsInSplashScreen(
      XmlDocWriter writer,
      AppxManifestDefs manifestDefs)
    {
      if (string.IsNullOrWhiteSpace(this.SplashScreen))
        return;
      if (!writer.HasElement(manifestDefs.SplashScreenPath))
        writer.AddChildElement(manifestDefs.VisualElementsPath, manifestDefs.SplashScreenElementPrefix, manifestDefs.SplashScreenElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      ManifestWriter.FieldWriteInfo info = new ManifestWriter.FieldWriteInfo(manifestDefs.SplashScreenPath, manifestDefs.SplashScreenAttributeName, this.SplashScreen);
      ManifestWriter.WriteOneField(writer, info);
    }

    private void WriteLanguages(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (this.languages.Count == 0)
        throw new ConverterException("No language is set");
      writer.RemoveAllChildElements(manifestDefs.ResourcesPath);
      foreach (string language in this.languages)
        writer.AddChildElement(manifestDefs.ResourcesPath, (string) null, manifestDefs.ResourceElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.LanguageAttribute] = language
        }, (string) null);
    }

    private void WriteShareTargetExtension(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (this.shareTargetDataFormats == null || this.shareTargetDataFormats.Count == 0)
        return;
      if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
        writer.AddChildElement(manifestDefs.ApplicationPath, (string) null, manifestDefs.ExtensionsElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      writer.AddChildElement(manifestDefs.ExtensionsElementPath, "uap", manifestDefs.ExtensionElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
      {
        [manifestDefs.CategoryAttributeName] = manifestDefs.ShareTargetCategory
      }, (string) null);
      writer.AddChildElement(manifestDefs.UapExtensionElementPath + "[@Category='windows.shareTarget']", "uap", manifestDefs.ShareTargetElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      foreach (string targetDataFormat in this.shareTargetDataFormats)
        writer.AddChildElement(manifestDefs.ShareTargetElementPath, "uap", manifestDefs.DataFormatElementName, (IReadOnlyDictionary<string, string>) null, targetDataFormat);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUpperCase", Justification = "Schema requires lowercase protocol names.")]
    private void WriteProtocolExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (this.protocolExtensions == null || this.protocolExtensions.Count == 0)
        return;
      if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
        writer.AddChildElement(manifestDefs.ApplicationPath, (string) null, manifestDefs.ExtensionsElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      foreach (KeyValuePair<string, string> protocolExtension in this.protocolExtensions)
      {
        writer.AddChildElement(manifestDefs.ExtensionsElementPath, "uap", manifestDefs.ExtensionElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.CategoryAttributeName] = manifestDefs.ProtocolCategory
        }, (string) null);
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary[manifestDefs.NameAttribute] = protocolExtension.Key.ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(protocolExtension.Value))
          dictionary[manifestDefs.ReturnResultsAttributeName] = protocolExtension.Value;
        writer.AddChildElement(manifestDefs.UapExtensionElementPath + "[@Category='windows.protocol']", "uap", manifestDefs.ProtocolElementName, (IReadOnlyDictionary<string, string>) dictionary, (string) null);
      }
    }

    private void WriteBackgroundTaskExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (this.backgroundTaskExtensions == null || this.backgroundTaskExtensions.Count <= 0)
        return;
      if (!writer.HasElement(manifestDefs.ExtensionsElementPath))
        writer.AddChildElement(manifestDefs.ApplicationPath, (string) null, manifestDefs.ExtensionsElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      foreach (KeyValuePair<string, ManifestWriter.BackgroundTaskInfo> backgroundTaskExtension in this.backgroundTaskExtensions)
      {
        string key = backgroundTaskExtension.Key;
        HashSet<string> taskTypes = backgroundTaskExtension.Value.TaskTypes;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary[manifestDefs.CategoryAttributeName] = manifestDefs.BackgroundTasksCategory;
        if (!string.IsNullOrEmpty(backgroundTaskExtension.Value.Executable))
          dictionary[manifestDefs.ExecutableAttributeName] = backgroundTaskExtension.Value.Executable;
        dictionary[manifestDefs.EntryPointAttributeName] = key;
        writer.AddChildElement(manifestDefs.ExtensionsElementPath, (string) null, manifestDefs.ExtensionElementName, (IReadOnlyDictionary<string, string>) dictionary, (string) null);
        string parentPath1 = manifestDefs.ExtensionElementPath + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[@{0}='{1}' and @{2}='{3}']", (object) manifestDefs.CategoryAttributeName, (object) manifestDefs.BackgroundTasksCategory, (object) manifestDefs.EntryPointAttributeName, (object) key);
        writer.AddChildElement(parentPath1, (string) null, manifestDefs.BackgroundTasksElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
        string parentPath2 = parentPath1 + "/" + XmlUtilites.MakeElementPath(XmlConstants.XmlManifestDefaultPrefix, manifestDefs.BackgroundTasksElementName);
        foreach (string str in taskTypes)
          writer.AddChildElement(parentPath2, (string) null, manifestDefs.TaskElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
          {
            [manifestDefs.TypeAttributeName] = str
          }, (string) null);
      }
    }

    private void WriteInProcessServerExtensions(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      if (this.inProcessServerExtensions == null || this.inProcessServerExtensions.Count <= 0)
        return;
      if (!writer.HasElement(manifestDefs.PackageExtensionsElementPath))
        writer.AddChildElement(manifestDefs.PackagePath, (string) null, manifestDefs.ExtensionsElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
      foreach (KeyValuePair<string, List<ManifestWriter.InProcessServerClassInfo>> processServerExtension in this.inProcessServerExtensions)
      {
        writer.AddChildElement(manifestDefs.PackageExtensionsElementPath, (string) null, manifestDefs.ExtensionElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.CategoryAttributeName] = manifestDefs.InProcessServerCategory
        }, (string) null);
        string parentPath1 = manifestDefs.PackageExtensionElementPath + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[@{0}='{1}']", new object[2]
        {
          (object) manifestDefs.CategoryAttributeName,
          (object) manifestDefs.InProcessServerCategory
        });
        writer.AddChildElement(parentPath1, (string) null, manifestDefs.InProcessServerElementName, (IReadOnlyDictionary<string, string>) null, (string) null);
        string parentPath2 = parentPath1 + "/" + XmlUtilites.MakeElementPath(XmlConstants.XmlManifestDefaultPrefix, manifestDefs.InProcessServerElementName);
        writer.AddChildElement(parentPath2, (string) null, manifestDefs.PathElementName, (IReadOnlyDictionary<string, string>) null, processServerExtension.Key);
        foreach (ManifestWriter.InProcessServerClassInfo processServerClassInfo in processServerExtension.Value)
          writer.AddChildElement(parentPath2, (string) null, manifestDefs.ActivatableClassElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
          {
            [manifestDefs.ActivatableClassIdAttributeName] = processServerClassInfo.ClassId,
            [manifestDefs.ThreadingModelAttributeName] = processServerClassInfo.ThreadingModel
          }, (string) null);
      }
    }

    private void WriteCapabilities(XmlDocWriter writer, AppxManifestDefs manifestDefs)
    {
      writer.RemoveAllChildElements(manifestDefs.CapabilitiesPath);
      foreach (string restrictedCapability in this.restrictedCapabilities)
        writer.AddChildElement(manifestDefs.CapabilitiesPath, "rescap", manifestDefs.CapabilityElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.CapabilityAttributeName] = restrictedCapability
        }, (string) null);
      foreach (string softwareCapability in this.softwareCapabilities)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary[manifestDefs.CapabilityAttributeName] = softwareCapability;
        string prefix = (string) null;
        if (ManifestWriter.IsUapCapability(softwareCapability))
          prefix = "uap";
        writer.AddChildElement(manifestDefs.CapabilitiesPath, prefix, manifestDefs.CapabilityElementName, (IReadOnlyDictionary<string, string>) dictionary, (string) null);
      }
      foreach (string deviceCapability in this.deviceCapabilities)
        writer.AddChildElement(manifestDefs.CapabilitiesPath, (string) null, manifestDefs.DeviceCapabilityElementName, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
        {
          [manifestDefs.CapabilityAttributeName] = deviceCapability
        }, (string) null);
    }

    private class InProcessServerClassInfo
    {
      public InProcessServerClassInfo(string classId, string threadingModel)
      {
        this.ClassId = classId;
        this.ThreadingModel = threadingModel;
      }

      public string ClassId { get; private set; }

      public string ThreadingModel { get; private set; }
    }

    private class BackgroundTaskInfo
    {
      public BackgroundTaskInfo(string executable)
      {
        this.Executable = executable;
        this.TaskTypes = new HashSet<string>();
      }

      public string Executable { get; private set; }

      public HashSet<string> TaskTypes { get; private set; }
    }

    private class FieldWriteInfo
    {
      public FieldWriteInfo(string elementPath, string attributeName, string value)
      {
        this.ElementPath = elementPath;
        this.AttributeName = attributeName;
        this.Value = value;
      }

      public string ElementPath { get; private set; }

      public string AttributeName { get; private set; }

      public string Value { get; private set; }
    }
  }
}
