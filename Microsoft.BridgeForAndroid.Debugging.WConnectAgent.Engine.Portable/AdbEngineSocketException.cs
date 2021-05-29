// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbEngineSocketException
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public class AdbEngineSocketException : AdbEngineException
  {
    public AdbEngineSocketException()
    {
    }

    public AdbEngineSocketException(string message)
      : base(message)
    {
    }

    public AdbEngineSocketException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
