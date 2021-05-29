// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.ByteArrayTypeInfo
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class ByteArrayTypeInfo : TraceLoggingTypeInfo<byte[]>
  {
    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      switch (format)
      {
        case EventFieldFormat.String:
          collector.AddBinary(name, TraceLoggingDataType.CountedMbcsString);
          break;
        case EventFieldFormat.Boolean:
          collector.AddArray(name, TraceLoggingDataType.Boolean8);
          break;
        case EventFieldFormat.Hexadecimal:
          collector.AddArray(name, TraceLoggingDataType.HexInt8);
          break;
        case EventFieldFormat.Xml:
          collector.AddBinary(name, TraceLoggingDataType.CountedMbcsXml);
          break;
        case EventFieldFormat.Json:
          collector.AddBinary(name, TraceLoggingDataType.CountedMbcsJson);
          break;
        default:
          collector.AddBinary(name, Statics.MakeDataType(TraceLoggingDataType.Binary, format));
          break;
      }
    }

    public override void WriteData(TraceLoggingDataCollector collector, ref byte[] value) => collector.AddBinary(value);
  }
}
