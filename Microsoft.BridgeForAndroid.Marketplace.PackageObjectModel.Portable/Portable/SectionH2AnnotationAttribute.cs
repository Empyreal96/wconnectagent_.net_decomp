// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.SectionH2AnnotationAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  [AttributeUsage(AttributeTargets.All)]
  public sealed class SectionH2AnnotationAttribute : Attribute
  {
    public ReportSectionH1 ParentSection { get; private set; }

    public int Order { get; private set; }

    public SectionH2AnnotationAttribute(ReportSectionH1 parentSection, int order)
    {
      this.ParentSection = parentSection;
      this.Order = order;
    }
  }
}
