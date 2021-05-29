// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Mobile.SocketImpl
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A84214B8-7116-4A16-85C5-B9B27B5D19AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using System;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
  internal class SocketImpl : ISocket
  {
    public SocketImpl(StreamSocket realSocket) => this.RealSocket = realSocket != null ? realSocket : throw new ArgumentNullException(nameof (realSocket));

    public StreamSocket RealSocket { get; private set; }

    void ISocket.Close() => this.RealSocket.Dispose();
  }
}
