// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestReceiver
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestReceiver : IDevReportReceiver, IDevReportIntentReceiver
  {
    private List<ManifestIntentFilter> filters;

    public ManifestReceiver(XElement receiverXmlElement)
    {
      this.ReceiverXmlElement = receiverXmlElement != null ? receiverXmlElement : throw new ArgumentNullException(nameof (receiverXmlElement));
      this.PopulateFields();
    }

    public XElement ReceiverXmlElement { get; private set; }

    public ManifestStringResource NameResource { get; private set; }

    public ManifestString PermissionString { get; private set; }

    public bool IsEnabled { get; private set; }

    public IReadOnlyList<ManifestIntentFilter> Filters => (IReadOnlyList<ManifestIntentFilter>) this.filters;

    public string Permission => this.PermissionString != null ? this.PermissionString.Content : (string) null;

    public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => this.filters != null ? (IReadOnlyCollection<IDevReportIntentFilter>) this.filters : (IReadOnlyCollection<IDevReportIntentFilter>) new List<IDevReportIntentFilter>();

    private void PopulateFields()
    {
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.ReceiverXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (!string.IsNullOrEmpty(attributeValueForElement1))
        this.NameResource = new ManifestStringResource(attributeValueForElement1);
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.ReceiverXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "permission");
      if (!string.IsNullOrEmpty(attributeValueForElement2))
        this.PermissionString = new ManifestString("permission", attributeValueForElement2);
      string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(this.ReceiverXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "enabled");
      if (!string.IsNullOrEmpty(attributeValueForElement3))
      {
        bool result;
        if (bool.TryParse(attributeValueForElement3, out result))
          this.IsEnabled = result;
      }
      else
        this.IsEnabled = true;
      this.filters = new List<ManifestIntentFilter>();
      foreach (XElement descendant in this.ReceiverXmlElement.Descendants((XName) "intent-filter"))
        this.filters.Add(new ManifestIntentFilter(descendant));
    }
  }
}
