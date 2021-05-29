// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellPmInstallParam
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ShellPmInstallParam : ShellParamBase
  {
    private const string CommandName = "pm";
    private const string ShellInstall = "install";

    private ShellPmInstallParam(
      List<string> options,
      string apkFilePath,
      bool fromInteractiveShell)
    {
      this.Options = (IReadOnlyCollection<string>) options;
      this.ApkFilePath = apkFilePath;
      this.FromInteractiveShell = fromInteractiveShell;
    }

    public IReadOnlyCollection<string> Options { get; private set; }

    public string ApkFilePath { get; private set; }

    public static ShellPmInstallParam ParseFromInteractiveShell(string command) => ShellPmInstallParam.ParseInstallParameters(command, true);

    public static ShellPmInstallParam ParseFromOpen(string command) => ShellPmInstallParam.ParseInstallParameters(command, false);

    private static ShellPmInstallParam ParseInstallParameters(
      string command,
      bool isFromInteractiveShell)
    {
      string[] strArray = StringParsingUtils.Tokenize(command);
      if (strArray.Length < 3)
        return (ShellPmInstallParam) null;
      if (!ShellParamBase.IsSystemCommand("pm", strArray[0], isFromInteractiveShell) || string.Compare(strArray[1], "install", StringComparison.Ordinal) != 0)
        return (ShellPmInstallParam) null;
      List<string> options = (List<string>) null;
      string apkFilePath = (string) null;
      for (int index = 2; index < strArray.Length; ++index)
      {
        string str = strArray[index];
        if (str[0] == '-')
        {
          if (options == null)
            options = new List<string>();
          options.Add(str);
        }
        else if (string.IsNullOrEmpty(apkFilePath))
        {
          apkFilePath = str;
          break;
        }
      }
      return new ShellPmInstallParam(options, apkFilePath, isFromInteractiveShell);
    }
  }
}
