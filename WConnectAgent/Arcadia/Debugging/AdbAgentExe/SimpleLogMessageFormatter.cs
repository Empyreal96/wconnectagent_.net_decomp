// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.SimpleLogMessageFormatter
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  public class SimpleLogMessageFormatter : ILogMessageFormatter
  {
    public ILogMessage CreateMessage(
      LoggerCore.LogLevels logLevel,
      string message,
      IMessageArg[] messageArgs)
    {
      return (ILogMessage) new SimpleLogMessage(logLevel, message, messageArgs);
    }

    public string GetExceptionMessage(Exception exception) => string.Empty;
  }
}
