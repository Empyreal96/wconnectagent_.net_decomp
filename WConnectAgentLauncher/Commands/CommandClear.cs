// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandClear
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.IO;
using System.Threading;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal class CommandClear : LauncherCommand
  {
    public override int MinimumArgumentCount => 0;

    protected override void OnExecute(string[] args)
    {
      if (!Directory.Exists(PathProvider.AgentDirectoryPath))
      {
        this.WriteMessage("Agent directory already doesn't exist. Skipped.");
      }
      else
      {
        bool retryDelete = AgentInstances.TerminateRunningAgentInstances();
        if (CommandClear.RemoveAgentUnpackDirectory(retryDelete))
        {
          this.WriteMessage("Deleted agent successfully.");
        }
        else
        {
          if (retryDelete)
            throw new InvalidOperationException("Could not delete agent directory. Exhausted retry attempts.");
          throw new InvalidOperationException("Could not delete agent directory.");
        }
      }
    }

    private static bool RemoveAgentUnpackDirectory(bool retryDelete)
    {
      int num = 10;
      bool flag = false;
      while (num > 0)
      {
        try
        {
          if (File.Exists(PathProvider.SuccessMarkerPath))
            File.Delete(PathProvider.SuccessMarkerPath);
          Directory.Delete(PathProvider.AgentDirectoryPath, true);
          flag = true;
          break;
        }
        catch (UnauthorizedAccessException ex)
        {
          if (retryDelete)
          {
            --num;
            Logger.Instance.Log("Cannot delete agent directory as it is in use. Waiting and trying again - {0} remaining attempt(s).", (object) num);
            Thread.Sleep((10 - num) * 100);
          }
          else
            break;
        }
      }
      return flag;
    }
  }
}
