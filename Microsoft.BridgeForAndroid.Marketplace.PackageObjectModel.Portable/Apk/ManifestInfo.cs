// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestInfo
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestInfo : IDevReportManifestObjectModel
  {
    private IList<ManifestUsesFeature> usesFeatureList;
    private IList<ManifestUsesPermission> usesPermissionList;
    private IList<ManifestServiceElement> servicesList;
    private HashSet<string> allPermissions;
    private IReadOnlyCollection<IDevReportActivity> contradictingActivites;
    private HashSet<string> allUsesFeatures;

    public ManifestInfo(XDocument manifestXml, IDictionary<uint, ApkResource> resources)
    {
      this.XmlDoc = manifestXml != null ? manifestXml : throw new ArgumentNullException(nameof (manifestXml));
      this.PopulateFields(resources);
    }

    public ManifestInfo(XDocument manifestXml)
      : this(manifestXml, (IDictionary<uint, ApkResource>) null)
    {
    }

    public int MinSdkVersion { get; private set; }

    public int TargetSdkVersion { get; private set; }

    public int? MaxSdkVersion { get; private set; }

    public WindowsOSVersion MinWindowsOSVersion { get; set; }

    public string PackageName => this.PackageNameResource != null ? this.PackageNameResource.Content : (string) null;

    public string VersionCodeValue { get; private set; }

    public IDevReportManifestApplication ManifestApplication => (IDevReportManifestApplication) this.Application;

    public IReadOnlyCollection<string> AllPermissions => (IReadOnlyCollection<string>) this.allPermissions.ToList<string>();

    public IReadOnlyCollection<string> AllUsesFeatures => (IReadOnlyCollection<string>) this.allUsesFeatures.ToList<string>();

    public IReadOnlyCollection<IDevReportActivity> AllActivities => this.Application != null && this.Application.Activities != null ? (IReadOnlyCollection<IDevReportActivity>) this.Application.Activities : (IReadOnlyCollection<IDevReportActivity>) new List<IDevReportActivity>();

    public IReadOnlyCollection<IDevReportActivityAlias> AllActivityAliases => this.Application != null && this.Application.ActivityAliases != null ? (IReadOnlyCollection<IDevReportActivityAlias>) this.Application.ActivityAliases : (IReadOnlyCollection<IDevReportActivityAlias>) new List<IDevReportActivityAlias>();

    public IReadOnlyCollection<IDevReportReceiver> AllReceivers => this.Application != null && this.Application.Receivers != null ? (IReadOnlyCollection<IDevReportReceiver>) this.Application.Receivers : (IReadOnlyCollection<IDevReportReceiver>) new List<IDevReportReceiver>();

    public IReadOnlyCollection<IDevReportManifestService> AllServices => this.servicesList == null ? (IReadOnlyCollection<IDevReportManifestService>) new List<IDevReportManifestService>() : (IReadOnlyCollection<IDevReportManifestService>) ((IEnumerable<IDevReportManifestService>) this.servicesList).Where<IDevReportManifestService>((Func<IDevReportManifestService, bool>) (m => !string.IsNullOrWhiteSpace(m.ServiceName))).ToList<IDevReportManifestService>();

    public string ActualPackageName { get; private set; }

    public string ActualVersionName { get; private set; }

    public Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestApplication Application { get; private set; }

    public IReadOnlyCollection<ManifestServiceElement> Services => (IReadOnlyCollection<ManifestServiceElement>) this.servicesList.ToList<ManifestServiceElement>();

    public ManifestStringResource PackageNameResource { get; private set; }

    public IReadOnlyCollection<ManifestUsesFeature> UsesFeatures => (IReadOnlyCollection<ManifestUsesFeature>) this.usesFeatureList.ToList<ManifestUsesFeature>();

    public IReadOnlyCollection<ManifestUsesPermission> UsesPermissions => (IReadOnlyCollection<ManifestUsesPermission>) this.usesPermissionList.ToList<ManifestUsesPermission>();

    public ManifestUsesSdk UsesSdk { get; private set; }

    public ManifestStringResource VersionCode { get; private set; }

    public ManifestStringResource VersionName { get; private set; }

    public XDocument XmlDoc { get; private set; }

    public IReadOnlyCollection<IDevReportActivity> ContradictingActivities
    {
      get
      {
        if (this.contradictingActivites == null)
          this.contradictingActivites = this.Application == null || this.Application.Activities == null ? (IReadOnlyCollection<IDevReportActivity>) new List<IDevReportActivity>() : this.RetrieveContradicingRotatingActivities();
        return this.contradictingActivites;
      }
    }

    public bool HasActivity(string activityName, bool partialMatch) => partialMatch ? this.Application.Activities.Any<ManifestActivity>((Func<ManifestActivity, bool>) (a => a.Name.Content.Contains(activityName))) : this.Application.Activities.Any<ManifestActivity>((Func<ManifestActivity, bool>) (a => a.Name.Content.Equals(activityName)));

    public bool HasLibrary(string libraryName)
    {
      bool flag = false;
      foreach (XElement descendant in this.Application.ApplicationXmlElement.Descendants((XName) "uses-library"))
      {
        if (XmlUtilites.IsAttributeEqual(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "name", libraryName, true))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public bool HasMetadata(string metadataName, bool partialMatch) => partialMatch ? this.Application.MetadataElements.Any<ManifestApplicationMetadata>((Func<ManifestApplicationMetadata, bool>) (m => m.Name.Contains(metadataName))) : this.Application.MetadataElements.Any<ManifestApplicationMetadata>((Func<ManifestApplicationMetadata, bool>) (m => m.Name.Equals(metadataName)));

    public bool HasPermission(string permissionName) => this.UsesPermissions.Any<ManifestUsesPermission>((Func<ManifestUsesPermission, bool>) (p => p.Name.Content.Equals(permissionName)));

    private IReadOnlyCollection<IDevReportActivity> RetrieveContradicingRotatingActivities()
    {
      ManifestActivity manifestActivity = this.Application.Activities.Where<ManifestActivity>((Func<ManifestActivity, bool>) (a => a.HasMainActivity)).FirstOrDefault<ManifestActivity>();
      return manifestActivity == null ? (IReadOnlyCollection<IDevReportActivity>) new List<IDevReportActivity>() : ScreenOrientationMap.GetContradictionRotatingActivites(manifestActivity.ScreenOrientation, (ICollection<ManifestActivity>) this.Application.Activities.Where<ManifestActivity>((Func<ManifestActivity, bool>) (a => !a.HasMainActivity && a.ScreenOrientation != ApkScreenOrientationType.Undeclared)).ToList<ManifestActivity>());
    }

    private void PopulateFields(IDictionary<uint, ApkResource> resources)
    {
      this.MinWindowsOSVersion = WindowsOSVersion.ThresholdTH1;
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.XmlDoc.Root, (XNamespace) string.Empty, "package");
      this.PopulatePackageName(resources, attributeValueForElement1);
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.XmlDoc.Root, (XNamespace) "http://schemas.android.com/apk/res/android", "versionCode");
      this.PopulateVersionCode(resources, attributeValueForElement2);
      string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(this.XmlDoc.Root, (XNamespace) "http://schemas.android.com/apk/res/android", "versionName");
      this.PopulateVersionName(resources, attributeValueForElement3);
      LoggerCore.Log("Manifest Info - Package Name: {0}, Version Code: {1}, Version Name: {2}", (object) attributeValueForElement1, (object) attributeValueForElement2, (object) attributeValueForElement3);
      this.Application = new Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestApplication(this.XmlDoc.Descendants((XName) "application").FirstOrDefault<XElement>() ?? throw new DecoderManifestNoApplicationException("No Application element is found."));
      this.servicesList = (IList<ManifestServiceElement>) new List<ManifestServiceElement>();
      foreach (XElement descendant in this.XmlDoc.Descendants((XName) "service"))
        this.servicesList.Add(new ManifestServiceElement(descendant));
      this.MinSdkVersion = 1;
      this.TargetSdkVersion = 1;
      this.PopulateUsesSdk();
      this.usesPermissionList = (IList<ManifestUsesPermission>) new List<ManifestUsesPermission>();
      foreach (XElement descendant in this.XmlDoc.Descendants((XName) "uses-permission"))
        this.usesPermissionList.Add(new ManifestUsesPermission(descendant, this.TargetSdkVersion));
      this.allPermissions = new HashSet<string>();
      if (this.MinSdkVersion < 4 && this.TargetSdkVersion < 4)
      {
        this.allPermissions.Add("android.permission.READ_PHONE_STATE");
        this.allPermissions.Add("android.permission.WRITE_EXTERNAL_STORAGE");
        this.allPermissions.Add("android.permission.READ_EXTERNAL_STORAGE");
      }
      foreach (ManifestUsesPermission usesPermission in (IEnumerable<ManifestUsesPermission>) this.usesPermissionList)
      {
        this.allPermissions.Add(usesPermission.Name.Content);
        foreach (string impliedPermission in usesPermission.ImpliedPermissions)
          this.allPermissions.Add(impliedPermission);
      }
      this.usesFeatureList = (IList<ManifestUsesFeature>) new List<ManifestUsesFeature>();
      foreach (XElement descendant in this.XmlDoc.Descendants((XName) "uses-feature"))
        this.usesFeatureList.Add(new ManifestUsesFeature(descendant));
      this.allUsesFeatures = new HashSet<string>();
      foreach (ManifestUsesFeature usesFeature in (IEnumerable<ManifestUsesFeature>) this.usesFeatureList)
      {
        if (usesFeature.Name != null)
          this.allUsesFeatures.Add(usesFeature.Name.Content);
      }
    }

    private void PopulateUsesSdk()
    {
      XElement usesSdkElement = this.XmlDoc.Descendants((XName) "uses-sdk").FirstOrDefault<XElement>();
      if (usesSdkElement == null)
        return;
      this.UsesSdk = new ManifestUsesSdk(usesSdkElement);
      if (this.UsesSdk == null)
        return;
      int result;
      if (this.UsesSdk.MinSdkVersion != null && int.TryParse(this.UsesSdk.MinSdkVersion.Content, out result))
      {
        this.MinSdkVersion = result;
        this.TargetSdkVersion = result;
        LoggerCore.Log("Successfully parsed the minSdkVersion {0}", (object) this.MinSdkVersion);
      }
      if (this.UsesSdk.TargetSdkVersion != null && int.TryParse(this.UsesSdk.TargetSdkVersion.Content, out result))
      {
        this.TargetSdkVersion = result;
        LoggerCore.Log("Successfully parsed the targetSdkVersion {0}", (object) this.TargetSdkVersion);
      }
      if (this.UsesSdk.MaxSdkVersion == null || !int.TryParse(this.UsesSdk.MaxSdkVersion.Content, out result))
        return;
      this.MaxSdkVersion = new int?(result);
      LoggerCore.Log("Successfully parsed the maxSdkVersion {0}", (object) this.MaxSdkVersion);
    }

    private void PopulateVersionName(IDictionary<uint, ApkResource> resources, string versionName)
    {
      if (string.IsNullOrWhiteSpace(versionName))
        return;
      this.VersionName = new ManifestStringResource(versionName);
      if (!this.VersionName.IsResource || resources == null)
        return;
      ApkResource resource = ApkResourceHelper.GetResource(this.VersionName, resources);
      if (!resource.Values.Any<ApkResourceValue>())
        return;
      this.ActualVersionName = resource.Values.First<ApkResourceValue>().Value;
    }

    private void PopulateVersionCode(IDictionary<uint, ApkResource> resources, string versionCode)
    {
      this.VersionCode = !string.IsNullOrWhiteSpace(versionCode) ? new ManifestStringResource(versionCode) : throw new DecoderManifestNoVersionCodeException("No version code is found.");
      if (!this.VersionCode.IsResource)
      {
        uint result = 0;
        this.VersionCodeValue = uint.TryParse(this.VersionCode.Content, out result) ? this.VersionCode.Content : throw new DecoderManifestNoVersionCodeException("Invalid version code provided.");
      }
      else
      {
        if (resources == null)
          return;
        ApkResource resource = ApkResourceHelper.GetResource(this.VersionCode, resources);
        if (!resource.Values.Any<ApkResourceValue>())
          return;
        this.VersionCodeValue = resource.Values.First<ApkResourceValue>().Value;
      }
    }

    private void PopulatePackageName(IDictionary<uint, ApkResource> resources, string packageName)
    {
      if (string.IsNullOrWhiteSpace(packageName))
        return;
      this.PackageNameResource = new ManifestStringResource(packageName);
      if (!this.PackageNameResource.IsResource || resources == null)
        return;
      ApkResource resource = ApkResourceHelper.GetResource(this.PackageNameResource, resources);
      if (!resource.Values.Any<ApkResourceValue>())
        return;
      this.ActualPackageName = resource.Values.First<ApkResourceValue>().Value;
    }
  }
}
