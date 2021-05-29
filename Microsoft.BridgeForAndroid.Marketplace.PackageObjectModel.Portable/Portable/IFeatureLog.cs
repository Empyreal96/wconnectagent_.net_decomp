// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.IFeatureLog
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  public interface IFeatureLog
  {
    bool VisibleInReport { get; }

    ReportSectionH1 SectionH1 { get; }

    ReportSectionH2 SectionH2 { get; }

    WorkerLogProvider FeatureProvider { get; }

    string ProviderName { get; set; }

    string MessageCode { get; }

    bool IsSuppressed { get; set; }

    WorkerLogLevel LogLevel { get; }

    string FeatureSignature { get; }

    int NumberOfFields { get; }

    string GetFieldText(int fieldIndex, CultureInfo culture);

    object[] GetFieldArguments(int fieldIndex);

    string GetFieldTitle(int fieldIndex, CultureInfo culture);
  }
}
