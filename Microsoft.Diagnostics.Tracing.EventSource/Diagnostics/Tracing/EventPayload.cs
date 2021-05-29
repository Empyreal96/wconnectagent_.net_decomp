// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventPayload
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Diagnostics.Tracing
{
  internal class EventPayload : 
    IDictionary<string, object>,
    ICollection<KeyValuePair<string, object>>,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    private List<string> m_names;
    private List<object> m_values;

    internal EventPayload(List<string> payloadNames, List<object> payloadValues)
    {
      this.m_names = payloadNames;
      this.m_values = payloadValues;
    }

    public ICollection<string> Keys => (ICollection<string>) this.m_names;

    public ICollection<object> Values => (ICollection<object>) this.m_values;

    public object this[string key]
    {
      get
      {
        if (key == null)
          throw new ArgumentNullException(nameof (key));
        int index = 0;
        foreach (string name in this.m_names)
        {
          if (name == key)
            return this.m_values[index];
          ++index;
        }
        throw new KeyNotFoundException();
      }
      set => throw new NotSupportedException();
    }

    public void Add(string key, object value) => throw new NotSupportedException();

    public void Add(KeyValuePair<string, object> payloadEntry) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(KeyValuePair<string, object> entry) => this.ContainsKey(entry.Key);

    public bool ContainsKey(string key)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      foreach (string name in this.m_names)
      {
        if (name == key)
          return true;
      }
      return false;
    }

    public int Count => this.m_names.Count;

    public bool IsReadOnly => true;

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public void CopyTo(KeyValuePair<string, object>[] payloadEntries, int count) => throw new NotSupportedException();

    public bool Remove(string key) => throw new NotSupportedException();

    public bool Remove(KeyValuePair<string, object> entry) => throw new NotSupportedException();

    public bool TryGetValue(string key, out object value)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      int index = 0;
      foreach (string name in this.m_names)
      {
        if (name == key)
        {
          value = this.m_values[index];
          return true;
        }
        ++index;
      }
      value = (object) null;
      return false;
    }
  }
}
