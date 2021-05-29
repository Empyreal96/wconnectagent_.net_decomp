// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Logger
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  internal abstract class Logger
  {
    private static Logger instance = (Logger) new ConsoleLogger();

    public static Logger Instance => Logger.instance;

    public void Log(string logMessage) => this.OnLogMessage(logMessage);

    public void Log(Exception exp)
    {
      if (exp == null)
        return;
      this.OnLogMessage(Logger.BuildExceptionMessage(exp));
    }

    public void Log(string format, params object[] formatParams) => this.OnLogMessage(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, formatParams));

    protected abstract void OnLogMessage(string logMessage);

    private static string BuildExceptionMessage(Exception exp)
    {
      if (exp == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(exp.Message);
      stringBuilder.AppendLine(exp.StackTrace);
      if (exp.InnerException != null)
      {
        stringBuilder.AppendLine("\n\nInner Exception:");
        stringBuilder.AppendLine(Logger.BuildExceptionMessage(exp.InnerException));
      }
      return stringBuilder.ToString();
    }
  }
}
