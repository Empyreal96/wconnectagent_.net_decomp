// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.ManifestEnvelope
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal struct ManifestEnvelope
  {
    public const int MaxChunkSize = 65280;
    public ManifestEnvelope.ManifestFormats Format;
    public byte MajorVersion;
    public byte MinorVersion;
    public byte Magic;
    public ushort TotalChunks;
    public ushort ChunkNumber;

    public enum ManifestFormats : byte
    {
      SimpleXmlFormat = 1,
    }
  }
}
