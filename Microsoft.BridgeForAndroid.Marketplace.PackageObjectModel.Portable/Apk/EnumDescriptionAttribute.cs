// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.EnumDescriptionAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public sealed class EnumDescriptionAttribute : Attribute
  {
    private readonly string description;

    public string Description => this.description;

    public EnumDescriptionAttribute(string description) => this.description = description;
  }
}
