// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Log.LoggerCore
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Arcadia.Marketplace.Utils.Log
{
  public static class LoggerCore
  {
    private static readonly object SyncObject = new object();
    private static List<LogProvider> logProviders;
    private static ILogMessageFormatter formatter;

    public static LogProvider GetLogProvider(Type providerType)
    {
      lock (LoggerCore.SyncObject)
      {
        if (LoggerCore.logProviders != null)
        {
          foreach (LogProvider logProvider in LoggerCore.logProviders)
          {
            if ((object) logProvider.GetType() == (object) providerType)
              return logProvider;
          }
        }
      }
      return (LogProvider) null;
    }

    public static void Log(
      LoggerCore.LogLevels logLevel,
      string message,
      IMessageArg[] messageArgs)
    {
      LoggerCore.DoLog(logLevel, message, messageArgs);
    }

    public static void Log(LoggerCore.LogLevels logLevel, string message, IMessageArg messageArg) => LoggerCore.DoLog(logLevel, message, new IMessageArg[1]
    {
      messageArg
    });

    public static void Log(LoggerCore.LogLevels logLevel, string format, params object[] args) => LoggerCore.DoLog(logLevel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args), (IMessageArg[]) null);

    public static void Log(LoggerCore.LogLevels logLevel, string message) => LoggerCore.DoLog(logLevel, message, (IMessageArg[]) null);

    public static void Log(string message, IMessageArg[] messageArgs) => LoggerCore.DoLog(LoggerCore.LogLevels.Debug, message, messageArgs);

    public static void Log(string message, IMessageArg messageArgs) => LoggerCore.DoLog(LoggerCore.LogLevels.Debug, message, new IMessageArg[1]
    {
      messageArgs
    });

    public static void Log(string message) => LoggerCore.DoLog(LoggerCore.LogLevels.Debug, message, (IMessageArg[]) null);

    public static void Log(string format, params object[] args) => LoggerCore.DoLog(LoggerCore.LogLevels.Debug, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args), (IMessageArg[]) null);

    public static void Log(Exception exp)
    {
      if (exp == null)
        return;
      LoggerCore.DoLog(LoggerCore.LogLevels.Exp, LoggerCore.formatter.GetExceptionMessage(exp), new IMessageArg[1]
      {
        (IMessageArg) new ExpMessageArg(exp)
      });
    }

    public static void Log(LoggerCore.LogLevels logLevel, Exception exp) => LoggerCore.DoLog(logLevel, string.Empty, new IMessageArg[1]
    {
      (IMessageArg) new ExpMessageArg(exp)
    });

    public static void Log() => LoggerCore.DoLog(LoggerCore.LogLevels.Debug, string.Empty, (IMessageArg[]) null);

    public static void RemoveLogProvider(LogProvider logProvider)
    {
      if (logProvider == null)
        throw new ArgumentNullException(nameof (logProvider));
      lock (LoggerCore.SyncObject)
      {
        if (LoggerCore.logProviders == null)
          return;
        LoggerCore.logProviders.Remove(logProvider);
        logProvider.DeinitLog();
      }
    }

    public static void RemoveLogProvider(Type providerType) => LoggerCore.RemoveLogProvider(LoggerCore.GetLogProvider(providerType));

    public static void AddLogProvider(LogProvider logProvider)
    {
      if (logProvider == null)
        throw new ArgumentNullException(nameof (logProvider));
      lock (LoggerCore.SyncObject)
      {
        if (LoggerCore.logProviders == null)
          LoggerCore.logProviders = new List<LogProvider>();
        if (LoggerCore.GetLogProvider(logProvider.GetType()) != null)
          return;
        logProvider.InitLog();
        LoggerCore.logProviders.Add(logProvider);
      }
    }

    public static void RegisterFormatter(ILogMessageFormatter newFormatter)
    {
      if (newFormatter == null)
        throw new ArgumentNullException(nameof (newFormatter));
      lock (LoggerCore.SyncObject)
      {
        if (newFormatter == null)
          return;
        LoggerCore.formatter = newFormatter;
      }
    }

    public static void Deinit() => LoggerCore.RemoveProviders();

    private static void ProvidersLog(ILogMessage logMessage)
    {
      if (LoggerCore.logProviders == null)
        return;
      foreach (LogProvider logProvider in LoggerCore.logProviders)
      {
        if ((logProvider.LogLevels & logMessage.LogLevel) != LoggerCore.LogLevels.None)
          logProvider.Log(logMessage);
      }
    }

    private static bool CanSkipThisLogLevel(LoggerCore.LogLevels logLevel)
    {
      if (LoggerCore.logProviders != null)
      {
        foreach (LogProvider logProvider in LoggerCore.logProviders)
        {
          if ((logProvider.LogLevels & logLevel) != LoggerCore.LogLevels.None)
            return false;
        }
      }
      return true;
    }

    private static void DoLog(
      LoggerCore.LogLevels logLevel,
      string message,
      IMessageArg[] messageArgs)
    {
      lock (LoggerCore.SyncObject)
      {
        if (LoggerCore.CanSkipThisLogLevel(logLevel))
          return;
        LoggerCore.ProvidersLog(LoggerCore.formatter.CreateMessage(logLevel, message, messageArgs));
      }
    }

    private static void RemoveProviders()
    {
      lock (LoggerCore.SyncObject)
      {
        if (LoggerCore.logProviders != null)
        {
          foreach (LogProvider logProvider in LoggerCore.logProviders)
            logProvider.DeinitLog();
          LoggerCore.logProviders.Clear();
        }
        LoggerCore.logProviders = (List<LogProvider>) null;
      }
    }

    [Flags]
    public enum LogDecorations
    {
      None = 0,
      Detail = 1,
      Time = 2,
      LogLevel = 4,
      All = 255, // 0x000000FF
    }

    [Flags]
    public enum LogLevels
    {
      None = 0,
      Debug = 1,
      Info = 2,
      Warning = 4,
      Error = 8,
      Exp = 16, // 0x00000010
      ExpStack = 32, // 0x00000020
      All = 255, // 0x000000FF
    }
  }
}
