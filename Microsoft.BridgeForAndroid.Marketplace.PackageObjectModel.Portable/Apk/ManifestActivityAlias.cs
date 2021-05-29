// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestActivityAlias
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestActivityAlias : 
    IDevReportActivityAlias,
    IDevReportIntentReceiver,
    IDevReportMetadataContainer
  {
    private List<ManifestIntentFilter> filters;
    private List<DevReportMetadata> metadata;

    public ManifestActivityAlias(XElement activityAliasXmlElement)
    {
      this.ActivityAliasXmlElement = activityAliasXmlElement != null ? activityAliasXmlElement : throw new ArgumentNullException(nameof (activityAliasXmlElement));
      this.PopulateFields();
    }

    public XElement ActivityAliasXmlElement { get; private set; }

    public ManifestStringResource Name { get; private set; }

    public ManifestStringResource Label { get; private set; }

    public ManifestStringResource TargetActivity { get; private set; }

    public IReadOnlyList<ManifestIntentFilter> Filters => (IReadOnlyList<ManifestIntentFilter>) this.filters;

    public bool HasMainActivity { get; private set; }

    public bool IsLauncherCategory { get; private set; }

    public bool IsHomeCategory { get; private set; }

    public string TargetActivityString => this.TargetActivity != null ? this.TargetActivity.Content : (string) null;

    public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => (IReadOnlyCollection<IDevReportIntentFilter>) this.filters;

    public IReadOnlyCollection<DevReportMetadata> Metadata => (IReadOnlyCollection<DevReportMetadata>) this.metadata;

    private void PopulateFields()
    {
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.ActivityAliasXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (!string.IsNullOrEmpty(attributeValueForElement1))
        this.Name = new ManifestStringResource(attributeValueForElement1);
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.ActivityAliasXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "label");
      if (!string.IsNullOrEmpty(attributeValueForElement2))
        this.Label = new ManifestStringResource(attributeValueForElement2);
      string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(this.ActivityAliasXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "targetActivity");
      if (!string.IsNullOrEmpty(attributeValueForElement3))
        this.TargetActivity = new ManifestStringResource(attributeValueForElement3);
      this.filters = ManifestUtilities.GetIntentFilters(this.ActivityAliasXmlElement);
      foreach (ManifestIntentFilter filter in this.filters)
      {
        if (filter.Actions.Contains<string>("android.intent.action.MAIN"))
          this.HasMainActivity = true;
        if (filter.Categories.Contains<string>("android.intent.category.LAUNCHER"))
          this.IsLauncherCategory = true;
        if (filter.Categories.Contains<string>("android.intent.category.HOME"))
          this.IsHomeCategory = true;
        if (this.HasMainActivity && this.IsLauncherCategory)
        {
          if (this.IsHomeCategory)
            break;
        }
      }
      this.metadata = ManifestUtilities.GetMetadata(this.ActivityAliasXmlElement);
    }
  }
}
