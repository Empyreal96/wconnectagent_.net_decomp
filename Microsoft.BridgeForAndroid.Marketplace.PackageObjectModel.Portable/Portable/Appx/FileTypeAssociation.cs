// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx.FileTypeAssociation
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.Appx
{
  public class FileTypeAssociation
  {
    public FileTypeAssociation(string name, string fileType)
    {
      this.FileType = fileType;
      this.Name = name;
    }

    public string FileType { get; private set; }

    public string Name { get; private set; }
  }
}
