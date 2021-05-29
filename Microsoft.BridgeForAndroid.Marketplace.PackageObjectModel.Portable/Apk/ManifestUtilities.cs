// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestUtilities
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  internal static class ManifestUtilities
  {
    internal static List<DevReportMetadata> GetMetadata(
      XElement containerElement)
    {
      List<DevReportMetadata> devReportMetadataList = new List<DevReportMetadata>();
      foreach (XElement descendant in containerElement.Descendants((XName) "meta-data"))
      {
        string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
        if (attributeValueForElement != null)
          devReportMetadataList.Add(new DevReportMetadata(attributeValueForElement));
      }
      return devReportMetadataList;
    }

    internal static List<ManifestIntentFilter> GetIntentFilters(
      XElement containerElemenet)
    {
      List<ManifestIntentFilter> manifestIntentFilterList = new List<ManifestIntentFilter>();
      foreach (XElement descendant in containerElemenet.Descendants((XName) "intent-filter"))
        manifestIntentFilterList.Add(new ManifestIntentFilter(descendant));
      return manifestIntentFilterList;
    }
  }
}
