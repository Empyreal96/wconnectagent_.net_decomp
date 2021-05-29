// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandRunIfNecessary
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Threading;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  internal class CommandRunIfNecessary : LauncherCommand
  {
    private const int WaitTimeOutInMs = 40000;

    public override int MinimumArgumentCount => 0;

    protected override void OnExecute(string[] args)
    {
      if (AgentInstances.GetRunningAgentInstances().Count > 0)
        this.WriteMessage("Agent instance already running. Skipped.");
      using (EventWaitHandle eventWaitHandle1 = new EventWaitHandle(false, EventResetMode.ManualReset, "WConnectAgentPlatformBootFailureEvent"))
      {
        using (EventWaitHandle eventWaitHandle2 = new EventWaitHandle(false, EventResetMode.ManualReset, "WConnectAgentBootstrappedEvent"))
        {
          using (EventWaitHandle eventWaitHandle3 = new EventWaitHandle(false, EventResetMode.ManualReset, "WConnectAgentStartFailureEvent"))
          {
            eventWaitHandle1.Reset();
            eventWaitHandle3.Reset();
            eventWaitHandle2.Reset();
            WaitHandle[] waitHandles = new WaitHandle[3]
            {
              (WaitHandle) eventWaitHandle1,
              (WaitHandle) eventWaitHandle3,
              (WaitHandle) eventWaitHandle2
            };
            new CommandRun().Execute(args);
            switch (WaitHandle.WaitAny(waitHandles, 40000))
            {
              case 0:
                throw new CommandException("WConnectAgent signaled that a failure happened while trying to boot the platform.", CommandExceptionReason.AgentPlatformBootFailure);
              case 1:
                throw new CommandException("WConnectAgent encountered a failure while starting.", CommandExceptionReason.AgentStartFailure);
              case 2:
                this.WriteMessage("WConnectAgent was started successfully.");
                break;
              case 258:
                throw new CommandException("Wait for WConnectAgent start timed out.", CommandExceptionReason.AgentWaitTimeout);
              default:
                throw new InvalidOperationException("Unexpected value for index.");
            }
          }
        }
      }
    }
  }
}
