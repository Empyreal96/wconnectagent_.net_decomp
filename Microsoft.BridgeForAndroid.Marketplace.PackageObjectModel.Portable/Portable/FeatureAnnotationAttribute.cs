// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.FeatureAnnotationAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Reflection;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  [AttributeUsage(AttributeTargets.All)]
  public sealed class FeatureAnnotationAttribute : Attribute
  {
    public string AggregateFeature { get; set; }

    public uint AggregateFeatureMessageVersion { get; set; }

    public bool VisibleInReport { get; set; }

    public ReportSectionH1 SectionH1 { get; private set; }

    public ReportSectionH2 SectionH2 { get; private set; }

    public WorkerLogProvider Provider { get; private set; }

    public string EnumName { get; internal set; }

    public WorkerLogLevel LogLevel { get; private set; }

    public FeatureAnnotationAttribute(ReportSectionH2 sectionH2, WorkerLogLevel logLevel)
    {
      this.AggregateFeature = (string) null;
      this.AggregateFeatureMessageVersion = 0U;
      this.VisibleInReport = true;
      this.SectionH2 = sectionH2;
      this.LogLevel = logLevel;
      this.Provider = WorkerLogProvider.Analyser;
      SectionH2AnnotationAttribute customAttribute = typeof (ReportSectionH2).GetRuntimeField(this.SectionH2.ToString()).GetCustomAttribute<SectionH2AnnotationAttribute>();
      if (customAttribute == null)
        return;
      this.SectionH1 = customAttribute.ParentSection;
    }
  }
}
