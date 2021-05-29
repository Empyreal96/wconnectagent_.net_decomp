// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ManifestIntentFilterData
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public class ManifestIntentFilterData : IDevReportIntentFilterData
  {
    public ManifestIntentFilterData(
      string host,
      string port,
      string path,
      string scheme,
      string mimeType,
      string pathPattern,
      string pathPrefix)
    {
      this.Host = host;
      this.Port = port;
      this.Path = path;
      this.Scheme = scheme;
      this.MimeType = mimeType;
      this.PathPattern = pathPattern;
      this.PathPrefix = pathPrefix;
    }

    public string Host { get; private set; }

    public string Port { get; private set; }

    public string Path { get; private set; }

    public string Scheme { get; private set; }

    public string MimeType { get; private set; }

    public string PathPattern { get; private set; }

    public string PathPrefix { get; private set; }
  }
}
