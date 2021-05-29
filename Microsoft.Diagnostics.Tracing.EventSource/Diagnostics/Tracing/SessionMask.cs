// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.SessionMask
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

namespace Microsoft.Diagnostics.Tracing
{
  internal struct SessionMask
  {
    internal const int SHIFT_SESSION_TO_KEYWORD = 44;
    internal const uint MASK = 15;
    internal const uint MAX = 4;
    private uint m_mask;

    public SessionMask(SessionMask m) => this.m_mask = m.m_mask;

    public SessionMask(uint mask = 0) => this.m_mask = mask & 15U;

    public bool IsEqualOrSupersetOf(SessionMask m) => ((int) this.m_mask | (int) m.m_mask) == (int) this.m_mask;

    public static SessionMask All => new SessionMask(15U);

    public static SessionMask FromId(int perEventSourceSessionId) => new SessionMask((uint) (1 << perEventSourceSessionId));

    public ulong ToEventKeywords() => (ulong) this.m_mask << 44;

    public static SessionMask FromEventKeywords(ulong m) => new SessionMask((uint) (m >> 44));

    public bool this[int perEventSourceSessionId]
    {
      get => ((long) this.m_mask & (long) (1 << perEventSourceSessionId)) != 0L;
      set
      {
        if (value)
          this.m_mask |= (uint) (1 << perEventSourceSessionId);
        else
          this.m_mask &= (uint) ~(1 << perEventSourceSessionId);
      }
    }

    public static SessionMask operator |(SessionMask m1, SessionMask m2) => new SessionMask(m1.m_mask | m2.m_mask);

    public static SessionMask operator &(SessionMask m1, SessionMask m2) => new SessionMask(m1.m_mask & m2.m_mask);

    public static SessionMask operator ^(SessionMask m1, SessionMask m2) => new SessionMask(m1.m_mask ^ m2.m_mask);

    public static SessionMask operator ~(SessionMask m) => new SessionMask((uint) (15 & ~(int) m.m_mask));

    public static explicit operator ulong(SessionMask m) => (ulong) m.m_mask;

    public static explicit operator uint(SessionMask m) => m.m_mask;
  }
}
