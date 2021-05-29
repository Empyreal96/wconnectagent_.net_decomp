// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Portable.PathSanitizer
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E4E9B837-7759-4706-BEEB-88C279158D9B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Portable;
using System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Portable
{
  internal class PathSanitizer
  {
    public PathSanitizer(string folder) => this.Path = folder != null ? PortableUtilsServiceLocator.FileUtils.GetFullPath(folder.ToLower()) : throw new ArgumentNullException(nameof (folder));

    public string Path { get; private set; }

    public bool IsWithinFolder(string candidatePath)
    {
      candidatePath = candidatePath != null ? candidatePath.ToLower() : throw new ArgumentNullException(nameof (candidatePath));
      return this.Path.StartsWith(candidatePath, StringComparison.OrdinalIgnoreCase);
    }
  }
}
