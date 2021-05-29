// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.ImageConfig
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel
{
  public sealed class ImageConfig : IEquatable<ImageConfig>
  {
    public ImageConfig(string scaleQualifier, int widthPixel, int heightPixel, bool mandatory)
    {
      this.ScaleQualifier = scaleQualifier;
      this.WidthPixel = widthPixel;
      this.HeightPixel = heightPixel;
      this.Mandatory = mandatory;
    }

    public string ScaleQualifier { get; private set; }

    public int WidthPixel { get; private set; }

    public int HeightPixel { get; private set; }

    public bool Mandatory { get; private set; }

    public bool Equals(ImageConfig other)
    {
      if (object.ReferenceEquals((object) other, (object) null))
        return false;
      return object.ReferenceEquals((object) this, (object) other) || this.WidthPixel == other.WidthPixel && this.HeightPixel == other.HeightPixel && this.Mandatory.Equals(other.Mandatory) && this.ScaleQualifier.ToString().Equals(other.ScaleQualifier.ToString());
    }

    public override int GetHashCode()
    {
      int num1 = 0;
      int num2 = 397;
      return (((num1 * num2 ^ this.WidthPixel.GetHashCode()) * num2 ^ this.HeightPixel.GetHashCode()) * num2 ^ this.Mandatory.GetHashCode()) * num2 ^ this.ScaleQualifier.GetHashCode();
    }
  }
}
