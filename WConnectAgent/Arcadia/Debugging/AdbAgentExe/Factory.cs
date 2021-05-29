// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Factory
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Debugging.AdbAgent.Mobile;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.AdbEngine.Mobile;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Portable.SystemEX;
using Microsoft.Arcadia.Marketplace.Utils.Shareable;
using System;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  public sealed class Factory : IFactory, IDisposable
  {
    private Microsoft.Arcadia.Debugging.AdbAgentExe.AgentConfiguration configuration = new Microsoft.Arcadia.Debugging.AdbAgentExe.AgentConfiguration();
    private IAowInstanceWrapper aowInstance = (IAowInstanceWrapper) new AowInstanceWrapper();
    private bool hasDisposed;

    public AppxPackageType AppxPackageType => AppxPackageType.Phone;

    public IAgentConfiguration AgentConfiguration => (IAgentConfiguration) this.configuration;

    public IAowInstanceWrapper AowInstance => this.aowInstance;

    ISocketAcceptWork IFactory.CreateSocketAcceptWork(
      InternetEndPoint listenEndPoint)
    {
      if (listenEndPoint == null)
        throw new ArgumentNullException(nameof (listenEndPoint));
      return (ISocketAcceptWork) new SocketAcceptWork(listenEndPoint.Host, listenEndPoint.Port);
    }

    ISocketConnectWork IFactory.CreateSocketConnectWork(
      InternetEndPoint connectEndPoint,
      uint attempts)
    {
      if (connectEndPoint == null)
        throw new ArgumentNullException(nameof (connectEndPoint));
      return (ISocketConnectWork) new SocketConnectWork(connectEndPoint.Host, connectEndPoint.Port, attempts);
    }

    ISocketReceiveWork IFactory.CreateSocketReceiveWork(
      ISocket socket,
      string identifier)
    {
      return (ISocketReceiveWork) new SocketReceiveWork(socket, identifier);
    }

    ISocketSendWork IFactory.CreateSocketSendWork(
      ISocket socket,
      string identifier)
    {
      return (ISocketSendWork) new SocketSendWork(socket, identifier);
    }

    public IPortableRepositoryHandler CreateRepository() => (IPortableRepositoryHandler) new RepositoryHandler((IFactory) this, (IAgentConfiguration) this.configuration);

    IPackageManager IFactory.CreatePackageManager() => (IPackageManager) new PackageManagerMobile();

    public IPortableFileUtils CreatePortableFileUtils() => (IPortableFileUtils) new FileUtils();

    public IPortableResourceUtils CreatePortableResourceUtils() => (IPortableResourceUtils) new Microsoft.Arcadia.Marketplace.Utils.Portable.FileSystem.ResourceHelper();

    public ISystemInformation CreateSystemInformation() => (ISystemInformation) new SystemInformation();

    public IProcessRunnerFactory CreateProcessRunnerFactory() => (IProcessRunnerFactory) new PortableProcessRunnerFactory();

    public IUriLauncher CreateUriLauncher() => (IUriLauncher) new UriLauncherMobile();

    public IShellManager CreateShellManager() => (IShellManager) new ShellManagerMobile();

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing || this.hasDisposed)
        return;
      if (this.aowInstance != null)
        this.aowInstance.Dispose();
      this.hasDisposed = true;
    }
  }
}
