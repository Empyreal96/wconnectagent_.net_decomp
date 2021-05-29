// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandRun
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.IO;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal class CommandRun : LauncherCommand
  {
    public override int MinimumArgumentCount => 0;

    protected override void OnExecute(string[] args)
    {
      if (!File.Exists(PathProvider.AgentExePath))
        throw new InvalidOperationException("Agent doesn't exist in the unpack directory. Have you ran the 'unpack' command?");
      if (!File.Exists(PathProvider.SuccessMarkerPath))
        throw new InvalidOperationException("Success marker doesn't exist in the unpack directory. Have you ran the 'unpack' command?");
      string sessionIdentifier = (string) null;
      if (args.Length > 0)
      {
        sessionIdentifier = args[0];
        this.WriteMessage("Session Identifier: {0}.", (object) sessionIdentifier);
      }
      else
        this.WriteMessage("Session Identifier has not been specified.");
      AgentInstances.SpawnAgentInstance(sessionIdentifier);
    }
  }
}
