// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Mobile.SocketConnectWork
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A84214B8-7116-4A16-85C5-B9B27B5D19AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
  public sealed class SocketConnectWork : ISocketConnectWork, IWork, IDisposable
  {
    private AutoResetEvent signalHandle = new AutoResetEvent(true);
    private SocketConnectWork.StateValue state;
    private string remoteAddress;
    private int remotePort;
    private StreamSocket socket;
    private uint attemptsRemaining;

    public SocketConnectWork(string remoteName, int remotePort, uint attempts)
    {
      if (string.IsNullOrEmpty(remoteName))
        throw new ArgumentException("host name or IP must be provided", nameof (remoteName));
      if (remotePort <= 0)
        throw new ArgumentException("Invalid port number", nameof (remotePort));
      if (attempts == 0U)
        throw new ArgumentException("attempts must be greater than zero", nameof (attempts));
      this.remoteAddress = remoteName;
      this.remotePort = remotePort;
      this.state = SocketConnectWork.StateValue.NotStarted;
      this.attemptsRemaining = attempts;
    }

    public event EventHandler<SocketConnectedEventArgs> SocketConnected;

    WaitHandle IWork.SignalHandle => (WaitHandle) this.signalHandle;

    void IWork.DoWork()
    {
      switch (this.state)
      {
        case SocketConnectWork.StateValue.NotStarted:
          --this.attemptsRemaining;
          this.state = SocketConnectWork.StateValue.Connecting;
          this.socket = new StreamSocket();
          this.socket.Control.put_KeepAlive(true);
          this.socket.ConnectAsync(new HostName(this.remoteAddress), this.remotePort.ToString((IFormatProvider) CultureInfo.InvariantCulture)).AsTask().ContinueWith((Action<Task>) (previousTask =>
          {
            if (previousTask.Exception == null)
              this.state = SocketConnectWork.StateValue.Success;
            else if (this.attemptsRemaining > 0U)
            {
              this.socket = (StreamSocket) null;
              this.state = SocketConnectWork.StateValue.NotStarted;
            }
            else
              this.state = SocketConnectWork.StateValue.Error;
            try
            {
              this.signalHandle.Set();
            }
            catch (ObjectDisposedException ex)
            {
            }
          }));
          break;
        case SocketConnectWork.StateValue.Success:
          if (this.SocketConnected == null)
            break;
          this.SocketConnected((object) this, new SocketConnectedEventArgs((ISocket) new SocketImpl(this.socket)));
          this.socket = (StreamSocket) null;
          break;
        case SocketConnectWork.StateValue.Error:
          throw new AdbEngineSocketConnectException();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.signalHandle != null)
        this.signalHandle.Dispose();
      if (this.socket == null)
        return;
      this.socket.Dispose();
    }

    private enum StateValue
    {
      NotStarted,
      Connecting,
      Success,
      Error,
    }
  }
}
