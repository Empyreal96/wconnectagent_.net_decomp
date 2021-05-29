// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestActivity
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ManifestActivity : 
    IDevReportActivity,
    IDevReportIntentReceiver,
    IDevReportMetadataContainer
  {
    private List<ManifestIntentFilter> filters;
    private List<DevReportMetadata> metadata;
    private IDictionary<string, ApkScreenOrientationType> screenOrientationMapping = (IDictionary<string, ApkScreenOrientationType>) new Dictionary<string, ApkScreenOrientationType>()
    {
      {
        "0",
        ApkScreenOrientationType.Landscape
      },
      {
        "1",
        ApkScreenOrientationType.Portrait
      },
      {
        "2",
        ApkScreenOrientationType.User
      },
      {
        "3",
        ApkScreenOrientationType.Behind
      },
      {
        "4",
        ApkScreenOrientationType.Sensor
      },
      {
        "5",
        ApkScreenOrientationType.Nosensor
      },
      {
        "6",
        ApkScreenOrientationType.SensorLandscape
      },
      {
        "7",
        ApkScreenOrientationType.SensorPortrait
      },
      {
        "8",
        ApkScreenOrientationType.ReverseLandscape
      },
      {
        "9",
        ApkScreenOrientationType.ReversePortrait
      },
      {
        "10",
        ApkScreenOrientationType.FullSensor
      },
      {
        "11",
        ApkScreenOrientationType.UserLandscape
      },
      {
        "12",
        ApkScreenOrientationType.UserPortrait
      },
      {
        "13",
        ApkScreenOrientationType.FullUser
      },
      {
        "14",
        ApkScreenOrientationType.Locked
      },
      {
        "landscape",
        ApkScreenOrientationType.Landscape
      },
      {
        "landscapeFlipped",
        ApkScreenOrientationType.LandscapeFlipped
      },
      {
        "portrait",
        ApkScreenOrientationType.Portrait
      },
      {
        "portraitFlipped",
        ApkScreenOrientationType.PortraitFlipped
      },
      {
        "user",
        ApkScreenOrientationType.User
      },
      {
        "behind",
        ApkScreenOrientationType.Behind
      },
      {
        "sensor",
        ApkScreenOrientationType.Sensor
      },
      {
        "nosensor",
        ApkScreenOrientationType.Nosensor
      },
      {
        "sensorLandscape",
        ApkScreenOrientationType.SensorLandscape
      },
      {
        "sensorPortrait",
        ApkScreenOrientationType.SensorPortrait
      },
      {
        "reverseLandscape",
        ApkScreenOrientationType.ReverseLandscape
      },
      {
        "reversePortrait",
        ApkScreenOrientationType.ReversePortrait
      },
      {
        "fullSensor",
        ApkScreenOrientationType.FullSensor
      },
      {
        "userLandscape",
        ApkScreenOrientationType.UserLandscape
      },
      {
        "userPortrait",
        ApkScreenOrientationType.UserPortrait
      },
      {
        "fullUser",
        ApkScreenOrientationType.FullUser
      },
      {
        "locked",
        ApkScreenOrientationType.Locked
      }
    };

    public ManifestActivity(XElement activityXmlElement)
    {
      this.ActivityXmlElement = activityXmlElement != null ? activityXmlElement : throw new ArgumentNullException(nameof (activityXmlElement));
      this.PopulateFields();
    }

    public XElement ActivityXmlElement { get; private set; }

    public ManifestStringResource Name { get; private set; }

    public ManifestStringResource Label { get; private set; }

    public ManifestStringResource Theme { get; private set; }

    public bool HasMainActivity { get; private set; }

    public bool IsLauncherCategory { get; private set; }

    public bool IsHomeCategory { get; private set; }

    public string NameString => this.Name != null ? this.Name.Content : (string) null;

    public string LaunchModeValue => this.LaunchModeString != null ? this.LaunchModeString.Content : (string) null;

    public IReadOnlyCollection<IDevReportIntentFilter> IntentFilters => (IReadOnlyCollection<IDevReportIntentFilter>) this.filters;

    public IReadOnlyCollection<DevReportMetadata> Metadata => (IReadOnlyCollection<DevReportMetadata>) this.metadata;

    public ApkScreenOrientationType ScreenOrientation { get; private set; }

    public ManifestString LaunchModeString { get; private set; }

    public IReadOnlyList<ManifestIntentFilter> Filters => (IReadOnlyList<ManifestIntentFilter>) this.filters;

    private void PopulateFields()
    {
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(this.ActivityXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (!string.IsNullOrEmpty(attributeValueForElement1))
        this.Name = new ManifestStringResource(attributeValueForElement1);
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(this.ActivityXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "label");
      if (!string.IsNullOrEmpty(attributeValueForElement2))
        this.Label = new ManifestStringResource(attributeValueForElement2);
      string attributeValueForElement3 = XmlUtilites.GetAttributeValueForElement(this.ActivityXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "theme");
      if (!string.IsNullOrEmpty(attributeValueForElement3))
        this.Theme = new ManifestStringResource(attributeValueForElement3);
      string attributeValueForElement4 = XmlUtilites.GetAttributeValueForElement(this.ActivityXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "launchMode");
      if (!string.IsNullOrEmpty(attributeValueForElement4))
        this.LaunchModeString = new ManifestString("launchMode", attributeValueForElement4);
      this.filters = ManifestUtilities.GetIntentFilters(this.ActivityXmlElement);
      string attributeValueForElement5 = XmlUtilites.GetAttributeValueForElement(this.ActivityXmlElement, (XNamespace) "http://schemas.android.com/apk/res/android", "screenOrientation");
      this.ScreenOrientation = ApkScreenOrientationType.Undeclared;
      if (attributeValueForElement5 != null && this.screenOrientationMapping.ContainsKey(attributeValueForElement5))
        this.ScreenOrientation = this.screenOrientationMapping[attributeValueForElement5];
      foreach (ManifestIntentFilter filter in this.filters)
      {
        if (filter.Actions.Contains<string>("android.intent.action.MAIN"))
          this.HasMainActivity = true;
        if (filter.Categories.Contains<string>("android.intent.category.LAUNCHER"))
          this.IsLauncherCategory = true;
        if (filter.Categories.Contains<string>("android.intent.category.HOME"))
          this.IsHomeCategory = true;
        if (this.HasMainActivity && this.IsLauncherCategory)
        {
          if (this.IsHomeCategory)
            break;
        }
      }
      this.metadata = ManifestUtilities.GetMetadata(this.ActivityXmlElement);
    }
  }
}
