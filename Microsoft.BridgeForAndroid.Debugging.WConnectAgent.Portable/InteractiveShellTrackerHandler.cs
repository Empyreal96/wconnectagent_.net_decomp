// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.InteractiveShellTrackerHandler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.AdbProtocol.Portable;
using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class InteractiveShellTrackerHandler : IAdbPacketHandler
  {
    private const string ShellOpenCommand = "shell:";
    private bool directionIsFromAdbd;

    public InteractiveShellTrackerHandler(bool directionIsFromAdbd) => this.directionIsFromAdbd = directionIsFromAdbd;

    bool IAdbPacketHandler.HandlePacket(AdbPacket receivedPacket)
    {
      if (receivedPacket == null)
        throw new ArgumentNullException(nameof (receivedPacket));
      if (this.directionIsFromAdbd && receivedPacket.Command == 1497451343U)
        InteractiveShellChannels.AdbdOpened(receivedPacket.Arg1, receivedPacket.Arg0);
      else if (!this.directionIsFromAdbd && receivedPacket.Command == 1313165391U)
      {
        string stringFromData = AdbPacket.ParseStringFromData(receivedPacket.Data);
        if (stringFromData == null)
          return false;
        string[] strArray = StringParsingUtils.Tokenize(stringFromData);
        if (strArray.Length > 0 && string.Compare(strArray[0], "shell:", StringComparison.OrdinalIgnoreCase) == 0)
          InteractiveShellChannels.AdbServerOpen(receivedPacket.Arg0);
      }
      else if (receivedPacket.Command == 1163086915U)
      {
        if (this.directionIsFromAdbd)
          InteractiveShellChannels.AdbdChannelClose(receivedPacket.Arg1);
        else
          InteractiveShellChannels.AdbServerChannelClose(receivedPacket.Arg1);
      }
      return false;
    }
  }
}
