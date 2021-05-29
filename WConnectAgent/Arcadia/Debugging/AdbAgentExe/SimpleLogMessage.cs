// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.SimpleLogMessage
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  public class SimpleLogMessage : ILogMessage
  {
    private string logMessage;
    private IMessageArg[] logMessageArgs;

    public SimpleLogMessage(LoggerCore.LogLevels level, string message, IMessageArg[] messageArgs)
    {
      if (message == null)
        message = string.Empty;
      this.LogLevel = level;
      this.logMessage = message;
      this.logMessageArgs = messageArgs;
    }

    public LoggerCore.LogLevels LogLevel { get; private set; }

    public string GetLogMessage(
      LoggerCore.LogDecorations logDecoration,
      LoggerCore.LogLevels logLevel)
    {
      StringBuilder stringBuilder = new StringBuilder(this.logMessage.Length);
      stringBuilder.Insert(0, this.logMessage);
      if (logLevel.HasFlag((Enum) LoggerCore.LogLevels.ExpStack) && this.logMessageArgs != null && this.logMessageArgs[0] is ExpMessageArg)
        stringBuilder.Append(Environment.NewLine + this.GetExceptionStackLog());
      return stringBuilder.ToString();
    }

    private string GetExceptionStackLog()
    {
      if (this.logMessageArgs == null || this.logMessageArgs.Length != 1 || !(this.logMessageArgs[0] is ExpMessageArg))
        return string.Empty;
      Exception exception = (this.logMessageArgs[0] as ExpMessageArg).MessageArgument as Exception;
      if (exception is AggregateException)
        exception = exception.InnerException;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\n@\n{1}", (object) exception.Message, (object) exception.StackTrace);
    }
  }
}
