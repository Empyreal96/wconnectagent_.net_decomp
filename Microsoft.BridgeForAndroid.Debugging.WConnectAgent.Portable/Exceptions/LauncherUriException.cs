// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions.LauncherUriException
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions
{
  public class LauncherUriException : Exception
  {
    public LauncherUriException()
      : base("Error launching the URI.")
    {
    }

    public LauncherUriException(string message)
      : base(message)
    {
    }

    public LauncherUriException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public LauncherUriException(Exception ex)
      : base("Error launching the URI.", ex)
    {
    }
  }
}
