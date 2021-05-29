// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.AppxManifestDefs
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  internal class AppxManifestDefs
  {
    private const string IdentityElementName = "Identity";
    private const string PropertiesElementName = "Properties";
    private const string ApplicationsElementName = "Applications";
    private const string ApplicationElementName = "Application";
    private const string DisplayNameElementName = "DisplayName";
    private const string PublisherDisplayNameElementName = "PublisherDisplayName";
    private const string ResourcesElementName = "Resources";
    private const string CapabilitiesElementName = "Capabilities";
    public readonly string NameAttribute = "Name";
    public readonly string PackagePublisherAttribute = "Publisher";
    public readonly string VersionAttribute = "Version";
    public readonly string ProcessorArchitectureAttribute = "ProcessorArchitecture";
    public readonly string LanguageAttribute = "Language";
    public readonly string DisplayNameAttribute = "DisplayName";
    public readonly string MediumTileLogoAttribute = "Square150x150Logo";
    public readonly string WideTileLogoAttributeName = "Wide310x150Logo";
    public readonly string SplashScreenAttributeName = "Image";
    public readonly string CategoryAttributeName = "Category";
    public readonly string CapabilityAttributeName = "Name";
    public readonly string ApplicationIdAttribute = "Id";
    public readonly string RotationPreferenceAttributeName = "Preference";
    public readonly string BackgroundColorAttributeName = "BackgroundColor";
    public readonly string ReturnResultsAttributeName = "ReturnResults";
    public readonly string EntryPointAttributeName = "EntryPoint";
    public readonly string ExecutableAttributeName = "Executable";
    public readonly string TypeAttributeName = "Type";
    public readonly string ActivatableClassIdAttributeName = "ActivatableClassId";
    public readonly string ThreadingModelAttributeName = "ThreadingModel";
    public readonly string ResourceElementName = "Resource";
    public readonly string DefaultTileElementName = "DefaultTile";
    public readonly string SplashScreenElementName = "SplashScreen";
    public readonly string InitialRotationPreferenceElementName = "InitialRotationPreference";
    public readonly string RotationElementName = "Rotation";
    public readonly string ExtensionsElementName = "Extensions";
    public readonly string ExtensionElementName = "Extension";
    public readonly string ProjectAElementName = "AowApp";
    public readonly string PayloadNameElementName = "PayloadName";
    public readonly string PayloadVersionElementName = "PayloadVersion";
    public readonly string ShareTargetElementName = "ShareTarget";
    public readonly string ProtocolElementName = "Protocol";
    public readonly string DataFormatElementName = "DataFormat";
    public readonly string ShareTargetCategory = "windows.shareTarget";
    public readonly string ProtocolCategory = "windows.protocol";
    public readonly string FileTypeTypeAssociateCategory = "windows.fileTypeAssociation";
    public readonly string BackgroundTasksCategory = "windows.backgroundTasks";
    public readonly string InProcessServerCategory = "windows.activatableClass.inProcessServer";
    public readonly string FileTypeAssociationElementName = "FileTypeAssociation";
    public readonly string SupportedFileTypesElementName = "SupportedFileTypes";
    public readonly string FileTypeElementName = "FileType";
    public readonly string CapabilityElementName = "Capability";
    public readonly string DeviceCapabilityElementName = "DeviceCapability";
    public readonly string PackageElementName = "Package";
    public readonly string BackgroundTasksElementName = "BackgroundTasks";
    public readonly string TaskElementName = "Task";
    public readonly string InProcessServerElementName = "InProcessServer";
    public readonly string PathElementName = "Path";
    public readonly string ActivatableClassElementName = "ActivatableClass";

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx")]
    public AppxManifestDefs(string prefixDefault, string prefixMobile, string prefixUap)
    {
      this.PackagePath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName);
      this.IdentityPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Identity");
      this.PropertiesDisplayNamePath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Properties", "DisplayName");
      this.PropertiesPublisherDisplayNamePath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Properties", "PublisherDisplayName");
      this.ResourcesPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Resources");
      this.CapabilitiesPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Capabilities");
      this.PropertiesLogoPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Properties", "Logo");
      this.ApplicationPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Applications", "Application");
      this.ExtensionsElementPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Applications", "Application", this.ExtensionsElementName);
      this.ExtensionElementPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, "Applications", "Application", this.ExtensionsElementName, this.ExtensionElementName);
      this.MobileExtensionElementPath = this.ExtensionsElementPath + "/" + XmlUtilites.MakeElementPath(prefixMobile, this.ExtensionElementName);
      this.MobileProjectAExtensionElementPath = this.MobileExtensionElementPath + "/" + XmlUtilites.MakeElementPath(prefixMobile, this.ProjectAElementName);
      this.UapExtensionElementPath = this.ExtensionsElementPath + "/" + XmlUtilites.MakeElementPath(prefixUap, this.ExtensionElementName);
      this.ShareTargetElementPath = this.UapExtensionElementPath + "/" + XmlUtilites.MakeElementPath(prefixUap, this.ShareTargetElementName);
      this.FileTypeAssociationElementPath = this.UapExtensionElementPath + "/" + XmlUtilites.MakeElementPath(prefixUap, this.FileTypeAssociationElementName);
      this.VisualElementsPath = this.ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements");
      this.DefaultTilePath = this.ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements", this.DefaultTileElementName);
      this.DefaultTileElementPrefix = prefixUap;
      this.SplashScreenPath = this.ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements", this.SplashScreenElementName);
      this.SplashScreenElementPrefix = prefixUap;
      this.InitialRotationPreferencePath = this.ApplicationPath + "/" + XmlUtilites.MakeElementPath(prefixUap, "VisualElements", this.InitialRotationPreferenceElementName);
      this.InitialRotationPreferenceElementPrefix = prefixUap;
      this.RotationElementPrefix = prefixUap;
      this.AppLogoAttribute = "Square44x44Logo";
      this.SmallTileLogoAttributeName = "Square71x71Logo";
      this.LargeTileLogoAttributeName = "Square310x310Logo";
      this.PackageExtensionsElementPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, this.ExtensionsElementName);
      this.PackageExtensionElementPath = XmlUtilites.MakeElementPath(prefixDefault, this.PackageElementName, this.ExtensionsElementName, this.ExtensionElementName);
    }

    public string PackagePath { get; private set; }

    public string IdentityPath { get; private set; }

    public string PropertiesDisplayNamePath { get; private set; }

    public string PropertiesPublisherDisplayNamePath { get; private set; }

    public string PropertiesLogoPath { get; private set; }

    public string ResourcesPath { get; private set; }

    public string CapabilitiesPath { get; private set; }

    public string ApplicationPath { get; private set; }

    public string ExtensionsElementPath { get; private set; }

    public string ExtensionElementPath { get; private set; }

    public string MobileExtensionElementPath { get; private set; }

    public string MobileProjectAExtensionElementPath { get; private set; }

    public string UapExtensionElementPath { get; private set; }

    public string ShareTargetElementPath { get; private set; }

    public string FileTypeAssociationElementPath { get; private set; }

    public string PackageExtensionsElementPath { get; private set; }

    public string PackageExtensionElementPath { get; private set; }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
    public string VisualElementsPath { get; protected set; }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
    public string DefaultTilePath { get; protected set; }

    public string DefaultTileElementPrefix { get; protected set; }

    public string SplashScreenElementPrefix { get; protected set; }

    public string InitialRotationPreferenceElementPrefix { get; protected set; }

    public string RotationElementPrefix { get; protected set; }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
    public string SplashScreenPath { get; protected set; }

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StypeCop reports error on some abbreviations such as appx, xmlns")]
    public string InitialRotationPreferencePath { get; protected set; }

    public string AppLogoAttribute { get; protected set; }

    public string SmallTileLogoAttributeName { get; protected set; }

    public string LargeTileLogoAttributeName { get; protected set; }
  }
}
