// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.ConfigWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  public sealed class ConfigWriter
  {
    private string outputFilePath;

    public ConfigWriter(string outputFilePath) => this.outputFilePath = !string.IsNullOrEmpty(outputFilePath) ? outputFilePath : throw new ArgumentException("File path is null or empty", nameof (outputFilePath));

    public string AndroidPackageId { get; set; }

    public void WriteToFile()
    {
      string input = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<application>" + Environment.NewLine + "  <packageId>Place Holder</packageId>" + Environment.NewLine + "</application>";
      if (string.IsNullOrEmpty(this.AndroidPackageId))
        throw new ConverterException("APPX Package Id is required");
      XmlDocWriter xmlDocWriter = new XmlDocWriter(input, InputType.XmlString);
      xmlDocWriter.SetElementInnerText("application/packageId", this.AndroidPackageId);
      xmlDocWriter.WriteToFile(this.outputFilePath);
    }
  }
}
