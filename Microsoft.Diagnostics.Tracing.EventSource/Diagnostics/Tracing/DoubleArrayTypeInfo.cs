// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.DoubleArrayTypeInfo
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class DoubleArrayTypeInfo : TraceLoggingTypeInfo<double[]>
  {
    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      collector.AddArray(name, Statics.Format64(format, TraceLoggingDataType.Double));
    }

    public override void WriteData(TraceLoggingDataCollector collector, ref double[] value) => collector.AddArray(value);
  }
}
