// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.InternetEndPoint
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class InternetEndPoint
  {
    public InternetEndPoint(string host, int port)
    {
      if (port <= 0)
        throw new ArgumentException("port must be greater then 0", nameof (port));
      this.Host = host;
      this.Port = port;
    }

    public string Host { get; private set; }

    public int Port { get; private set; }
  }
}
