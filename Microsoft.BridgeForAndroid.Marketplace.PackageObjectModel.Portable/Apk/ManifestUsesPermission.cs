// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestUsesPermission
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Explicitly want to name this class ending with permission.")]
  public sealed class ManifestUsesPermission
  {
    public ManifestUsesPermission(XElement usesPermissionXmlElement, int targetSdkVersion)
    {
      if (usesPermissionXmlElement == null)
        throw new ArgumentNullException(nameof (usesPermissionXmlElement));
      this.ImpliedFeatures = new HashSet<string>();
      this.ImpliedPermissions = new HashSet<string>();
      this.PopulateFields(usesPermissionXmlElement, targetSdkVersion);
    }

    public ManifestStringResource Name { get; private set; }

    public ManifestStringResource MaxSdkVersion { get; private set; }

    public HashSet<string> ImpliedFeatures { get; private set; }

    public HashSet<string> ImpliedPermissions { get; private set; }

    private void PopulateFields(XElement element, int targetSdkVersion)
    {
      string attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(element, (XNamespace) "http://schemas.android.com/apk/res/android", "name");
      if (string.IsNullOrEmpty(attributeValueForElement1))
        attributeValueForElement1 = XmlUtilites.GetAttributeValueForElement(element, (XNamespace) "http://schemas.amazon.com/apk/res/android", "name");
      string attributeValueForElement2 = XmlUtilites.GetAttributeValueForElement(element, (XNamespace) "http://schemas.android.com/apk/res/android", "maxSdkVersion");
      if (!string.IsNullOrEmpty(attributeValueForElement2))
        this.MaxSdkVersion = new ManifestStringResource(attributeValueForElement2);
      if (string.IsNullOrEmpty(attributeValueForElement1))
        return;
      this.Name = new ManifestStringResource(attributeValueForElement1);
      this.PopulateImplied(attributeValueForElement1, targetSdkVersion);
    }

    private void PopulateImplied(string fullyQualifiedPermissionName, int targetSdkVersion)
    {
      switch (fullyQualifiedPermissionName)
      {
        case "android.permission.BLUETOOTH":
        case "android.permission.BLUETOOTH_ADMIN":
          if (targetSdkVersion <= 4)
            break;
          this.ImpliedFeatures.Add("android.hardware.bluetooth");
          break;
        case "android.permission.CAMERA":
          this.ImpliedFeatures.Add("android.hardware.camera");
          break;
        case "android.permission.ACCESS_MOCK_LOCATION":
        case "android.permission.ACCESS_LOCATION_EXTRA_COMMANDS":
        case "android.permission.INSTALL_LOCATION_PROVIDER":
          this.ImpliedFeatures.Add("android.hardware.location");
          break;
        case "android.permission.ACCESS_COARSE_LOCATION":
          this.ImpliedFeatures.Add("android.hardware.location");
          this.ImpliedFeatures.Add("android.hardware.location.network");
          break;
        case "android.permission.ACCESS_FINE_LOCATION":
          this.ImpliedFeatures.Add("android.hardware.location");
          this.ImpliedFeatures.Add("android.hardware.location.gps");
          break;
        case "android.permission.RECORD_AUDIO":
          this.ImpliedFeatures.Add("android.hardware.microphone");
          break;
        case "android.permission.CALL_PHONE":
        case "android.permission.CALL_PRIVILEGED":
        case "android.permission.MODIFY_PHONE_STATE":
        case "android.permission.PROCESS_OUTGOING_CALLS":
        case "android.permission.READ_SMS":
        case "android.permission.RECEIVE_SMS":
        case "android.permission.RECEIVE_MMS":
        case "android.permission.RECEIVE_WAP_PUSH":
        case "android.permission.SEND_SMS":
        case "android.permission.WRITE_APN_SETTINGS":
        case "android.permission.WRITE_SMS":
          this.ImpliedFeatures.Add("android.hardware.telephony");
          break;
        case "android.permission.ACCESS_WIFI_STATE":
        case "android.permission.CHANGE_WIFI_STATE":
        case "android.permission.CHANGE_WIFI_MULTICAST_STATE":
          this.ImpliedFeatures.Add("android.hardware.wifi");
          break;
        case "android.permission.WRITE_EXTERNAL_STORAGE":
          this.ImpliedPermissions.Add("android.permission.READ_EXTERNAL_STORAGE");
          break;
        case "android.permission.READ_CONTACTS":
          if (targetSdkVersion >= 16)
            break;
          this.ImpliedPermissions.Add("android.permission.READ_CALL_LOG");
          break;
        case "android.permission.WRITE_CONTACTS":
          if (targetSdkVersion >= 16)
            break;
          this.ImpliedPermissions.Add("android.permission.WRITE_CALL_LOG");
          break;
      }
    }
  }
}
