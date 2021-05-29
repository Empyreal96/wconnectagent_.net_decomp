// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.AdbEngineSocketSendReceiveException
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public class AdbEngineSocketSendReceiveException : AdbEngineSocketException
  {
    public AdbEngineSocketSendReceiveException()
    {
    }

    public AdbEngineSocketSendReceiveException(string message)
      : base(message)
    {
    }

    public AdbEngineSocketSendReceiveException(string socketIdentifier, string reason)
    {
      this.SocketIdentifier = socketIdentifier;
      this.Reason = reason;
    }

    public AdbEngineSocketSendReceiveException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public string SocketIdentifier { get; private set; }

    public string Reason { get; private set; }
  }
}
