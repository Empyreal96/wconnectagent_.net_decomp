// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.IDevReportObjectModel
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public interface IDevReportObjectModel
  {
    IDevReportManifestObjectModel Manifest { get; }

    IDevReportDexModel Dex { get; }

    IDevReportMiddlewareModel Middleware { get; }

    IDevReportServicesModel Services { get; }

    IDevApkPackageModel Package { get; }
  }
}
