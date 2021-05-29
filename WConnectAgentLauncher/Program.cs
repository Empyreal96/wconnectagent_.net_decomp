// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Program
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  public static class Program
  {
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Acceptable for a utility.", MessageId = "System.Logger.Instance.Log(System.String)")]
    public static int Main(string[] args)
    {
      try
      {
        if (args == null)
          throw new ArgumentNullException(nameof (args));
        Logger.Instance.Log("Microsoft Windows Bridge For Android WConnectAgent Launcher");
        Logger.Instance.Log("(c) Copyright 2014-2015 Microsoft Corporation\n");
        Logger.Instance.Log("SDK Version {0} \n\n", (object) LauncherConfiguration.SDKVersion.ToString());
        PathProvider.BuildPaths();
        return (int) Program.ProcessCommandline(args);
      }
      catch (Exception ex)
      {
        Logger.Instance.Log(ex);
        return -5;
      }
    }

    private static LauncherExitCodes ProcessCommandline(string[] args)
    {
      LauncherCommand commandToExecute = (LauncherCommand) new CommandHelp();
      string[] commandArguments = new string[0];
      if (args.Length > 0)
      {
        LauncherCommand commandFromName = Program.CreateCommandFromName(args[0]);
        if (commandFromName.MinimumArgumentCount > 0 && args.Length - 1 < commandFromName.MinimumArgumentCount)
        {
          Logger.Instance.Log("Invalid number of arguments. Type 'help' to see a list of commands and their usage.");
          return LauncherExitCodes.InvalidArguments;
        }
        commandToExecute = commandFromName;
        commandArguments = new string[args.Length - 1];
        Array.Copy((Array) args, 1, (Array) commandArguments, 0, args.Length - 1);
      }
      LauncherExitCodes launcherExitCodes = Program.RunCommand(commandToExecute, commandArguments);
      switch (commandToExecute)
      {
        case CommandHelp _ when args.Length == 0:
          launcherExitCodes = LauncherExitCodes.NoCommandSpecified;
          break;
        case CommandUnknown _:
          launcherExitCodes = LauncherExitCodes.CommandNotFound;
          break;
      }
      return launcherExitCodes;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design.")]
    private static LauncherExitCodes RunCommand(
      LauncherCommand commandToExecute,
      string[] commandArguments)
    {
      try
      {
        commandToExecute.Execute(commandArguments);
        return LauncherExitCodes.Success;
      }
      catch (CommandException ex)
      {
        Logger.Instance.Log("Launcher Failure: {0} Reason = {1}.", (object) ex.Message, (object) ex.Reason);
        return Program.MapExitCodeFromExceptionReason(ex.Reason);
      }
      catch (Exception ex)
      {
        Logger.Instance.Log("Launcher Failure: " + ex.Message);
        return LauncherExitCodes.CommandException;
      }
    }

    private static LauncherCommand CreateCommandFromName(string commandName)
    {
      Type commandType = (Type) null;
      return Program.BuildCommandHandlers().TryGetValue(commandName, out commandType) ? Program.CreateCommandInstanceFromType(commandType) : (LauncherCommand) new CommandUnknown();
    }

    private static LauncherCommand CreateCommandInstanceFromType(Type commandType) => Activator.CreateInstance(commandType) is LauncherCommand instance ? instance : throw new InvalidOperationException("Could not cast command handler to required interface.");

    private static Dictionary<string, Type> BuildCommandHandlers() => new Dictionary<string, Type>()
    {
      {
        "help",
        typeof (CommandHelp)
      },
      {
        "unpack",
        typeof (CommandUnpack)
      },
      {
        "clear",
        typeof (CommandClear)
      },
      {
        "killAll",
        typeof (CommandKillAll)
      },
      {
        "run",
        typeof (CommandRun)
      },
      {
        "runIfNecessary",
        typeof (CommandRunIfNecessary)
      },
      {
        "instances",
        typeof (CommandInstances)
      }
    };

    private static LauncherExitCodes MapExitCodeFromExceptionReason(
      CommandExceptionReason reason)
    {
      switch (reason)
      {
        case CommandExceptionReason.AgentWaitTimeout:
          return LauncherExitCodes.AgentWaitTimeout;
        case CommandExceptionReason.AgentPlatformBootFailure:
          return LauncherExitCodes.AgentPlatformBootFailure;
        case CommandExceptionReason.AgentStartFailure:
          return LauncherExitCodes.AgentStartFailure;
        default:
          return LauncherExitCodes.Other;
      }
    }
  }
}
