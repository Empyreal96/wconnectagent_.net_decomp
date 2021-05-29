// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.StoreManifestWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  public sealed class StoreManifestWriter
  {
    private string outputFilePath;

    public StoreManifestWriter(string outputFilePath) => this.outputFilePath = !string.IsNullOrEmpty(outputFilePath) ? outputFilePath : throw new ArgumentException("File path is null or empty", nameof (outputFilePath));

    public void WriteToFile() => new XmlDocWriter("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<StoreManifest xmlns=\"http://schemas.microsoft.com/appx/2015/StoreManifest\">" + Environment.NewLine + "<Dependencies>" + Environment.NewLine + "   <MemoryDependency MinForeground=\"300MB\" />" + Environment.NewLine + "</Dependencies>" + Environment.NewLine + "</StoreManifest>", InputType.XmlString).WriteToFile(this.outputFilePath);
  }
}
