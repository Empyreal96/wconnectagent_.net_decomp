// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.UtilsException
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public class UtilsException : Exception
  {
    public UtilsException()
    {
    }

    public UtilsException(string message)
      : base(message)
    {
    }

    public UtilsException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
