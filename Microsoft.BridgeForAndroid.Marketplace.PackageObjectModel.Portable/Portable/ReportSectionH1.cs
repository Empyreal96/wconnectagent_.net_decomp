// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.ReportSectionH1
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  public enum ReportSectionH1
  {
    [SectionH1Annotation(1), SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "GMS is an acronym for Google Mobile Services.", MessageId = "GMS")] GMSDependencies,
    [SectionH1Annotation(2)] AndroidComponents,
    [SectionH1Annotation(3)] Sensors,
    [SectionH1Annotation(4)] MediaAndGraphics,
    [SectionH1Annotation(5)] ConnectivityAndData,
    InternalAppErrors,
  }
}
