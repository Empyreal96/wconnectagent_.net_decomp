// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.ClassPropertyWriter`2
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal class ClassPropertyWriter<ContainerType, ValueType> : PropertyAccessor<ContainerType>
  {
    private readonly TraceLoggingTypeInfo<ValueType> valueTypeInfo;
    private readonly ClassPropertyWriter<ContainerType, ValueType>.Getter getter;

    public ClassPropertyWriter(PropertyAnalysis property)
    {
      this.valueTypeInfo = (TraceLoggingTypeInfo<ValueType>) property.typeInfo;
      this.getter = (ClassPropertyWriter<ContainerType, ValueType>.Getter) Statics.CreateDelegate(typeof (ClassPropertyWriter<ContainerType, ValueType>.Getter), property.getterInfo);
    }

    public override void Write(TraceLoggingDataCollector collector, ref ContainerType container)
    {
      ValueType valueType = (object) container == null ? default (ValueType) : this.getter(container);
      this.valueTypeInfo.WriteData(collector, ref valueType);
    }

    public override object GetData(ContainerType container) => (object) ((object) container == null ? default (ValueType) : this.getter(container));

    private delegate ValueType Getter(ContainerType container);
  }
}
