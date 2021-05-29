// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Program
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using Microsoft.Arcadia.Debugging.AdbEngine.Portable;
using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  public static class Program
  {
    private const int ExportedAgentPort = 50505;
    private const string HelpCommand = "help";
    private const string LocalAdbAddress = "127.0.0.1";
    private const string AgentBindAddress = "127.0.0.1";
    private const int LocalAdbPort = 5555;
    private const int SessionIdMaxLength = 36;
    private static EventWaitHandle platformBootFailureEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "WConnectAgentPlatformBootFailureEvent");
    private static EventWaitHandle agentStartFailureEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "WConnectAgentStartFailureEvent");

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Console output is for internal debug only, no need to localize", MessageId = "System.Console.WriteLine(System.String)")]
    public static void Main(string[] args)
    {
      bool flag1 = false;
      try
      {
        if (args == null)
          throw new ArgumentNullException(nameof (args));
        if (args.Length == 1 && args[0] == "help")
        {
          Program.ShowUsage();
        }
        else
        {
          using (Factory factory = new Factory())
          {
            using (EtwEventStreamProvider eventStreamProvider = new EtwEventStreamProvider())
            {
              PortableUtilsServiceLocator.Initialize(factory.CreatePortableFileUtils(), factory.CreatePortableResourceUtils(), factory.CreateProcessRunnerFactory());
              EtwLogger.Initialize((IEtwEventStreamProvider) eventStreamProvider);
              LoggerCore.RegisterFormatter((ILogMessageFormatter) new SimpleLogMessageFormatter());
              LoggerCore.AddLogProvider((LogProvider) new EtwLogProvider());
              flag1 = true;
              string adbdHost = "127.0.0.1";
              string localBindAddress = "127.0.0.1";
              int adbdPort = 5555;
              int exportedPort = 50505;
              bool flag2 = false;
              for (int index = 0; index < args.Length; ++index)
              {
                string strA = args[index];
                if (string.Compare(strA, "-v", StringComparison.OrdinalIgnoreCase) == 0)
                {
                  if (!flag2)
                  {
                    ConsoleLog consoleLog = new ConsoleLog();
                    consoleLog.LogLevels = LoggerCore.LogLevels.All;
                    LoggerCore.AddLogProvider((LogProvider) consoleLog);
                    flag2 = true;
                  }
                }
                else
                {
                  int num;
                  if (string.Compare(strA, "-i", StringComparison.OrdinalIgnoreCase) == 0)
                  {
                    if (index + 1 >= args.Length)
                    {
                      Program.ShowUsage();
                      return;
                    }
                    string appxFilePath = args[num = index + 1];
                    AppxLaunchDeployUtil.InstallAppx((IFactory) factory, appxFilePath);
                    return;
                  }
                  if (string.Compare(strA, "-l", StringComparison.OrdinalIgnoreCase) == 0)
                  {
                    if (index + 1 >= args.Length)
                    {
                      Program.ShowUsage();
                      return;
                    }
                    string uri = args[num = index + 1];
                    AppxLaunchDeployUtil.LaunchUri((IFactory) factory, uri);
                    return;
                  }
                  if (string.Compare(strA, "-adbd", StringComparison.OrdinalIgnoreCase) == 0)
                  {
                    if (index + 1 >= args.Length)
                    {
                      Program.ShowUsage();
                      return;
                    }
                    adbdHost = args[++index];
                  }
                  else if (string.Compare(strA, "-bindAll", StringComparison.OrdinalIgnoreCase) == 0)
                    localBindAddress = (string) null;
                  else if (string.Compare(strA, "-sessionId", StringComparison.OrdinalIgnoreCase) == 0)
                  {
                    if (index + 1 >= args.Length)
                    {
                      Program.ShowUsage();
                      return;
                    }
                    Program.ProcessSessionIdentifier(args[++index]);
                  }
                }
              }
              Program.StartAgent(factory, localBindAddress, adbdHost, adbdPort, exportedPort);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Program.agentStartFailureEvent.Set();
        if (flag1)
        {
          LoggerCore.Log(ex);
        }
        else
        {
          Console.WriteLine(ex.Message);
          throw;
        }
      }
    }

    private static void ProcessSessionIdentifier(string sessionIdentifier)
    {
      if (!string.IsNullOrWhiteSpace(sessionIdentifier))
      {
        if (sessionIdentifier.Length <= 36)
          EtwLogger.Instance.SessionIdentifier = sessionIdentifier;
        else
          EtwLogger.Instance.LogWarning("Ignoring session identifier parameter as it's too long.");
      }
      else
        EtwLogger.Instance.LogWarning("Ignoring session identifier parameter as it's null or empty.");
    }

    private static void ShowUsage()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("Usage: ~.exe <ADBD host> [-v | -b |-h]");
      stringBuilder.AppendLine("       ~.exe -i <APPXToInstall>");
      stringBuilder.AppendLine("       ~.exe -l <URIToLaunch>\n\n");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("Options:");
      stringBuilder.AppendLine("       -v  Enables verbose logging.");
      stringBuilder.AppendLine("       -bindAll  When specified, binds the agent to all IP addresses available to the device.");
      stringBuilder.AppendLine("       -adbd  The host address of ADBD which the agent shall connect to. By default, this is 127.0.0.1");
      stringBuilder.AppendLine("       -sessionId  Specifies the unique ID of the session. This is used for telemetry purposes. ");
      Console.Write(stringBuilder.ToString());
    }

    private static void StartAgent(
      Factory factory,
      string localBindAddress,
      string adbdHost,
      int adbdPort,
      int exportedPort)
    {
      string str = localBindAddress == null ? "All" : localBindAddress;
      Console.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ADBD Host: {0}", (object) adbdHost));
      Console.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ADBD Port: {0}", (object) adbdPort));
      Console.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Binded IP Address: {0}", (object) str));
      Console.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exported Port: {0}", (object) exportedPort));
      Console.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Session Identifier: {0}", (object) EtwLogger.Instance.SessionIdentifier));
      LoggerCore.Log("Starting WConnectAgent");
      try
      {
        new AppxFileSystemCleanUpService((IFactory) factory).RemoveStaleAppxLayout();
        IOUtils.RemoveDirectory(factory.AgentConfiguration.LocalDataSniffedDirectory);
        Program.StartAow(factory.AowInstance);
        using (AndroidDebugBridgeAgent debugBridgeAgent = new AndroidDebugBridgeAgent((IFactory) factory, (IAdbTrafficMonitor) null, new InternetEndPoint(adbdHost, adbdPort), new InternetEndPoint(localBindAddress, exportedPort), factory.AppxPackageType))
        {
          using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            debugBridgeAgent.RunAsync(cancellationTokenSource.Token).Wait();
        }
      }
      catch (Exception ex)
      {
        if (ex.InnerException is AdbEngineSocketConnectException)
          EtwLogger.Instance.AdbDaemonConnectFailure();
        LoggerCore.Log(ex);
        throw;
      }
    }

    private static void StartAow(IAowInstanceWrapper instance)
    {
      try
      {
        instance.StartAow();
        LoggerCore.Log("AOW instance started");
      }
      catch (ProjectAStartFailureException ex)
      {
        Program.platformBootFailureEvent.Set();
        throw;
      }
    }
  }
}
