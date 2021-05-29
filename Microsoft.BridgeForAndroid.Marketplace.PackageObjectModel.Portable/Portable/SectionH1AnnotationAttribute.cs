// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.SectionH1AnnotationAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  [AttributeUsage(AttributeTargets.All)]
  public sealed class SectionH1AnnotationAttribute : Attribute
  {
    public int Order { get; private set; }

    public SectionH1AnnotationAttribute(int order) => this.Order = order;
  }
}
