// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestUsesSdk
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestUsesSdk
  {
    public ManifestUsesSdk(XElement usesSdkElement)
    {
      this.UsesSdkXmlElement = usesSdkElement != null ? usesSdkElement : throw new ArgumentNullException(nameof (usesSdkElement));
      this.PopulateFields();
    }

    public ManifestStringResource MinSdkVersion { get; private set; }

    public ManifestStringResource MaxSdkVersion { get; private set; }

    public ManifestStringResource TargetSdkVersion { get; private set; }

    private XElement UsesSdkXmlElement { get; set; }

    private static ManifestStringResource PopulateSdkVersion(
      XElement element,
      string manifestusessdkversionattribute)
    {
      ManifestStringResource manifestStringResource = (ManifestStringResource) null;
      if (XmlUtilites.IsAttributeFound(element, (XNamespace) "http://schemas.android.com/apk/res/android", manifestusessdkversionattribute))
        manifestStringResource = new ManifestStringResource(XmlUtilites.GetAttributeValueForElement(element, (XNamespace) "http://schemas.android.com/apk/res/android", manifestusessdkversionattribute));
      return manifestStringResource;
    }

    private void PopulateFields()
    {
      this.MaxSdkVersion = ManifestUsesSdk.PopulateSdkVersion(this.UsesSdkXmlElement, "maxSdkVersion");
      this.MinSdkVersion = ManifestUsesSdk.PopulateSdkVersion(this.UsesSdkXmlElement, "minSdkVersion");
      this.TargetSdkVersion = ManifestUsesSdk.PopulateSdkVersion(this.UsesSdkXmlElement, "targetSdkVersion");
    }
  }
}
