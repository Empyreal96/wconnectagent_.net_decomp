// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions.AndroidPackageResolveException
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
  public class AndroidPackageResolveException : Exception
  {
    public AndroidPackageResolveException()
      : base("Error resolving Android Package.")
    {
    }

    public AndroidPackageResolveException(string message)
      : base(message)
    {
    }

    public AndroidPackageResolveException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public AndroidPackageResolveException(Exception ex)
      : base("Error resolving Android Package.", ex)
    {
    }
  }
}
