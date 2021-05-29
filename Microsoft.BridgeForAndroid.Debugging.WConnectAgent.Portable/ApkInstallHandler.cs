// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkInstallHandler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using Microsoft.Arcadia.Marketplace.PackageObjectModel;
using System;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ApkInstallHandler : IAdbPacketHandler
  {
    private IAdbChannelClientManager channelManager;
    private IFactory factory;
    private AppxPackageType appxPackageType;
    private AdbPacketSendWork adbServerSender;
    private ChannelJobDispatcher dispatcher;

    public ApkInstallHandler(
      IAdbChannelClientManager channelManager,
      IFactory factory,
      AppxPackageType appxPackageType,
      AdbPacketSendWork adbServerSender,
      ChannelJobDispatcher dispatcher)
    {
      if (channelManager == null)
        throw new ArgumentNullException(nameof (channelManager));
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (adbServerSender == null)
        throw new ArgumentNullException(nameof (adbServerSender));
      if (dispatcher == null)
        throw new ArgumentNullException(nameof (dispatcher));
      this.factory = factory;
      this.channelManager = channelManager;
      this.appxPackageType = appxPackageType;
      this.adbServerSender = adbServerSender;
      this.dispatcher = dispatcher;
    }

    bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
    {
      if (receivedPacket == null)
        throw new ArgumentNullException(nameof (receivedPacket));
      ShellPmInstallParam shellPmInstallParam = (ShellPmInstallParam) null;
      if (receivedPacket.Command == 1313165391U)
        shellPmInstallParam = ShellPmInstallParam.ParseFromOpen(AdbPacket.ParseStringFromData(receivedPacket.Data));
      else if (this.factory.AgentConfiguration.EnableInteractiveShell && receivedPacket.Command == 1163154007U && InteractiveShellChannels.ChannelExists(receivedPacket.Arg0, receivedPacket.Arg1))
        shellPmInstallParam = ShellPmInstallParam.ParseFromInteractiveShell(Encoding.UTF8.GetString(receivedPacket.Data, 0, receivedPacket.Data.Length));
      if (shellPmInstallParam == null)
        return false;
      this.StartNewInstallJob(receivedPacket, shellPmInstallParam);
      return true;
    }

    private void StartNewInstallJob(AdbPacket receivedPacket, ShellPmInstallParam param) => this.dispatcher.ExecuteJob((ChannelJob) new ApkInstallJob(param, this.factory, this.appxPackageType), this.adbServerSender, this.channelManager, receivedPacket.Arg0, receivedPacket.Arg1);
  }
}
