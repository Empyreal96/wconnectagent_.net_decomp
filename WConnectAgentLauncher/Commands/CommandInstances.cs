// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandInstances
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System.Collections.Generic;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal class CommandInstances : LauncherCommand
  {
    public override int MinimumArgumentCount => 0;

    protected override void OnExecute(string[] args)
    {
      IList<ProcessInfo> runningAgentInstances = AgentInstances.GetRunningAgentInstances();
      foreach (ProcessInfo processInfo in (IEnumerable<ProcessInfo>) runningAgentInstances)
        this.WriteMessage(processInfo.Id.ToString() + ":" + processInfo.BaseName);
      this.WriteMessage("{0} agent instance(s) running.", (object) runningAgentInstances.Count);
    }
  }
}
