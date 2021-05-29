// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.InteractiveShellChannels
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal static class InteractiveShellChannels
  {
    private static List<InteractiveShellChannels.InteractiveShell> interactiveShells = new List<InteractiveShellChannels.InteractiveShell>();
    private static object lockObj = new object();

    public static bool ChannelExists(uint adbServerChannelId, uint adbdChannelId)
    {
      lock (InteractiveShellChannels.lockObj)
        return InteractiveShellChannels.interactiveShells.Count != 0 && InteractiveShellChannels.interactiveShells.Any<InteractiveShellChannels.InteractiveShell>((Func<InteractiveShellChannels.InteractiveShell, bool>) (m => (int) m.AdbdChannelId == (int) adbdChannelId && (int) m.AdbServerChannelId == (int) adbServerChannelId));
    }

    public static void AdbServerOpen(uint adbServerId)
    {
      lock (InteractiveShellChannels.lockObj)
      {
        if (adbServerId == 0U)
          LoggerCore.Log("ADB Server channel identifier is 0. Ignoring.");
        else if (InteractiveShellChannels.interactiveShells.Where<InteractiveShellChannels.InteractiveShell>((Func<InteractiveShellChannels.InteractiveShell, bool>) (m => (int) m.AdbServerChannelId == (int) adbServerId)).Count<InteractiveShellChannels.InteractiveShell>() > 0)
        {
          LoggerCore.Log(LoggerCore.LogLevels.Warning, "ADB Server Channel ID {0} already exists. Ignoring.", (object) adbServerId);
        }
        else
        {
          InteractiveShellChannels.InteractiveShell interactiveShell = new InteractiveShellChannels.InteractiveShell()
          {
            AdbServerChannelId = adbServerId
          };
          InteractiveShellChannels.interactiveShells.Add(interactiveShell);
          LoggerCore.Log("Created new pending open interactive shell entry. ADB Server Channel ID: {0}.", (object) adbServerId);
        }
      }
    }

    public static void AdbdOpened(uint adbServerId, uint adbdId)
    {
      lock (InteractiveShellChannels.lockObj)
      {
        if (adbServerId == 0U)
          LoggerCore.Log("ADB Server channel identifier specified was 0. Ignoring.");
        else if (adbdId == 0U)
        {
          LoggerCore.Log(LoggerCore.LogLevels.Warning, "ADBD channel identifier specified was 0. Ignoring.");
        }
        else
        {
          if (InteractiveShellChannels.interactiveShells.Count == 0)
            return;
          InteractiveShellChannels.InteractiveShell interactiveShell = InteractiveShellChannels.interactiveShells.Where<InteractiveShellChannels.InteractiveShell>((Func<InteractiveShellChannels.InteractiveShell, bool>) (m => (int) m.AdbServerChannelId == (int) adbServerId)).FirstOrDefault<InteractiveShellChannels.InteractiveShell>();
          if (interactiveShell == null)
            return;
          if (interactiveShell.IsFullyOpened)
            LoggerCore.Log("{0} already opened. Ignoring.", (object) interactiveShell);
          else if ((int) interactiveShell.AdbdChannelId == (int) adbdId)
          {
            LoggerCore.Log(LoggerCore.LogLevels.Warning, "ADBD channel identifier already exists. Ignoring.");
          }
          else
          {
            interactiveShell.AdbdChannelId = adbdId;
            interactiveShell.IsFullyOpened = true;
            LoggerCore.Log("New fully constructed interactive shell: {0}.", (object) interactiveShell);
          }
        }
      }
    }

    public static void AdbServerChannelClose(uint adbdChannelId)
    {
      lock (InteractiveShellChannels.lockObj)
      {
        if (InteractiveShellChannels.interactiveShells.Count == 0 || InteractiveShellChannels.interactiveShells.RemoveAll((Predicate<InteractiveShellChannels.InteractiveShell>) (m => (int) m.AdbdChannelId == (int) adbdChannelId)) != 0)
          return;
        LoggerCore.Log("Interactive shell successfully closed by ADB Server.");
      }
    }

    public static void AdbdChannelClose(uint adbServerChannelId)
    {
      lock (InteractiveShellChannels.lockObj)
      {
        if (InteractiveShellChannels.interactiveShells.Count == 0 || InteractiveShellChannels.interactiveShells.RemoveAll((Predicate<InteractiveShellChannels.InteractiveShell>) (m => (int) m.AdbServerChannelId == (int) adbServerChannelId)) <= 0)
          return;
        LoggerCore.Log("Interactive shell successfully closed by ADBD.");
      }
    }

    private class InteractiveShell
    {
      public uint AdbServerChannelId { get; set; }

      public uint AdbdChannelId { get; set; }

      public bool IsFullyOpened { get; set; }

      public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "ADB Server Channel ID: {0}. ADBD Channel ID: {1}.", new object[2]
      {
        (object) this.AdbServerChannelId,
        (object) this.AdbdChannelId
      });
    }
  }
}
