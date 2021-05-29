// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.LogAnnotationAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  [AttributeUsage(AttributeTargets.All)]
  public sealed class LogAnnotationAttribute : Attribute
  {
    public string MessageCode { get; private set; }

    public ReportSectionH1 Category { get; private set; }

    public WorkerLogProvider Provider { get; private set; }

    public WorkerLogLevel Level { get; private set; }

    public string EnumName { get; internal set; }

    public bool Hidden { get; set; }

    public LogAnnotationAttribute(string messageCode, ReportSectionH1 category)
    {
      this.MessageCode = messageCode;
      this.Category = category;
      this.Hidden = false;
      this.Provider = WorkerLogProvider.Analyser;
      if (string.IsNullOrWhiteSpace(messageCode))
        throw new ArgumentException("messageCode must not be null or empty.", nameof (messageCode));
      string str = messageCode.Length == 8 ? messageCode.Substring(0, 2).ToUpperInvariant() : throw new PackageObjectModelException("Incorrect error code length, expecting 8 " + messageCode);
      string upperInvariant = messageCode.Substring(2, 2).ToUpperInvariant();
      switch (str)
      {
        case "ER":
          this.Level = WorkerLogLevel.Error;
          break;
        case "WA":
          this.Level = WorkerLogLevel.Warning;
          break;
        case "IN":
          this.Level = WorkerLogLevel.Info;
          break;
        default:
          throw new PackageObjectModelException("Incorrect error code level prefix " + messageCode);
      }
      switch (upperInvariant)
      {
        case "AN":
          this.Provider = WorkerLogProvider.Analyser;
          break;
        case "CO":
          this.Provider = WorkerLogProvider.Converter;
          break;
        case "DE":
          this.Provider = WorkerLogProvider.Decoder;
          break;
        case "WA":
          this.Provider = WorkerLogProvider.WebApi;
          break;
        case "OT":
          this.Provider = WorkerLogProvider.Other;
          break;
        default:
          throw new PackageObjectModelException("Incorrect error code provider " + messageCode);
      }
    }
  }
}
