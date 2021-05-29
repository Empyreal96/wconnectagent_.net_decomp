// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbEngine.Portable.WorkScheduler
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 977E322A-C0F9-4D9A-A9E1-F4418E87D0AB
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Engine.Portable.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Debugging.AdbEngine.Portable
{
  public class WorkScheduler : IDisposable
  {
    private object lockObject = new object();
    private IList<IWork> works = (IList<IWork>) new List<IWork>();
    private AutoResetEvent worksChangeEvent = new AutoResetEvent(false);
    private ManualResetEvent stopEvent = new ManualResetEvent(false);

    public void AddWorks(params IWork[] workList)
    {
      lock (this.lockObject)
      {
        this.AppendWorks(workList);
        this.worksChangeEvent.Set();
      }
    }

    public void AssignWorks(params IWork[] workList)
    {
      lock (this.lockObject)
      {
        this.works.Clear();
        this.AppendWorks(workList);
        this.worksChangeEvent.Set();
      }
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
      if (cancellationToken.IsCancellationRequested)
        return;
      using (cancellationToken.Register((Action) (() => this.stopEvent.Set())))
        await Task.Factory.StartNew((Action) (() => this.Run()), TaskCreationOptions.LongRunning);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.worksChangeEvent.Dispose();
      this.stopEvent.Dispose();
    }

    private void AppendWorks(params IWork[] workList)
    {
      if (workList == null || workList.Length <= 0)
        return;
      foreach (IWork work in workList)
      {
        if (work == null)
          throw new ArgumentException("Input should not contain null object", nameof (workList));
        this.works.Add(work);
      }
    }

    private void Run()
    {
label_0:
      WaitHandle waitHandle;
      do
      {
        IList<WaitHandle> source = (IList<WaitHandle>) new List<WaitHandle>();
        source.Add((WaitHandle) this.worksChangeEvent);
        source.Add((WaitHandle) this.stopEvent);
        lock (this.lockObject)
        {
          foreach (IWork work in (IEnumerable<IWork>) this.works)
            source.Add(work.SignalHandle);
        }
        int index;
        try
        {
          index = WaitHandle.WaitAny(source.ToArray<WaitHandle>());
        }
        catch (ObjectDisposedException ex)
        {
          return;
        }
        waitHandle = source[index];
        if (waitHandle == this.stopEvent)
          return;
      }
      while (waitHandle == this.worksChangeEvent);
      IList<IWork> workList = (IList<IWork>) new List<IWork>();
      lock (this.lockObject)
      {
        foreach (IWork work in (IEnumerable<IWork>) this.works)
        {
          if (work.SignalHandle == waitHandle)
            workList.Add(work);
        }
      }
      using (IEnumerator<IWork> enumerator = workList.GetEnumerator())
      {
        while (enumerator.MoveNext())
          enumerator.Current.DoWork();
        goto label_0;
      }
    }
  }
}
