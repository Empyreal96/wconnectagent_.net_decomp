// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.SharedUtils.Portable.EtwLogger
// Assembly: Microsoft.BridgeForAndroid.Debugging.SharedUtils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3109BA47-CAD2-4316-A501-803E769B457B
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.SharedUtils.Portable.dll

using System;

namespace Microsoft.Arcadia.Debugging.SharedUtils.Portable
{
  public sealed class EtwLogger
  {
    private static IEtwEventStreamProvider instance;

    private EtwLogger()
    {
    }

    public static IEtwEventStreamProvider Instance => EtwLogger.instance;

    public static void Initialize(IEtwEventStreamProvider stream) => EtwLogger.instance = stream != null ? stream : throw new ArgumentNullException(nameof (stream));
  }
}
