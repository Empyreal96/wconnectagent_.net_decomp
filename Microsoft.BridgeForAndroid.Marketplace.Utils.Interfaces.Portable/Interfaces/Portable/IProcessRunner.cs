// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable.IProcessRunner
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Interfaces.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 941436A9-99F1-46B2-9422-2A9545611EFD
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Interfaces.Portable.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable
{
  public interface IProcessRunner : IDisposable
  {
    int? ExitCode { get; }

    bool RunAndWait(int milliseconds);

    string ExePath { get; set; }

    string Arguments { get; set; }

    bool SupportsStandardOutputRedirection { get; }

    bool SupportsStandardErrorRedirection { get; }

    IReadOnlyList<string> StandardError { get; }

    IReadOnlyList<string> StandardOutput { get; }

    Encoding StandardOutputEncoding { get; set; }

    Encoding StandardErrorEncoding { get; set; }
  }
}
