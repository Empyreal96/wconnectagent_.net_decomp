// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.GameServicesConfigWriter
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Arcadia.Marketplace.Converter
{
  public sealed class GameServicesConfigWriter
  {
    private GameServicesConfig gameServicesConfig;

    public GameServicesConfigWriter(GameServicesConfig gameServicesConfig) => this.gameServicesConfig = gameServicesConfig != null ? gameServicesConfig : throw new ArgumentNullException(nameof (gameServicesConfig));

    public void WriteToFile(string outputFilePath)
    {
      if (string.IsNullOrEmpty(outputFilePath))
        throw new ArgumentException("File path is null or empty", nameof (outputFilePath));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{" + Environment.NewLine + "\"TitleId\" : \"{0}\"," + Environment.NewLine + "\"PrimaryServiceConfigId\" : \"{1}\"," + Environment.NewLine + "\"Sandbox\" : \"{2}\"," + Environment.NewLine + "\"UseDeviceToken\" : \"{3}\"," + Environment.NewLine + "}}", (object) this.gameServicesConfig.TitleId, (object) this.gameServicesConfig.PrimaryServiceConfigId, (object) this.gameServicesConfig.Sandbox, (object) this.gameServicesConfig.UseDeviceToken);
      using (StreamWriter streamWriter = new StreamWriter(PortableUtilsServiceLocator.FileUtils.OpenOrCreateFileStream(outputFilePath)))
        streamWriter.Write(str);
    }
  }
}
