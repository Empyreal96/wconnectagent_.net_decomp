// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestIntentFilter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public class ManifestIntentFilter : IDevReportIntentFilter
  {
    private List<string> actions;
    private List<string> categories;
    private List<ManifestIntentFilterData> data;

    public ManifestIntentFilter(XElement intentFilteryXmlElement)
    {
      this.IntentFilterXmlElement = intentFilteryXmlElement != null ? intentFilteryXmlElement : throw new ArgumentNullException(nameof (intentFilteryXmlElement));
      this.PopulateFields();
    }

    public XElement IntentFilterXmlElement { get; private set; }

    public IReadOnlyList<string> Actions => (IReadOnlyList<string>) this.actions;

    public IReadOnlyList<IDevReportIntentFilterData> FilterData => (IReadOnlyList<IDevReportIntentFilterData>) this.data;

    public IReadOnlyList<string> Categories => (IReadOnlyList<string>) this.categories;

    public IReadOnlyList<ManifestIntentFilterData> Data => (IReadOnlyList<ManifestIntentFilterData>) this.data;

    private void PopulateFields()
    {
      this.actions = new List<string>();
      foreach (XElement descendant in this.IntentFilterXmlElement.Descendants((XName) "action"))
      {
        if (XmlUtilites.IsAttributeFound(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "name"))
          this.actions.Add(descendant.Attribute((XNamespace) "http://schemas.android.com/apk/res/android" + "name").Value);
      }
      this.categories = new List<string>();
      foreach (XElement descendant in this.IntentFilterXmlElement.Descendants((XName) "category"))
      {
        if (XmlUtilites.IsAttributeFound(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "name"))
          this.categories.Add(descendant.Attribute((XNamespace) "http://schemas.android.com/apk/res/android" + "name").Value);
      }
      this.data = new List<ManifestIntentFilterData>();
      foreach (XElement descendant in this.IntentFilterXmlElement.Descendants((XName) "data"))
      {
        string host = (string) null;
        string port = (string) null;
        string path = (string) null;
        string pathPrefix = (string) null;
        string pathPattern = (string) null;
        string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "scheme");
        string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "mimeType");
        if (attributeValueForElement1 != null)
        {
          host = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "host");
          if (host != null)
          {
            port = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "port");
            path = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "path");
            pathPrefix = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "pathPrefix");
            pathPattern = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "pathPattern");
          }
        }
        if (attributeValueForElement1 != null || attributeValueForElement2 != null)
          this.data.Add(new ManifestIntentFilterData(host, port, path, attributeValueForElement1, attributeValueForElement2, pathPattern, pathPrefix));
      }
    }
  }
}
