// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.IFactory
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public interface IFactory : IDisposable
  {
    AppxPackageType AppxPackageType { get; }

    IAgentConfiguration AgentConfiguration { get; }

    IAowInstanceWrapper AowInstance { get; }

    ISocketAcceptWork CreateSocketAcceptWork(InternetEndPoint listenEndPoint);

    ISocketConnectWork CreateSocketConnectWork(
      InternetEndPoint connectEndPoint,
      uint attempts);

    ISocketReceiveWork CreateSocketReceiveWork(ISocket socket, string identifier);

    ISocketSendWork CreateSocketSendWork(ISocket socket, string identifier);

    IPackageManager CreatePackageManager();

    IUriLauncher CreateUriLauncher();

    IPortableRepositoryHandler CreateRepository();

    IPortableFileUtils CreatePortableFileUtils();

    IPortableResourceUtils CreatePortableResourceUtils();

    ISystemInformation CreateSystemInformation();

    IProcessRunnerFactory CreateProcessRunnerFactory();

    IShellManager CreateShellManager();
  }
}
