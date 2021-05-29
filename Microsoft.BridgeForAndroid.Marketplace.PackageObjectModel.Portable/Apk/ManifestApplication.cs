// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestApplication
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestApplication : IDevReportManifestApplication
  {
    private const string DefaultLocaleQualifier = "en-US";
    private IList<ManifestActivity> activities;
    private IList<ManifestActivityAlias> activityAliases;
    private IList<ManifestReceiver> receivers;
    private IList<ManifestApplicationMetadata> metadataElements;
    private IList<ManifestContentProvider> contentProviderList;

    public ManifestApplication(XElement applicationXmlElement)
    {
      this.ApplicationXmlElement = applicationXmlElement != null ? applicationXmlElement : throw new ArgumentNullException(nameof (applicationXmlElement));
      this.PopulateFields();
    }

    public XElement ApplicationXmlElement { get; private set; }

    public ManifestStringResource Label { get; private set; }

    public ManifestStringResource Icon { get; private set; }

    public ManifestApplicationMetadata BackgroundColorData { get; private set; }

    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "GMS is an acronym for Google Mobile Services.", MessageId = "GMS")]
    public ManifestStringResource GMSVersion { get; private set; }

    public IReadOnlyList<ManifestActivity> Activities => (IReadOnlyList<ManifestActivity>) this.activities.ToList<ManifestActivity>();

    public IReadOnlyList<ManifestActivityAlias> ActivityAliases => (IReadOnlyList<ManifestActivityAlias>) this.activityAliases.ToList<ManifestActivityAlias>();

    public IReadOnlyList<ManifestReceiver> Receivers => (IReadOnlyList<ManifestReceiver>) this.receivers.ToList<ManifestReceiver>();

    public IReadOnlyList<ManifestApplicationMetadata> MetadataElements => (IReadOnlyList<ManifestApplicationMetadata>) this.metadataElements.ToList<ManifestApplicationMetadata>();

    public IReadOnlyCollection<ManifestContentProvider> ContentProviders => (IReadOnlyCollection<ManifestContentProvider>) this.contentProviderList.ToList<ManifestContentProvider>();

    public string Permission { get; private set; }

    private void PopulateFields()
    {
      this.PopulateLabel();
      this.PopulateIcon();
      this.PopulateActivities();
      this.PopulateActivityAliases();
      this.PopulateReceivers();
      this.PopulateMetadata();
      this.PopulateContentProvider();
      this.PopulateGMSVersion();
      this.PopulatePermission();
    }

    private void PopulateGMSVersion()
    {
      XElement element = this.ApplicationXmlElement.Descendants((XName) "meta-data").Where<XElement>((Func<XElement, bool>) (el => el.Attribute((XNamespace) "http://schemas.android.com/apk/res/android" + "name").Value == "com.google.android.gms.version")).FirstOrDefault<XElement>();
      if (element == null)
        return;
      string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(element, (XNamespace) "http://schemas.android.com/apk/res/android", "value");
      if (string.IsNullOrEmpty(attributeValueForElement))
        return;
      this.GMSVersion = new ManifestStringResource(attributeValueForElement);
    }

    private void PopulateLabel()
    {
      string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(this.ApplicationXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "label");
      if (string.IsNullOrEmpty(attributeValueForElement))
        return;
      this.Label = new ManifestStringResource(attributeValueForElement);
    }

    public string RetrieveEnglishLabelAsString(IDictionary<uint, ApkResource> resources)
    {
      if (this.Label == null)
        return (string) null;
      if (!this.Label.IsResource)
        return this.Label.Content;
      ApkResource resource = ApkResourceHelper.GetResource(this.Label, resources);
      string str = resource.Values.Where<ApkResourceValue>((Func<ApkResourceValue, bool>) (oneValue => !string.IsNullOrWhiteSpace(oneValue.Config.Locale) && oneValue.Config.Locale.ToUpper().Equals("en-US".ToUpper()))).Select<ApkResourceValue, string>((Func<ApkResourceValue, string>) (oneValue => oneValue.Value)).FirstOrDefault<string>();
      if (string.IsNullOrWhiteSpace(str))
      {
        LoggerCore.Log("A value with config explicitly set to english not found.");
        str = resource.Values.Where<ApkResourceValue>((Func<ApkResourceValue, bool>) (oneValue => string.IsNullOrWhiteSpace(oneValue.Config.Locale))).Select<ApkResourceValue, string>((Func<ApkResourceValue, string>) (oneValue => oneValue.Value)).FirstOrDefault<string>();
      }
      if (string.IsNullOrWhiteSpace(str))
      {
        LoggerCore.Log("English label from manifest is not found. Just returning the first value");
        str = resource.Values[0].Value;
      }
      LoggerCore.Log("English label calculated as: {0}", (object) str);
      return str;
    }

    public IEnumerable<ManifestReceiver> FindReceiversWithAction(
      string actionName)
    {
      foreach (ManifestReceiver receiver in (IEnumerable<ManifestReceiver>) this.Receivers)
      {
        foreach (ManifestIntentFilter filter in (IEnumerable<ManifestIntentFilter>) receiver.Filters)
        {
          if (filter.Actions.Contains<string>(actionName))
            yield return receiver;
        }
      }
    }

    private void PopulateIcon()
    {
      string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(this.ApplicationXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "icon");
      if (string.IsNullOrEmpty(attributeValueForElement))
        return;
      this.Icon = new ManifestStringResource(attributeValueForElement);
    }

    private void PopulateActivities()
    {
      this.activities = (IList<ManifestActivity>) new List<ManifestActivity>();
      foreach (XElement descendant in this.ApplicationXmlElement.Descendants((XName) "activity"))
        this.activities.Add(new ManifestActivity(descendant));
    }

    private void PopulateActivityAliases()
    {
      this.activityAliases = (IList<ManifestActivityAlias>) new List<ManifestActivityAlias>();
      foreach (XElement descendant in this.ApplicationXmlElement.Descendants((XName) "activity-alias"))
        this.activityAliases.Add(new ManifestActivityAlias(descendant));
    }

    private void PopulateReceivers()
    {
      this.receivers = (IList<ManifestReceiver>) new List<ManifestReceiver>();
      foreach (XElement descendant in this.ApplicationXmlElement.Descendants((XName) "receiver"))
        this.receivers.Add(new ManifestReceiver(descendant));
    }

    private void PopulateMetadata()
    {
      this.metadataElements = (IList<ManifestApplicationMetadata>) new List<ManifestApplicationMetadata>();
      foreach (XElement descendant in this.ApplicationXmlElement.Descendants((XName) "meta-data"))
      {
        ManifestApplicationMetadata applicationMetadata = new ManifestApplicationMetadata(descendant);
        this.metadataElements.Add(applicationMetadata);
        if (string.Compare(applicationMetadata.Name, "windows-background", StringComparison.OrdinalIgnoreCase) == 0)
          this.BackgroundColorData = applicationMetadata;
      }
    }

    private void PopulateContentProvider()
    {
      this.contentProviderList = (IList<ManifestContentProvider>) new List<ManifestContentProvider>();
      foreach (XElement descendant in this.ApplicationXmlElement.Descendants((XName) "provider"))
        this.contentProviderList.Add(new ManifestContentProvider(descendant));
    }

    private void PopulatePermission() => this.Permission = XmlUtilites.GetAttributeValueForElement(this.ApplicationXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "permission");
  }
}
