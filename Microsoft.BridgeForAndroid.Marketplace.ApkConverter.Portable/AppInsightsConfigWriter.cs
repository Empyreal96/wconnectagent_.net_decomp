// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.AppInsightsConfigWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  public sealed class AppInsightsConfigWriter
  {
    private string outputFilePath;

    public AppInsightsConfigWriter(string outputFilePath) => this.outputFilePath = !string.IsNullOrEmpty(outputFilePath) ? outputFilePath : throw new ArgumentException("File path is null or empty", nameof (outputFilePath));

    public string InstrumentationKey { get; set; }

    public void WriteToFile()
    {
      string input = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<ApplicationInsights xmlns=\"http://schemas.microsoft.com/ApplicationInsights/2013/Settings\" schemaVersion=\"2014-05-30\">" + Environment.NewLine + "   <InstrumentationKey>key</InstrumentationKey>" + Environment.NewLine + "</ApplicationInsights>" + Environment.NewLine;
      if (string.IsNullOrEmpty(this.InstrumentationKey))
        throw new ConverterException("Instrumentation key is required");
      XmlDocWriter xmlDocWriter = new XmlDocWriter(input, InputType.XmlString);
      xmlDocWriter.AddDefaultNamespace("dft", "http://schemas.microsoft.com/ApplicationInsights/2013/Settings");
      xmlDocWriter.SetElementInnerText(XmlUtilites.MakeElementPath("dft", "ApplicationInsights", "InstrumentationKey"), this.InstrumentationKey);
      xmlDocWriter.WriteToFile(this.outputFilePath);
    }
  }
}
