// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestServiceElement
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public class ManifestServiceElement : IDevReportManifestService, IDevReportIntentReceiver
  {
    private List<IDevReportIntentFilter> intentFilterList;

    public ManifestServiceElement(XElement serviceXmlElement)
    {
      this.ServiceXmlElement = serviceXmlElement != null ? serviceXmlElement : throw new ArgumentNullException(nameof (serviceXmlElement));
      this.PopulateFields();
    }

    public bool IsExported { get; private set; }

    public string ServiceName { get; private set; }

    public string Permission { get; private set; }

    public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => (IReadOnlyCollection<IDevReportIntentFilter>) this.intentFilterList;

    public XElement ServiceXmlElement { get; private set; }

    private void PopulateFields()
    {
      this.ServiceName = XmlUtilites.GetAttributeValueForElement(this.ServiceXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(this.ServiceXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "exported");
      this.IsExported = attributeValueForElement != null && attributeValueForElement.ToUpperInvariant() == bool.TrueString.ToUpperInvariant();
      this.Permission = this.ServiceName = XmlUtilites.GetAttributeValueForElement(this.ServiceXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "permission");
      this.intentFilterList = new List<IDevReportIntentFilter>((IEnumerable<IDevReportIntentFilter>) this.ServiceXmlElement.Elements((XName) "intent-filter").Select<XElement, ManifestIntentFilter>((Func<XElement, ManifestIntentFilter>) (filterElem => new ManifestIntentFilter(filterElem))));
    }
  }
}
