// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk.ApkConfigFile
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk
{
  public sealed class ApkConfigFile
  {
    public GameServicesConfig GameServices { get; private set; }

    public AnalyticsConfig Analytics { get; private set; }

    private XDocument XmlDoc { get; set; }

    public ApkConfigFile(XDocument configXml)
    {
      this.XmlDoc = configXml != null ? configXml : throw new ArgumentNullException(nameof (configXml));
      this.PopulateFields();
    }

    private void PopulateFields()
    {
      LoggerCore.Log("APK Config File, populating fields");
      foreach (XElement descendant in this.XmlDoc.Descendants((XName) "service"))
      {
        string attributeValueForElement = XmlUtilites.GetAttributeValueForElement(descendant, (XNamespace) string.Empty, "name");
        if (string.Compare(attributeValueForElement, "games", StringComparison.OrdinalIgnoreCase) == 0)
          this.PopulateGamesConfig(descendant);
        else if (string.Compare(attributeValueForElement, "analytics", StringComparison.OrdinalIgnoreCase) == 0)
          this.PopulateAnalytics(descendant);
      }
    }

    private void PopulateGamesConfig(XElement serviceElement)
    {
      this.GameServices = new GameServicesConfig();
      XElement xelement1 = serviceElement.Element((XName) "titleId");
      if (xelement1 != null)
        this.GameServices.TitleId = xelement1.Value;
      XElement xelement2 = serviceElement.Element((XName) "primaryServiceConfigId");
      if (xelement2 != null)
        this.GameServices.PrimaryServiceConfigId = xelement2.Value;
      XElement xelement3 = serviceElement.Element((XName) "sandbox");
      if (xelement3 != null)
        this.GameServices.Sandbox = xelement3.Value;
      XElement xelement4 = serviceElement.Element((XName) "useDeviceToken");
      if (xelement4 == null)
        return;
      this.GameServices.UseDeviceToken = xelement4.Value;
    }

    private void PopulateAnalytics(XElement serviceElement)
    {
      this.Analytics = new AnalyticsConfig();
      XElement xelement = serviceElement.Element((XName) "key");
      if (xelement == null)
        return;
      this.Analytics.AnalyticsKey = xelement.Value;
    }
  }
}
