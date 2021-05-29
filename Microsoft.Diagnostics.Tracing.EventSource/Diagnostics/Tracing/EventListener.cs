// Decompiled with JetBrains decompiler
// Type: Microsoft.Diagnostics.Tracing.EventListener
// Assembly: Microsoft.Diagnostics.Tracing.EventSource, Version=1.1.25.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5E38C965-5888-4D33-BC12-2DC76F063032
// Assembly location: .\\AowDebugger\Agent\Microsoft.Diagnostics.Tracing.EventSource.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Diagnostics.Tracing
{
  public abstract class EventListener : IDisposable
  {
    internal volatile EventListener m_Next;
    internal static EventListener s_Listeners;
    internal static List<WeakReference> s_EventSources;
    private static bool s_CreatingListener = false;
    private static bool s_EventSourceShutdownRegistered = false;

    protected EventListener()
    {
      lock (EventListener.EventListenersLock)
      {
        if (EventListener.s_CreatingListener)
          throw new InvalidOperationException(Microsoft.Diagnostics.Tracing.Internal.Environment.GetResourceString("EventSource_ListenerCreatedInsideCallback"));
        try
        {
          EventListener.s_CreatingListener = true;
          this.m_Next = EventListener.s_Listeners;
          EventListener.s_Listeners = this;
          foreach (WeakReference weakReference in EventListener.s_EventSources.ToArray())
          {
            if (weakReference.Target is EventSource target5)
              target5.AddListener(this);
          }
        }
        finally
        {
          EventListener.s_CreatingListener = false;
        }
      }
    }

    public virtual void Dispose()
    {
      lock (EventListener.EventListenersLock)
      {
        if (EventListener.s_Listeners == null)
          return;
        if (this == EventListener.s_Listeners)
        {
          EventListener listeners = EventListener.s_Listeners;
          EventListener.s_Listeners = this.m_Next;
          EventListener.RemoveReferencesToListenerInEventSources(listeners);
        }
        else
        {
          EventListener eventListener = EventListener.s_Listeners;
          EventListener next;
          while (true)
          {
            next = eventListener.m_Next;
            if (next != null)
            {
              if (next != this)
                eventListener = next;
              else
                goto label_7;
            }
            else
              break;
          }
          return;
label_7:
          eventListener.m_Next = next.m_Next;
          EventListener.RemoveReferencesToListenerInEventSources(next);
        }
      }
    }

    public void EnableEvents(EventSource eventSource, EventLevel level) => this.EnableEvents(eventSource, level, EventKeywords.None);

    public void EnableEvents(
      EventSource eventSource,
      EventLevel level,
      EventKeywords matchAnyKeyword)
    {
      this.EnableEvents(eventSource, level, matchAnyKeyword, (IDictionary<string, string>) null);
    }

    public void EnableEvents(
      EventSource eventSource,
      EventLevel level,
      EventKeywords matchAnyKeyword,
      IDictionary<string, string> arguments)
    {
      if (eventSource == null)
        throw new ArgumentNullException(nameof (eventSource));
      eventSource.SendCommand(this, 0, 0, EventCommand.Update, true, level, matchAnyKeyword, arguments);
    }

    public void DisableEvents(EventSource eventSource)
    {
      if (eventSource == null)
        throw new ArgumentNullException(nameof (eventSource));
      eventSource.SendCommand(this, 0, 0, EventCommand.Update, false, EventLevel.LogAlways, EventKeywords.None, (IDictionary<string, string>) null);
    }

    protected internal virtual void OnEventSourceCreated(EventSource eventSource)
    {
    }

    protected internal abstract void OnEventWritten(EventWrittenEventArgs eventData);

    protected static int EventSourceIndex(EventSource eventSource) => eventSource.m_id;

    internal static void AddEventSource(EventSource newEventSource)
    {
      lock (EventListener.EventListenersLock)
      {
        if (EventListener.s_EventSources == null)
          EventListener.s_EventSources = new List<WeakReference>(2);
        if (!EventListener.s_EventSourceShutdownRegistered)
          EventListener.s_EventSourceShutdownRegistered = true;
        int num = -1;
        if (EventListener.s_EventSources.Count % 64 == 63)
        {
          int count = EventListener.s_EventSources.Count;
          while (0 < count)
          {
            --count;
            WeakReference eventSource = EventListener.s_EventSources[count];
            if (!eventSource.IsAlive)
            {
              num = count;
              eventSource.Target = (object) newEventSource;
              break;
            }
          }
        }
        if (num < 0)
        {
          num = EventListener.s_EventSources.Count;
          EventListener.s_EventSources.Add(new WeakReference((object) newEventSource));
        }
        newEventSource.m_id = num;
        for (EventListener listener = EventListener.s_Listeners; listener != null; listener = listener.m_Next)
          newEventSource.AddListener(listener);
      }
    }

    private static void DisposeOnShutdown(object sender, EventArgs e)
    {
      foreach (WeakReference eventSource in EventListener.s_EventSources)
      {
        if (eventSource.Target is EventSource target1)
          target1.Dispose();
      }
    }

    private static void RemoveReferencesToListenerInEventSources(EventListener listenerToRemove)
    {
      using (List<WeakReference>.Enumerator enumerator = EventListener.s_EventSources.GetEnumerator())
      {
label_10:
        while (enumerator.MoveNext())
        {
          if (enumerator.Current.Target is EventSource target3)
          {
            if (target3.m_Dispatchers.m_Listener == listenerToRemove)
            {
              target3.m_Dispatchers = target3.m_Dispatchers.m_Next;
            }
            else
            {
              EventDispatcher eventDispatcher = target3.m_Dispatchers;
              EventDispatcher next;
              while (true)
              {
                next = eventDispatcher.m_Next;
                if (next != null)
                {
                  if (next.m_Listener != listenerToRemove)
                    eventDispatcher = next;
                  else
                    break;
                }
                else
                  goto label_10;
              }
              eventDispatcher.m_Next = next.m_Next;
            }
          }
        }
      }
    }

    [Conditional("DEBUG")]
    internal static void Validate()
    {
      lock (EventListener.EventListenersLock)
      {
        Dictionary<EventListener, bool> dictionary = new Dictionary<EventListener, bool>();
        for (EventListener key = EventListener.s_Listeners; key != null; key = key.m_Next)
          dictionary.Add(key, true);
        int num = -1;
        foreach (WeakReference eventSource in EventListener.s_EventSources)
        {
          ++num;
          if (eventSource.Target is EventSource target4)
          {
            EventDispatcher eventDispatcher1 = target4.m_Dispatchers;
            while (eventDispatcher1 != null)
              eventDispatcher1 = eventDispatcher1.m_Next;
            using (Dictionary<EventListener, bool>.KeyCollection.Enumerator enumerator = dictionary.Keys.GetEnumerator())
            {
label_15:
              while (enumerator.MoveNext())
              {
                EventListener current = enumerator.Current;
                EventDispatcher eventDispatcher2 = target4.m_Dispatchers;
                while (true)
                {
                  if (eventDispatcher2.m_Listener != current)
                    eventDispatcher2 = eventDispatcher2.m_Next;
                  else
                    goto label_15;
                }
              }
            }
          }
        }
      }
    }

    internal static object EventListenersLock
    {
      get
      {
        if (EventListener.s_EventSources == null)
          Interlocked.CompareExchange<List<WeakReference>>(ref EventListener.s_EventSources, new List<WeakReference>(2), (List<WeakReference>) null);
        return (object) EventListener.s_EventSources;
      }
    }
  }
}
