// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.ShellAmStartParam
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class ShellAmStartParam : ShellParamBase
  {
    private const string DebuggingFlag = "-D";
    private const string DataFlag = "-d";
    private const string CategoryFlag = "-c";
    private const string ComponentFlag = "-n";
    private const string ActionFlag = "-a";
    private const string CommandName = "am";
    private const string AmStart = "start";
    private const string DefaultAction = "android.intent.action.MAIN";
    private const string DefaultCategory = "android.intent.category.LAUNCHER";

    private ShellAmStartParam(string[] tokens, bool isFromInteractiveShell)
    {
      if (tokens.Length < 3)
        throw new InvalidOperationException("Tokens must be of at least length 3.");
      this.Intent = new Intent();
      this.Intent.Action = "android.intent.action.MAIN";
      this.Intent.Category = "android.intent.category.LAUNCHER";
      this.FromInteractiveShell = isFromInteractiveShell;
      this.ProcessTokens(tokens);
    }

    public bool IsDebugging { get; private set; }

    public Intent Intent { get; private set; }

    public bool IntentPresent { get; private set; }

    public static ShellAmStartParam ParseFromOpen(string command) => ShellAmStartParam.ParseInstallParameters(command, false);

    public static ShellAmStartParam ParseFromInteractiveShell(string command) => ShellAmStartParam.ParseInstallParameters(command, true);

    private static ShellAmStartParam ParseInstallParameters(
      string command,
      bool isFromInteractiveShell)
    {
      string[] tokens = StringParsingUtils.Tokenize(command);
      if (tokens.Length < 3)
        return (ShellAmStartParam) null;
      return !ShellParamBase.IsSystemCommand("am", tokens[0], isFromInteractiveShell) || string.Compare(tokens[1], "start", StringComparison.Ordinal) != 0 ? (ShellAmStartParam) null : new ShellAmStartParam(tokens, isFromInteractiveShell);
    }

    private void ProcessTokens(string[] tokens)
    {
      for (int index = 2; index < tokens.Length; ++index)
      {
        string token = tokens[index];
        string rightToken = index < tokens.Length - 1 ? tokens[index + 1] : (string) null;
        if (!this.ParseExplicitComponent(token))
        {
          if (token == "-D")
            this.IsDebugging = true;
          else if (rightToken != null && this.ParsePairedParameters(token, rightToken))
            ++index;
        }
      }
    }

    private bool ParsePairedParameters(string leftToken, string rightToken)
    {
      if (leftToken == "-n")
        return this.ParseExplicitComponent(rightToken);
      if (leftToken == "-a")
        this.Intent.Action = rightToken;
      else if (leftToken == "-c")
        this.Intent.Category = rightToken;
      else
        return leftToken == "-d" && this.ParseDataUri(rightToken);
      return true;
    }

    private bool ParseDataUri(string uriToken)
    {
      Uri result;
      if (!Uri.TryCreate(uriToken, UriKind.RelativeOrAbsolute, out result))
        return false;
      this.Intent.DataUri = result;
      return true;
    }

    private bool ParseExplicitComponent(string token)
    {
      Match match = new Regex("^([a-z0-9\\._]+)/([a-z0-9\\._]+)$", RegexOptions.IgnoreCase).Match(token);
      if (!match.Success)
        return false;
      this.Intent.PackageName = match.Groups[1].Value;
      this.Intent.ActivityName = match.Groups[2].Value;
      this.IntentPresent = true;
      return true;
    }
  }
}
