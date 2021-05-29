// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.StructPropertyWriter`2
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal class StructPropertyWriter<ContainerType, ValueType> : PropertyAccessor<ContainerType>
  {
    private readonly TraceLoggingTypeInfo<ValueType> valueTypeInfo;
    private readonly StructPropertyWriter<ContainerType, ValueType>.Getter getter;

    public StructPropertyWriter(PropertyAnalysis property)
    {
      this.valueTypeInfo = (TraceLoggingTypeInfo<ValueType>) property.typeInfo;
      this.getter = (StructPropertyWriter<ContainerType, ValueType>.Getter) Statics.CreateDelegate(typeof (StructPropertyWriter<ContainerType, ValueType>.Getter), property.getterInfo);
    }

    public override void Write(TraceLoggingDataCollector collector, ref ContainerType container)
    {
      ValueType valueType = (object) container == null ? default (ValueType) : this.getter(ref container);
      this.valueTypeInfo.WriteData(collector, ref valueType);
    }

    public override object GetData(ContainerType container) => (object) ((object) container == null ? default (ValueType) : this.getter(ref container));

    private delegate ValueType Getter(ref ContainerType container);
  }
}
