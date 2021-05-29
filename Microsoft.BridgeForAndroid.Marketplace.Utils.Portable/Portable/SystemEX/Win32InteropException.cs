// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX.Win32InteropException
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX
{
  [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Type will never be serialized.")]
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Type will never be serialized.")]
  public class Win32InteropException : Exception
  {
    public Win32InteropException(int errorCode)
      : base("A Win32 error has occurred. Error Code =" + errorCode.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    {
      this.HResult = errorCode;
    }

    public Win32InteropException(string errorMessage, int errorCode)
      : base(errorMessage)
    {
    }

    public Win32InteropException(string errorMessage)
      : base(errorMessage)
    {
    }
  }
}
