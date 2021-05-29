// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Decoder.Portable.PortableApkDecoder
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5254C82E-E3C9-4E74-8F95-5E133A0798CA
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkDecoder.Portable.dll

using Microsoft.Arcadia.Marketplace.Decoder.Portable.Manifest.Decoder;
using Microsoft.Arcadia.Marketplace.Decoder.Portable.Resources.Decoder;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.Decoder.Portable
{
  public class PortableApkDecoder
  {
    protected string ManifestAsString { get; set; }

    protected string ApkFilePath { get; set; }

    protected string ApkExtractionPath { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public property is IReadOnly; Needed to support ConcurrentDictionary in inner layers")]
    protected IDictionary<uint, ApkResource> Resources { get; set; }

    protected ApkConfigFile ConfigFile { get; set; }

    protected bool AllowNoResources { get; set; }

    protected async Task DecodeManifestFileAsync()
    {
      if (!string.IsNullOrWhiteSpace(this.ManifestAsString))
        return;
      string apkManifestFilePath = PortableZipUtils.ExtractFileFromZip(this.ApkFilePath, "AndroidManifest.xml", this.ApkExtractionPath);
      if (apkManifestFilePath == null)
        throw new ApkDecoderManifestException("Manifest not found");
      using (XmlDecoder manifestDecoder = new XmlDecoder(apkManifestFilePath))
      {
        try
        {
          this.ManifestAsString = await manifestDecoder.RetrieveStringContentAsync().ConfigureAwait(false);
          LoggerCore.Log(this.ManifestAsString);
        }
        catch (ApkDecoderCommonException ex)
        {
          throw new ApkDecoderManifestException("ManifestXML");
        }
      }
    }

    protected async Task DecodeResourcesFileAsync()
    {
      if (this.Resources != null)
        return;
      string apkResourcesFilePath = PortableZipUtils.ExtractFileFromZip(this.ApkFilePath, "Resources.arsc", this.ApkExtractionPath);
      if (apkResourcesFilePath != null)
      {
        using (ResourcesDecoder resourcesDecoder = new ResourcesDecoder(apkResourcesFilePath))
        {
          try
          {
            this.Resources = await resourcesDecoder.RetrieveApkResourcesAsync().ConfigureAwait(false);
            LoggerCore.Log(resourcesDecoder.ToString());
          }
          catch (ApkDecoderCommonException ex)
          {
            throw new ApkDecoderResourcesException("ResourceFile");
          }
        }
      }
      else
      {
        if (!this.AllowNoResources)
          throw new ApkDecoderResourcesException("Resource file not found");
        this.Resources = (IDictionary<uint, ApkResource>) new Dictionary<uint, ApkResource>();
      }
    }

    protected async Task DecodeConfigFileAsync() => await Task.Run((Action) (() =>
    {
      string fileFromZip = PortableZipUtils.ExtractFileFromZip(this.ApkFilePath, "assets\\MicrosoftServices.xml", this.ApkExtractionPath);
      if (!PortableUtilsServiceLocator.FileUtils.FileExists(fileFromZip))
        return;
      try
      {
        this.ConfigFile = new ApkConfigFile(XDocument.Load(fileFromZip));
      }
      catch (XmlException ex)
      {
        LoggerCore.Log((Exception) ex);
        throw new ApkDecoderConfigException("MicrosoftServices.xml");
      }
    }));
  }
}
