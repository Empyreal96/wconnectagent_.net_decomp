// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellParamBase
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  public class ShellParamBase
  {
    private const string SystemBinAbsoluteRemotePath = "/system/bin";
    private const string InlineShellPrefix = "shell:";

    protected ShellParamBase()
    {
    }

    public bool FromInteractiveShell { get; protected set; }

    public static bool IsSystemCommand(
      string expectedSystemCommand,
      string candidateShellCommand,
      bool fromInteractiveShell)
    {
      if (string.IsNullOrEmpty(expectedSystemCommand))
        throw new ArgumentException("expectedSystemCommand cannot be null or empty.", nameof (expectedSystemCommand));
      if (string.IsNullOrEmpty(candidateShellCommand))
        return false;
      string strA1 = "/system/bin" + "/" + expectedSystemCommand;
      if (fromInteractiveShell)
        return string.CompareOrdinal(strA1, candidateShellCommand) == 0 || string.CompareOrdinal(expectedSystemCommand, candidateShellCommand) == 0;
      string strA2 = "shell:" + strA1;
      string strA3 = "shell:" + expectedSystemCommand;
      return string.CompareOrdinal(strA2, candidateShellCommand) == 0 || string.CompareOrdinal(strA3, candidateShellCommand) == 0;
    }
  }
}
