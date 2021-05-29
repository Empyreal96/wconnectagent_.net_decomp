// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestApplicationMetadata
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestApplicationMetadata
  {
    public ManifestApplicationMetadata(XElement metadataXElement)
    {
      this.MetadataXmlElement = metadataXElement;
      this.PopulateFields();
    }

    public XElement MetadataXmlElement { get; private set; }

    public AppxPackageType PackageType { get; private set; }

    public AppxImageType ImageType { get; private set; }

    public string ScaleQualifier { get; private set; }

    public ManifestStringResource Resource { get; private set; }

    public ManifestStringResource Value { get; private set; }

    public string Name { get; private set; }

    public bool IsValidAppxResource { get; private set; }

    private void PopulateFields()
    {
      this.IsValidAppxResource = false;
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.MetadataXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (!string.IsNullOrEmpty(attributeValueForElement1))
      {
        this.Name = attributeValueForElement1.Trim();
        string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.MetadataXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "resource");
        if (!string.IsNullOrEmpty(attributeValueForElement2))
          this.TryPopulateAsAppxResourceMetadata(this.Name, attributeValueForElement2);
      }
      string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(this.MetadataXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "value");
      if (string.IsNullOrEmpty(attributeValueForElement3))
        return;
      this.Value = new ManifestStringResource(attributeValueForElement3.Trim());
    }

    private void TryPopulateAsAppxResourceMetadata(string name, string resource)
    {
      ManifestStringResource manifestStringResource = new ManifestStringResource(resource);
      if (manifestStringResource.IsResource)
      {
        if (Regex.IsMatch(name, "Windows\\.(applogo|storelogo|tilelogosmall|tilelogomedium|tilelogolarge|tilelogowide|splashscreen)\\.scale-(80|100|140|180)", RegexOptions.IgnoreCase))
        {
          char[] chArray = new char[1]{ '.' };
          string[] strArray = name.Split(chArray);
          this.PopulateAppxResourceMetadata(AppxPackageType.Tablet, strArray[1], strArray[2]);
        }
        else if (Regex.IsMatch(name, "Windows\\.phone\\.(applogo|storelogo|tilelogosmall|tilelogomedium|tilelogowide|splashscreen)\\.scale-(100|140|240)", RegexOptions.IgnoreCase))
        {
          char[] chArray = new char[1]{ '.' };
          string[] strArray = name.Split(chArray);
          this.PopulateAppxResourceMetadata(AppxPackageType.Phone, strArray[2], strArray[3]);
        }
      }
      this.Resource = manifestStringResource;
    }

    private void PopulateAppxResourceMetadata(
      AppxPackageType packageType,
      string imageTypeAsString,
      string scaleQualifier)
    {
      LoggerCore.Log("Type of Image = {0}, Scale-Qualifier = {1}", (object) imageTypeAsString, (object) scaleQualifier);
      AppxImageType result;
      if (!Enum.TryParse<AppxImageType>(imageTypeAsString, true, out result))
        return;
      this.PackageType = packageType;
      this.ImageType = result;
      this.ScaleQualifier = scaleQualifier;
      this.IsValidAppxResource = true;
    }
  }
}
