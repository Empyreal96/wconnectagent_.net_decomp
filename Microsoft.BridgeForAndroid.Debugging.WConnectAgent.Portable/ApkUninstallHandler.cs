// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ApkUninstallHandler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ApkUninstallHandler : IAdbPacketHandler
  {
    private IAdbChannelClientManager channelManager;
    private IFactory factory;
    private AdbPacketSendWork adbServerSender;
    private ChannelJobDispatcher dispatcher;

    public ApkUninstallHandler(
      IAdbChannelClientManager deviceChannelManager,
      IFactory factory,
      AdbPacketSendWork adbServerSender,
      ChannelJobDispatcher dispatcher)
    {
      if (deviceChannelManager == null)
        throw new ArgumentNullException(nameof (deviceChannelManager));
      if (factory == null)
        throw new ArgumentNullException(nameof (factory));
      if (adbServerSender == null)
        throw new ArgumentNullException(nameof (adbServerSender));
      if (dispatcher == null)
        throw new ArgumentNullException(nameof (dispatcher));
      this.factory = factory;
      this.channelManager = deviceChannelManager;
      this.adbServerSender = adbServerSender;
      this.dispatcher = dispatcher;
    }

    bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
    {
      if (receivedPacket == null)
        throw new ArgumentNullException(nameof (receivedPacket));
      ShellPmUninstallParam pmUninstallParam = (ShellPmUninstallParam) null;
      if (receivedPacket.Command == 1313165391U)
        pmUninstallParam = ShellPmUninstallParam.ParseFromOpen(AdbPacket.ParseStringFromData(receivedPacket.Data));
      else if (this.factory.AgentConfiguration.EnableInteractiveShell && receivedPacket.Command == 1163154007U && InteractiveShellChannels.ChannelExists(receivedPacket.Arg0, receivedPacket.Arg1))
        pmUninstallParam = ShellPmUninstallParam.ParseFromInteractiveShell(Encoding.UTF8.GetString(receivedPacket.Data, 0, receivedPacket.Data.Length));
      if (pmUninstallParam == null)
        return false;
      this.StartNewUninstallJob(receivedPacket, pmUninstallParam);
      return true;
    }

    private void StartNewUninstallJob(AdbPacket receivedPacket, ShellPmUninstallParam param) => this.dispatcher.ExecuteJob((ChannelJob) new ApkUninstallJob(this.factory, param), this.adbServerSender, this.channelManager, receivedPacket.Arg0, receivedPacket.Arg1);
  }
}
