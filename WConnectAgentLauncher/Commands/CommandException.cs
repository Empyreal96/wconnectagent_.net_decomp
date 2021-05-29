// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands.CommandException
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Commands
{
  [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Exception will never be serialized.")]
  public class CommandException : Exception
  {
    public CommandException()
      : base("Failure executing command.")
    {
    }

    public CommandException(string message)
      : base(message)
    {
    }

    public CommandException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public CommandException(CommandExceptionReason reason)
      : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failure executing command. Reason = {0}.", (object) reason.ToString()))
    {
      this.Reason = reason;
    }

    public CommandException(string message, CommandExceptionReason reason)
      : base(message)
    {
      this.Reason = reason;
    }

    protected CommandException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public CommandExceptionReason Reason { get; private set; }
  }
}
