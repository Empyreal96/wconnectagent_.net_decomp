// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.NullableTypeInfo`1
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class NullableTypeInfo<T> : TraceLoggingTypeInfo<T?> where T : struct
  {
    private readonly TraceLoggingTypeInfo<T> valueInfo;

    public NullableTypeInfo(List<Type> recursionCheck) => this.valueInfo = TraceLoggingTypeInfo<T>.GetInstance(recursionCheck);

    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      TraceLoggingMetadataCollector collector1 = collector.AddGroup(name);
      collector1.AddScalar("HasValue", TraceLoggingDataType.Boolean8);
      this.valueInfo.WriteMetadata(collector1, "Value", format);
    }

    public override void WriteData(TraceLoggingDataCollector collector, ref T? value)
    {
      bool hasValue = value.HasValue;
      collector.AddScalar(hasValue);
      T obj = hasValue ? value.Value : default (T);
      this.valueInfo.WriteData(collector, ref obj);
    }
  }
}
