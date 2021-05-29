// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.NameInfo
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Diagnostics.Tracing
{
  internal sealed class NameInfo : ConcurrentSetItem<KeyValuePair<string, EventTags>, NameInfo>
  {
    private static int lastIdentity = 184549376;
    internal readonly string name;
    internal readonly EventTags tags;
    internal readonly int identity;
    internal readonly byte[] nameMetadata;

    internal static void ReserveEventIDsBelow(int eventId)
    {
      int lastIdentity;
      int num;
      do
      {
        lastIdentity = NameInfo.lastIdentity;
        num = Math.Max((NameInfo.lastIdentity & -16777216) + eventId, lastIdentity);
      }
      while (Interlocked.CompareExchange(ref NameInfo.lastIdentity, num, lastIdentity) != lastIdentity);
    }

    public NameInfo(string name, EventTags tags, int typeMetadataSize)
    {
      this.name = name;
      this.tags = tags & (EventTags) 268435455;
      this.identity = Interlocked.Increment(ref NameInfo.lastIdentity);
      int pos1 = 0;
      Statics.EncodeTags((int) this.tags, ref pos1, (byte[]) null);
      this.nameMetadata = Statics.MetadataForString(name, pos1, 0, typeMetadataSize);
      int pos2 = 2;
      Statics.EncodeTags((int) this.tags, ref pos2, this.nameMetadata);
    }

    public override int Compare(NameInfo other) => this.Compare(other.name, other.tags);

    public override int Compare(KeyValuePair<string, EventTags> key) => this.Compare(key.Key, key.Value & (EventTags) 268435455);

    private int Compare(string otherName, EventTags otherTags)
    {
      int num = StringComparer.Ordinal.Compare(this.name, otherName);
      if (num == 0 && this.tags != otherTags)
        num = this.tags < otherTags ? -1 : 1;
      return num;
    }
  }
}
