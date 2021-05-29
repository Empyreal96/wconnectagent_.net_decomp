// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.DecoderValueMustBeStringException
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  public class DecoderValueMustBeStringException : ApkFormatException
  {
    public DecoderValueMustBeStringException()
    {
    }

    public DecoderValueMustBeStringException(string xmlAttributeName, string xmlAttributeValue)
      : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Node '{0}' cannot contain resource values. Specified value is: '{1}'.", new object[2]
      {
        (object) xmlAttributeName,
        (object) xmlAttributeValue
      }))
    {
      this.AttributeName = xmlAttributeName;
      this.AttributeContent = xmlAttributeValue;
    }

    public DecoderValueMustBeStringException(string message)
      : base(message)
    {
    }

    public DecoderValueMustBeStringException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public string AttributeName { get; private set; }

    public string AttributeContent { get; private set; }
  }
}
