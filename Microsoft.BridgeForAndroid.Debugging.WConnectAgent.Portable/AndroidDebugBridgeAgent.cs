// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.AndroidDebugBridgeAgent
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class AndroidDebugBridgeAgent : IDisposable
  {
    private IFactory factory;
    private IAdbTrafficMonitor adbTrafficMonitor;
    private InternetEndPoint adbDaemonEndPoint;
    private InternetEndPoint exportEndPoint;
    private WorkScheduler scheduler = new WorkScheduler();
    private ISocket adbDaemonSocket;
    private ISocket adbServerSocket;
    private ISocketConnectWork connector;
    private ISocketAcceptWork acceptor;
    private ISocketReceiveWork adbServerSocketReceiver;
    private ISocketReceiveWork adbDaemonSocketReceiver;
    private ISocketSendWork adbServerSocketSender;
    private ISocketSendWork adbDaemonSocketsender;
    private ChannelJobDispatcher jobDispatcher;
    private EventWaitHandle bootstrappedEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "WConnectAgentBootstrappedEvent");
    private IList<IAdbPacketHandler> serverAdbPacketHandlers = (IList<IAdbPacketHandler>) new List<IAdbPacketHandler>();
    private IList<IAdbPacketHandler> daemonAdbPacketHandlers = (IList<IAdbPacketHandler>) new List<IAdbPacketHandler>();
    private AppxPackageType appxPackageType;

    public AndroidDebugBridgeAgent(
      IFactory factory,
      IAdbTrafficMonitor adbTrafficMonitor,
      InternetEndPoint adbDaemonEndPoint,
      InternetEndPoint exportEndPoint,
      AppxPackageType appxPackageType)
    {
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (adbDaemonEndPoint == null)
        throw new ArgumentNullException(nameof (adbDaemonEndPoint));
      if (exportEndPoint == null)
        throw new ArgumentNullException(nameof (exportEndPoint));
      this.factory = factory;
      this.adbTrafficMonitor = adbTrafficMonitor;
      this.adbDaemonEndPoint = adbDaemonEndPoint;
      this.exportEndPoint = exportEndPoint;
      this.appxPackageType = appxPackageType;
      this.jobDispatcher = new ChannelJobDispatcher();
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
      try
      {
        while (true)
        {
          this.acceptor = this.factory.CreateSocketAcceptWork(this.exportEndPoint);
          this.acceptor.ListenStarted += new EventHandler(this.OnAcceptorListenStarted);
          this.acceptor.SocketAccepted += new EventHandler<SocketAcceptedEventArgs>(this.OnAcceptedConnectionFromAdbServer);
          this.scheduler.AssignWorks((IWork) this.acceptor);
          try
          {
            await this.scheduler.RunAsync(cancellationToken);
            break;
          }
          catch (AdbEngineSocketSendReceiveException ex)
          {
            this.bootstrappedEvent.Reset();
            this.scheduler.AssignWorks();
            this.CloseSockets();
            AndroidDebugBridgeAgent.DisposeObjects((IDisposable) this.connector, (IDisposable) this.acceptor, (IDisposable) this.adbServerSocketReceiver, (IDisposable) this.adbDaemonSocketReceiver, (IDisposable) this.adbServerSocketSender, (IDisposable) this.adbDaemonSocketsender);
            this.daemonAdbPacketHandlers.Clear();
            this.serverAdbPacketHandlers.Clear();
            EtwLogger.Instance.SocketDataSendReceiveError(ex.SocketIdentifier, ex.Reason);
          }
        }
      }
      finally
      {
        this.scheduler.AssignWorks();
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private static void DisposeObjects(params IDisposable[] objectsToDispose)
    {
      foreach (IDisposable disposable in objectsToDispose)
        disposable?.Dispose();
    }

    private static void HandlePacket(
      AdbPacket packet,
      IList<IAdbPacketHandler> handlers,
      AdbPacketSendWork sender)
    {
      EtwLogger.Instance.AdbComandSent(packet.Command, packet.Arg0, packet.Arg1, packet.Data);
      bool flag = false;
      foreach (IAdbPacketHandler handler in (IEnumerable<IAdbPacketHandler>) handlers)
      {
        flag = handler.HandlePacket(packet);
        if (flag)
          break;
      }
      if (flag)
        return;
      sender.EnqueueForSending(packet);
    }

    private void OnConnectedToAdbDaemon(object sender, SocketConnectedEventArgs e)
    {
      EtwLogger.Instance.AdbDaemonConnected();
      this.adbDaemonSocket = e.SocketConnected;
      this.adbServerSocketReceiver = this.factory.CreateSocketReceiveWork(this.adbServerSocket, "ADB server receiver");
      this.adbDaemonSocketReceiver = this.factory.CreateSocketReceiveWork(this.adbDaemonSocket, "ADB daemon receiver");
      this.adbServerSocketSender = this.factory.CreateSocketSendWork(this.adbServerSocket, "ADB server sender");
      this.adbDaemonSocketsender = this.factory.CreateSocketSendWork(this.adbDaemonSocket, "ADB daemon sender");
      AdbPacketReceivWork adbServerReceiver = new AdbPacketReceivWork(this.adbServerSocketReceiver);
      AdbPacketReceivWork adbDaemonReceiver = new AdbPacketReceivWork(this.adbDaemonSocketReceiver);
      AdbPacketSendWork adbServerSender = new AdbPacketSendWork(this.adbServerSocketSender);
      AdbPacketSendWork adbDaemonSender = new AdbPacketSendWork(this.adbDaemonSocketsender);
      adbServerReceiver.AdbPacketReceived += (EventHandler<AdbPacketReceivedEventArgs>) ((originator, param) =>
      {
        if (this.adbTrafficMonitor != null)
          this.adbTrafficMonitor.OnAdbTraffic(param.Packet, AdbTrafficDirection.FromServer);
        if (param.Packet.Command == 1314410051U)
        {
          int maxPacketDataBytes = (int) param.Packet.Arg1;
          adbServerReceiver.MaxPacketBytes = maxPacketDataBytes;
          adbDaemonReceiver.MaxPacketBytes = maxPacketDataBytes;
          AdbChannelClientManager channelClientManager = new AdbChannelClientManager(adbDaemonSender, maxPacketDataBytes);
          this.daemonAdbPacketHandlers.Add((IAdbPacketHandler) channelClientManager);
          if (this.factory.AgentConfiguration.EnableInteractiveShell)
          {
            this.serverAdbPacketHandlers.Add((IAdbPacketHandler) new InteractiveShellTrackerHandler(false));
            this.daemonAdbPacketHandlers.Add((IAdbPacketHandler) new InteractiveShellTrackerHandler(true));
          }
          this.serverAdbPacketHandlers.Add((IAdbPacketHandler) new ApkInstallHandler((IAdbChannelClientManager) channelClientManager, this.factory, this.appxPackageType, adbServerSender, this.jobDispatcher));
          this.serverAdbPacketHandlers.Add((IAdbPacketHandler) new ApkUninstallHandler((IAdbChannelClientManager) channelClientManager, this.factory, adbServerSender, this.jobDispatcher));
          this.serverAdbPacketHandlers.Add((IAdbPacketHandler) new ShellActivityStartHandler((IAdbChannelClientManager) channelClientManager, this.factory, adbServerSender, this.jobDispatcher));
          if (this.factory.AgentConfiguration.EnableInterception)
            this.serverAdbPacketHandlers.Add((IAdbPacketHandler) new FileSyncSnifferHandler(this.factory));
        }
        AndroidDebugBridgeAgent.HandlePacket(param.Packet, this.serverAdbPacketHandlers, adbDaemonSender);
      });
      adbDaemonReceiver.AdbPacketReceived += (EventHandler<AdbPacketReceivedEventArgs>) ((s, p) =>
      {
        if (this.adbTrafficMonitor != null)
          this.adbTrafficMonitor.OnAdbTraffic(p.Packet, AdbTrafficDirection.FromDaemon);
        AndroidDebugBridgeAgent.HandlePacket(p.Packet, this.daemonAdbPacketHandlers, adbServerSender);
      });
      this.scheduler.AssignWorks((IWork) adbServerReceiver, (IWork) adbDaemonReceiver, (IWork) adbServerSender, (IWork) adbDaemonSender);
    }

    private void OnAcceptorListenStarted(object sender, EventArgs e)
    {
      LoggerCore.Log("Socket acceptor started listening.");
      this.bootstrappedEvent.Set();
    }

    private void OnAcceptedConnectionFromAdbServer(object sender, SocketAcceptedEventArgs e)
    {
      EtwLogger.Instance.AdbServerAccepted();
      LoggerCore.Log("Accepted connection from ADB server.");
      this.adbServerSocket = e.SocketAccepted;
      this.connector = this.factory.CreateSocketConnectWork(this.adbDaemonEndPoint, 4U);
      this.connector.SocketConnected += new EventHandler<SocketConnectedEventArgs>(this.OnConnectedToAdbDaemon);
      this.scheduler.AssignWorks((IWork) this.connector);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.bootstrappedEvent != null)
      {
        this.bootstrappedEvent.Reset();
        this.bootstrappedEvent.Dispose();
      }
      if (this.scheduler != null)
      {
        this.scheduler.AssignWorks();
        this.scheduler.Dispose();
        this.scheduler = (WorkScheduler) null;
      }
      this.CloseSockets();
      AndroidDebugBridgeAgent.DisposeObjects((IDisposable) this.connector, (IDisposable) this.acceptor, (IDisposable) this.adbServerSocketReceiver, (IDisposable) this.adbDaemonSocketReceiver, (IDisposable) this.adbServerSocketSender, (IDisposable) this.adbDaemonSocketsender);
    }

    private void CloseSockets()
    {
      if (this.adbDaemonSocket != null)
      {
        this.adbDaemonSocket.Close();
        this.adbDaemonSocket = (ISocket) null;
        EtwLogger.Instance.ForcefullyClosedDaemonSocket();
      }
      if (this.adbServerSocket == null)
        return;
      this.adbServerSocket.Close();
      this.adbServerSocket = (ISocket) null;
      EtwLogger.Instance.ForcefullyClosedServerSocket();
    }
  }
}
