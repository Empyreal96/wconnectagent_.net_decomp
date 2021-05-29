// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EnumerableTypeInfo`2
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class EnumerableTypeInfo<IterableType, ElementType> : 
    TraceLoggingTypeInfo<IterableType>
    where IterableType : IEnumerable<ElementType>
  {
    private readonly TraceLoggingTypeInfo<ElementType> elementInfo;

    public EnumerableTypeInfo(TraceLoggingTypeInfo<ElementType> elementInfo) => this.elementInfo = elementInfo;

    public override void WriteMetadata(
      TraceLoggingMetadataCollector collector,
      string name,
      EventFieldFormat format)
    {
      collector.BeginBufferedArray();
      this.elementInfo.WriteMetadata(collector, name, format);
      collector.EndBufferedArray();
    }

    public override void WriteData(TraceLoggingDataCollector collector, ref IterableType value)
    {
      int bookmark = collector.BeginBufferedArray();
      int count = 0;
      if ((object) value != null)
      {
        foreach (ElementType elementType in value)
        {
          this.elementInfo.WriteData(collector, ref elementType);
          ++count;
        }
      }
      collector.EndBufferedArray(bookmark, count);
    }

    public override object GetData(object value)
    {
      IterableType iterableType = (IterableType) value;
      List<object> objectList = new List<object>();
      foreach (ElementType elementType in iterableType)
        objectList.Add(this.elementInfo.GetData((object) elementType));
      return (object) objectList.ToArray();
    }
  }
}
