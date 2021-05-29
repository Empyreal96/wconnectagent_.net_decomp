// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestContentProvider
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestContentProvider
  {
    public ManifestContentProvider(XElement contentProviderXmlElement)
    {
      this.ContentProviderXmlElement = contentProviderXmlElement != null ? contentProviderXmlElement : throw new ArgumentNullException(nameof (contentProviderXmlElement));
      this.PopulateFields();
    }

    public XElement ContentProviderXmlElement { get; private set; }

    public ManifestStringResource Name { get; private set; }

    public bool Enabled { get; private set; }

    private void PopulateFields()
    {
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.ContentProviderXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (!string.IsNullOrEmpty(attributeValueForElement1))
        this.Name = new ManifestStringResource(attributeValueForElement1);
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.ContentProviderXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "enabled");
      if (string.IsNullOrEmpty(attributeValueForElement2) || string.Compare(attributeValueForElement2.ToUpperInvariant(), "TRUE", StringComparison.Ordinal) == 0)
        this.Enabled = true;
      else
        this.Enabled = false;
    }
  }
}
