// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Mobile.SocketAcceptWork
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A84214B8-7116-4A16-85C5-B9B27B5D19AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Sockets.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using System;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Mobile
{
  public sealed class SocketAcceptWork : ISocketAcceptWork, IWork, IDisposable
  {
    private AutoResetEvent signalHandle = new AutoResetEvent(true);
    private SocketAcceptWork.StateValue state;
    private string hostName;
    private int port;
    private StreamSocketListener listener;
    private Task listeningTask;
    private StreamSocket incomingSocket;

    public SocketAcceptWork(string hostName, int port)
    {
      if (port <= 0)
        throw new ArgumentException("Invalid port number", nameof (port));
      this.hostName = hostName;
      this.port = port;
      this.state = SocketAcceptWork.StateValue.NotStarted;
    }

    public event EventHandler<SocketAcceptedEventArgs> SocketAccepted;

    public event EventHandler ListenStarted;

    WaitHandle IWork.SignalHandle => (WaitHandle) this.signalHandle;

    void IWork.DoWork()
    {
      switch (this.state)
      {
        case SocketAcceptWork.StateValue.NotStarted:
          this.state = SocketAcceptWork.StateValue.WaitingForConnection;
          this.listener = new StreamSocketListener();
          StreamSocketListener listener = this.listener;
          // ISSUE: method pointer
          WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<StreamSocketListener, StreamSocketListenerConnectionReceivedEventArgs>>(new Func<TypedEventHandler<StreamSocketListener, StreamSocketListenerConnectionReceivedEventArgs>, EventRegistrationToken>(listener.add_ConnectionReceived), new Action<EventRegistrationToken>(listener.remove_ConnectionReceived), new TypedEventHandler<StreamSocketListener, StreamSocketListenerConnectionReceivedEventArgs>((object) this, __methodptr(OnSocketConnectionReceived)));
          this.listeningTask = this.listener.BindEndpointAsync(this.hostName != null ? new HostName(this.hostName) : (HostName) null, this.port.ToString((IFormatProvider) CultureInfo.InvariantCulture)).AsTask();
          this.listeningTask.ContinueWith(new Action<Task>(this.OnListeningTaskFinished));
          if (this.ListenStarted == null)
            break;
          this.ListenStarted((object) this, EventArgs.Empty);
          break;
        case SocketAcceptWork.StateValue.ConnectionReceived:
          if (this.SocketAccepted != null)
          {
            this.SocketAccepted((object) this, new SocketAcceptedEventArgs((ISocket) new SocketImpl(this.incomingSocket)));
            this.incomingSocket = (StreamSocket) null;
          }
          this.listeningTask.Wait(1000);
          this.listeningTask = (Task) null;
          this.listener.Dispose();
          this.listener = (StreamSocketListener) null;
          break;
        case SocketAcceptWork.StateValue.Error:
          throw new AdbEngineSocketAcceptException();
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
      if (this.listener != null)
        this.listener.Dispose();
      if (this.incomingSocket == null)
        return;
      this.incomingSocket.Dispose();
    }

    private void OnSocketConnectionReceived(
      StreamSocketListener sender,
      StreamSocketListenerConnectionReceivedEventArgs args)
    {
      if (args.Socket != null)
      {
        this.incomingSocket = args.Socket;
        this.state = SocketAcceptWork.StateValue.ConnectionReceived;
      }
      else
        this.state = SocketAcceptWork.StateValue.Error;
      try
      {
        this.signalHandle.Set();
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private void OnListeningTaskFinished(Task task)
    {
      if (task.Exception == null)
        return;
      this.state = SocketAcceptWork.StateValue.Error;
      try
      {
        this.signalHandle.Set();
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private enum StateValue
    {
      NotStarted,
      WaitingForConnection,
      ConnectionReceived,
      Error,
    }
  }
}
