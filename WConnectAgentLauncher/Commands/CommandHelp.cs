// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandHelp
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal class CommandHelp : LauncherCommand
  {
    public override int MinimumArgumentCount => 0;

    protected override void OnExecute(string[] args)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("This utility supports the following commands:\n\n");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "help", (object) "Shows this information.");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "unpack", (object) "Unpacks the agent if necessary to the agent directory.");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "clear", (object) "Kills running agent instances and deletes the agent directory.");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "killAll", (object) "Ungracefully terminates all running agent instances.");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "run", (object) "Spawns a new instance of the agent. Args: <Session Identifier>");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "runIfNecessary", (object) "Spawns a new instance of the agent if it is not already running. Args: <ADBD host> <ADBD port> <exported host> <exported port>");
      stringBuilder.AppendFormat("{0, -25}{1, 0}\n", (object) "instances", (object) "Prints a list of running agent instances.");
      this.WriteMessage(stringBuilder.ToString());
    }
  }
}
