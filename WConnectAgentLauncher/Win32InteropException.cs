// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher.Win32InteropException
// Assembly: WConnectAgentLauncher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B260B2AB-026F-473C-B2C4-D0FC46C431CE
// Assembly location: C:\Users\Empyreal96\Documents\repos\AowDebugger\WConnectAgentLauncher.exe

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Arcadia.Debugging.AdbAgentExe.Launcher
{
  [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Type will never be serialized.")]
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Type will never be serialized.")]
  public class Win32InteropException : Exception
  {
    public Win32InteropException(int errorCode)
      : base("A Win32 error has occurred. Error Code =" + errorCode.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    {
      this.HResult = errorCode;
    }

    public Win32InteropException(string errorMessage, int errorCode)
      : base(errorMessage)
    {
    }

    public Win32InteropException(string errorMessage)
      : base(errorMessage)
    {
    }
  }
}
