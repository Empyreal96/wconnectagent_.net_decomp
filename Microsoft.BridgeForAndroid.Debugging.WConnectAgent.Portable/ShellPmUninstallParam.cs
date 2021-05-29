// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellPmUninstallParam
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ShellPmUninstallParam : ShellParamBase
  {
    private const string CommandName = "pm";
    private const string ShellUninstall = "uninstall";

    private ShellPmUninstallParam(string[] tokens, bool fromInteractiveShell)
    {
      if (tokens.Length < 3)
        throw new InvalidOperationException("Tokens must be of at least length 3.");
      this.FromInteractiveShell = fromInteractiveShell;
      this.ProcessTokens(tokens);
    }

    public string PackageName { get; private set; }

    public bool IsPackageNameSpecified => this.PackageName != null;

    public static ShellPmUninstallParam ParseFromInteractiveShell(
      string command)
    {
      return ShellPmUninstallParam.ParseUninstallParameters(command, true);
    }

    public static ShellPmUninstallParam ParseFromOpen(string command) => ShellPmUninstallParam.ParseUninstallParameters(command, false);

    private static ShellPmUninstallParam ParseUninstallParameters(
      string command,
      bool isFromInteractiveShell)
    {
      string[] tokens = StringParsingUtils.Tokenize(command);
      if (tokens.Length < 3)
        return (ShellPmUninstallParam) null;
      return !ShellParamBase.IsSystemCommand("pm", tokens[0], isFromInteractiveShell) || string.Compare(tokens[1], "uninstall", StringComparison.Ordinal) != 0 ? (ShellPmUninstallParam) null : new ShellPmUninstallParam(tokens, isFromInteractiveShell);
    }

    private void ProcessTokens(string[] tokens)
    {
      Match match = new Regex("^([a-z0-9\\._]+)$", RegexOptions.IgnoreCase).Match(tokens[2]);
      if (!match.Success)
        return;
      this.PackageName = match.Groups[1].Value;
    }
  }
}
