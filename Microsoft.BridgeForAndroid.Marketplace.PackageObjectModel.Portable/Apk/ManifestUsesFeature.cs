// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestUsesFeature
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Explicitly want to name this class ending with feature.")]
  public sealed class ManifestUsesFeature
  {
    public ManifestUsesFeature(XElement usesFeatureXmlElement)
    {
      this.UsesFeatureXmlElement = usesFeatureXmlElement != null ? usesFeatureXmlElement : throw new ArgumentNullException(nameof (usesFeatureXmlElement));
      this.PopulateFields();
    }

    public XElement UsesFeatureXmlElement { get; private set; }

    public ManifestStringResource Name { get; private set; }

    public bool Required { get; private set; }

    private void PopulateFields()
    {
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.UsesFeatureXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (!string.IsNullOrEmpty(attributeValueForElement1))
        this.Name = new ManifestStringResource(attributeValueForElement1);
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.UsesFeatureXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "required");
      if (!string.IsNullOrEmpty(attributeValueForElement2))
      {
        bool result;
        if (!bool.TryParse(attributeValueForElement2, out result))
          return;
        this.Required = result;
      }
      else
        this.Required = true;
    }
  }
}
