// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk.ApkObjectModel
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Apk;
using Microsoft.Arcadia.Marketplace.PackageTableObjectModel.Portable;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Apk
{
  public sealed class ApkObjectModel
  {
    private readonly IDictionary<uint, ApkResource> resources;
    private Dictionary<string, XDocument> decodedXmlFiles;
    private IPortableRepositoryHandler repositoryHandler;
    private PackageTableDataObjectModel packageTable;

    public ApkObjectModel(
      ManifestInfo manifestInfo,
      IDictionary<uint, ApkResource> resources,
      ApkConfigFile configFile)
      : this(manifestInfo, resources, configFile, (IPortableRepositoryHandler) null)
    {
    }

    public ApkObjectModel(
      ManifestInfo manifestInfo,
      IDictionary<uint, ApkResource> resources,
      ApkConfigFile configFile,
      IPortableRepositoryHandler repositoryHandler)
    {
      this.ManifestInfo = manifestInfo;
      this.resources = resources;
      this.ApkConfigFile = configFile;
      this.repositoryHandler = repositoryHandler;
      this.decodedXmlFiles = new Dictionary<string, XDocument>();
    }

    public ManifestInfo ManifestInfo { get; private set; }

    public ApkConfigFile ApkConfigFile { get; private set; }

    public IReadOnlyDictionary<string, XDocument> DecodedXmlFiles => (IReadOnlyDictionary<string, XDocument>) this.decodedXmlFiles;

    public IDictionary<uint, ApkResource> Resources => this.resources;

    public IPortableRepositoryHandler RepositoryHandler
    {
      get => this.repositoryHandler;
      set => this.repositoryHandler = value;
    }

    public PackageTableDataObjectModel PackageTable
    {
      get
      {
        if (this.packageTable == null)
          this.packageTable = new PackageTableDataObjectModel(this.repositoryHandler.RetrievePackageFilePath(), this.repositoryHandler.RetrieveAndroidAppPackageToolPath());
        return this.packageTable;
      }
      private set => this.packageTable = value;
    }

    public void AddParsedXmlFile(string fileName, XDocument document) => this.decodedXmlFiles.Add(fileName, document);

    public string BuildAppxPackageName()
    {
      string str = this.ManifestInfo.PackageNameResource.Content;
      if (this.ManifestInfo.PackageNameResource.IsResource)
      {
        ApkResource resource = ApkResourceHelper.GetResource(this.ManifestInfo.PackageNameResource, this.resources);
        if (resource.Values.Count <= 0)
          throw new InvalidOperationException("No resource entry for the package name.");
        str = resource.Values[0].Value;
      }
      char[] chArray = new char[4]{ ' ', '_', '-', '.' };
      if (string.IsNullOrWhiteSpace(str))
        throw new InvalidOperationException("Package name is empty or null.");
      foreach (char ch in chArray)
        str = str.Replace(ch.ToString(), string.Empty);
      if (str.Length > 35)
        str = str.Substring(0, 35);
      return "Aow" + str;
    }
  }
}
