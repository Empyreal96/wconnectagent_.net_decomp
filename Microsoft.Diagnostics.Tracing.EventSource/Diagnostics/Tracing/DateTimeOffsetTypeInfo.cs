// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.DateTimeOffsetTypeInfo
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class DateTimeOffsetTypeInfo : TraceLoggingTypeInfo<DateTimeOffset>
  {
    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      TraceLoggingMetadataCollector metadataCollector = collector.AddGroup(name);
      metadataCollector.AddScalar("Ticks", Statics.MakeDataType(TraceLoggingDataType.FileTime, format));
      metadataCollector.AddScalar("Offset", TraceLoggingDataType.Int64);
    }

    public override void WriteData(TraceLoggingDataCollector collector, ref DateTimeOffset value)
    {
      long ticks = value.Ticks;
      collector.AddScalar(ticks < 504911232000000000L ? 0L : ticks - 504911232000000000L);
      collector.AddScalar(value.Offset.Ticks);
    }
  }
}
