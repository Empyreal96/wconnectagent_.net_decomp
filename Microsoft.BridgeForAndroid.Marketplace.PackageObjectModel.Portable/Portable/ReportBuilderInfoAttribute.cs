// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.ReportBuilderInfoAttribute
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class ReportBuilderInfoAttribute : Attribute
  {
    public ReportBuilderInfoAttribute(string owner, string messagePrefix)
    {
      if (string.IsNullOrWhiteSpace(owner))
        throw new ArgumentException("Owner must not be null or empty.", nameof (owner));
      if (string.IsNullOrWhiteSpace(messagePrefix))
        throw new ArgumentException("messagePrefix must not be null or empty.", nameof (messagePrefix));
      this.Owner = owner;
      this.MessagePrefix = messagePrefix.ToUpperInvariant();
    }

    public string Owner { get; private set; }

    public string MessagePrefix { get; private set; }
  }
}
