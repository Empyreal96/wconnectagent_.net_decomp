// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.MobilePortableApkDecoder
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Decoder.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class MobilePortableApkDecoder : PortableApkDecoder
  {
    private string correlationId;

    public MobilePortableApkDecoder(IPortableRepositoryHandler repository, string correlationId)
    {
      if (repository == null)
        throw new ArgumentNullException(nameof (repository));
      if (correlationId == null)
        throw new ArgumentNullException(nameof (correlationId));
      this.ApkFilePath = repository.RetrievePackageFilePath();
      this.ApkExtractionPath = repository.RetrievePackageExtractionPath();
      this.correlationId = correlationId;
      this.AllowNoResources = true;
    }

    public ApkObjectModel ObjModel { get; private set; }

    public async Task DecodeAsync()
    {
      EtwLogger.Instance.ApkManifestDecoding(this.correlationId);
      await this.DecodeManifestFileAsync();
      EtwLogger.Instance.ApkManifestDecoded(this.correlationId);
      EtwLogger.Instance.ApkResourcesDecoding(this.correlationId);
      await this.DecodeResourcesFileAsync();
      EtwLogger.Instance.ApkResourcesDecoded(this.correlationId);
      await this.DecodeConfigFileAsync();
      XDocument manifestDocument = XDocument.Parse(this.ManifestAsString);
      ManifestInfo manifestInfo = new ManifestInfo(manifestDocument, this.Resources);
      EtwLogger.Instance.ApkManifestInfo(manifestInfo.PackageNameResource.Content, this.correlationId);
      this.ObjModel = new ApkObjectModel(manifestInfo, this.Resources, this.ConfigFile);
    }
  }
}
