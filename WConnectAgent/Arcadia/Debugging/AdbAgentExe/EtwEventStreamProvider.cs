// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.EtwEventStreamProvider
// Assembly: WConnectAgent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998BA8DE-78E1-437C-9EB7-7699DDCFCAB7
// Assembly location: .\\AowDebugger\Agent\WConnectAgent.exe

using Microsoft.Arcadia.Debugging.SharedUtils.Portable;
using Microsoft.Diagnostics.Telemetry;
using Microsoft.Diagnostics.Tracing;
using System;
using System.Text;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe
{
  [EventSource(Guid = "18f2739c-2213-59b4-2c5e-aa8e1a270f2f", Name = "Microsoft.Windows.AoW.ADB.DebugAgent")]
  internal class EtwEventStreamProvider : TelemetryEventSource, IEtwEventStreamProvider
  {
    private static EventSourceOptions debugVerboseOption = new EventSourceOptions()
    {
      Level = EventLevel.Verbose
    };
    private static EventSourceOptions telemetryErrorOption = new EventSourceOptions()
    {
      Level = EventLevel.Error,
      Keywords = (EventKeywords) 140737488355328
    };
    private static EventSourceOptions telemetryInfoOption = new EventSourceOptions()
    {
      Level = EventLevel.Informational,
      Keywords = (EventKeywords) 70368744177664
    };
    private static EventSourceOptions telemetryWarningOption = new EventSourceOptions()
    {
      Level = EventLevel.Warning,
      Keywords = (EventKeywords) 35184372088832
    };

    public string SessionIdentifier { get; set; }

    public void Startup()
    {
      if (!this.IsEnabled())
        return;
      this.Write("StartUp", EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Diagnostic",
        SessionId = this.SessionIdentifier
      });
    }

    public void AdbDaemonConnected()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AdbDaemonConnected), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        SessionId = this.SessionIdentifier
      });
    }

    public void AdbDaemonConnectFailure()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AdbDaemonConnectFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Network",
        SessionId = this.SessionIdentifier
      });
    }

    public void ListeningForAdbServer()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ListeningForAdbServer), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        SessionId = this.SessionIdentifier
      });
    }

    public void AdbServerAccepted()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AdbServerAccepted), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        SessionId = this.SessionIdentifier
      });
    }

    public void SocketDataSendReceiveError(string socketIdentifier, string reason)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (SocketDataSendReceiveError), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Network",
        socketIdentifier = socketIdentifier,
        reason = reason,
        SessionId = this.SessionIdentifier
      });
    }

    public void ForcefullyClosedDaemonSocket()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ForcefullyClosedDaemonSocket), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        SessionId = this.SessionIdentifier
      });
    }

    public void ForcefullyClosedServerSocket()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ForcefullyClosedServerSocket), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        SessionId = this.SessionIdentifier
      });
    }

    public void StartingApkInstall(string correlationId, string apkFileName)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (StartingApkInstall), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "PackageManager",
        correlationId = correlationId,
        apkFileName = apkFileName,
        SessionId = this.SessionIdentifier
      });
    }

    public void StartingApkSync(string correlationId, string apkFileName)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (StartingApkSync), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        correlationId = correlationId,
        apkFileName = apkFileName,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkSyncSuccess(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkSyncSuccess), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkSyncFailure(string correlationId, string reasonForFailure)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkSyncFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Network",
        correlationId = correlationId,
        reasonForFailure = reasonForFailure,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkManifestDecoding(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkManifestDecoding), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkManifestDecoded(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkManifestDecoded), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkResourcesDecoding(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkResourcesDecoding), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkResourcesDecoded(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkResourcesDecoded), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkConverting(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkConverting), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkConverted(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkConverted), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkConversionFailure(string correlationId, string reasonForFailure)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkConversionFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Conversion",
        correlationId = correlationId,
        reasonForFailure = reasonForFailure,
        SessionId = this.SessionIdentifier
      });
    }

    public void ApkManifestInfo(string manifestInfoPackageName, string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ApkManifestInfo), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Conversion",
        PackageName = manifestInfoPackageName,
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void StartAppxInstall(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (StartAppxInstall), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "PackageManager",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void AppxInstalled(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AppxInstalled), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "PackageManager",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void AppxInstallFailure(string correlationId, string reasonForFailure)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AppxInstallFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "PackageManager",
        correlationId = correlationId,
        reasonForFailure = reasonForFailure,
        SessionId = this.SessionIdentifier
      });
    }

    public void AppxInstallReattempt()
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AppxInstallReattempt), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "PackageManager",
        SessionId = this.SessionIdentifier
      });
    }

    public void AppxUninstalled(string correlationId)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AppxUninstalled), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "PackageManager",
        correlationId = correlationId,
        SessionId = this.SessionIdentifier
      });
    }

    public void AppxUninstallFailure(string correlationId, string reasonForFailure)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (AppxUninstallFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "PackageManager",
        correlationId = correlationId,
        reasonForFailure = reasonForFailure,
        SessionId = this.SessionIdentifier
      });
    }

    public void OpenedDaemonChannel(uint localId, string channelName)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (OpenedDaemonChannel), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Network",
        localId = localId,
        channelName = channelName,
        SessionId = this.SessionIdentifier
      });
    }

    public void OpenDaemonChannelFailure(uint originalLocalId, string channelName)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (OpenDaemonChannelFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Network",
        originalLocalId = originalLocalId,
        channelName = channelName,
        SessionId = this.SessionIdentifier
      });
    }

    public void TooManyPendingChannelJobs()
    {
      if (!this.IsEnabled())
        return;
      this.Write("TooManyChannelJobRequests", EtwEventStreamProvider.telemetryWarningOption, new
      {
        Cat = "PackageManager",
        SessionId = this.SessionIdentifier
      });
    }

    public void AdbComandSent(uint command, uint arg0, uint arg1, byte[] data)
    {
      if (!this.IsEnabled() || command == 1163154007U || command == 1497451343U)
        return;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (data != null)
        empty1 = Encoding.UTF8.GetString(data);
      if (command != 0U)
        empty2 = Encoding.UTF8.GetString(BitConverter.GetBytes(command));
      this.Write(nameof (AdbComandSent), EtwEventStreamProvider.telemetryInfoOption, new
      {
        Cat = "Commands",
        Command = empty2,
        arg0 = arg0,
        arg1 = arg1,
        data = empty1,
        SessionId = this.SessionIdentifier
      });
    }

    public void LogInfo(string message)
    {
      if (!this.IsEnabled())
        return;
      this.Write("Message", EtwEventStreamProvider.telemetryInfoOption, new
      {
        message = message,
        SessionId = this.SessionIdentifier
      });
    }

    public void ProjectAStartFailure(string reasonForFailure)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (ProjectAStartFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Platform",
        reason = reasonForFailure,
        SessionId = this.SessionIdentifier
      });
    }

    public void DaemonStartFailure(string reasonForFailure)
    {
      if (!this.IsEnabled())
        return;
      this.Write(nameof (DaemonStartFailure), EtwEventStreamProvider.telemetryErrorOption, new
      {
        Cat = "Platform",
        reason = reasonForFailure,
        SessionId = this.SessionIdentifier
      });
    }

    public void LogMessage(string message)
    {
      if (!this.IsEnabled())
        return;
      this.Write("Message", EtwEventStreamProvider.debugVerboseOption, new
      {
        message = message,
        SessionId = this.SessionIdentifier
      });
    }

    public void LogError(string errorMessage)
    {
      if (!this.IsEnabled())
        return;
      this.Write("Error", EtwEventStreamProvider.telemetryErrorOption, new
      {
        errorMessage = errorMessage,
        SessionId = this.SessionIdentifier
      });
    }

    public void LogWarning(string warning)
    {
      if (!this.IsEnabled())
        return;
      this.Write("Warning", EtwEventStreamProvider.telemetryWarningOption, new
      {
        warning = warning,
        SessionId = this.SessionIdentifier
      });
    }

    public class EventCategory
    {
      public const string Diagnostic = "Diagnostic";
      public const string Network = "Network";
      public const string Conversion = "Conversion";
      public const string PackageManager = "PackageManager";
      public const string Commands = "Commands";
      public const string Platform = "Platform";
    }

    public class Events
    {
      public const string StartUp = "StartUp";
      public const string AdbDaemonConnected = "AdbDaemonConnected";
      public const string AdbDaemonConnectFailure = "AdbDaemonConnectFailure";
      public const string ListeningForAdbServer = "ListeningForAdbServer";
      public const string AdbServerAccepted = "AdbServerAccepted";
      public const string ForcefullyClosedDaemonSocket = "ForcefullyClosedDaemonSocket";
      public const string ForcefullyClosedServerSocket = "ForcefullyClosedServerSocket";
      public const string SocketDataSendReceiveError = "SocketDataSendReceiveError";
      public const string StartingApkInstall = "StartingApkInstall";
      public const string StartingApkSync = "StartingApkSync";
      public const string ApkSyncSuccess = "ApkSyncSuccess";
      public const string ApkSyncFailure = "ApkSyncFailure";
      public const string ApkManifestDecoding = "ApkManifestDecoding";
      public const string ApkManifestDecoded = "ApkManifestDecoded";
      public const string ApkResourcesDecoding = "ApkResourcesDecoding";
      public const string ApkResourcesDecoded = "ApkResourcesDecoded";
      public const string ApkConverting = "ApkConverting";
      public const string ApkConverted = "ApkConverted";
      public const string ApkConversionFailure = "ApkConversionFailure";
      public const string ApkManifestInfo = "ApkManifestInfo";
      public const string StartAppxInstall = "StartAppxInstall";
      public const string AppxInstalled = "AppxInstalled";
      public const string AppxInstallFailure = "AppxInstallFailure";
      public const string AppxInstallReattempt = "AppxInstallReattempt";
      public const string AppxUninstalled = "AppxUninstalled";
      public const string AppxUninstallFailure = "AppxUninstallFailure";
      public const string OpenedDaemonChannel = "OpenedDaemonChannel";
      public const string OpenDaemonChannelFailure = "OpenDaemonChannelFailure";
      public const string DaemonStartFailure = "DaemonStartFailure";
      public const string ProjectAStartFailure = "ProjectAStartFailure";
      public const string AdbComandSent = "AdbComandSent";
      public const string LogMessage = "Message";
      public const string LogError = "Error";
      public const string LogWarning = "Warning";
      public const string TooManyChannelJobRequests = "TooManyChannelJobRequests";
    }
  }
}
