// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable.WorkLogMap
// Assembly: Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C5DFC509-F9BD-4763-8503-317A9C276F67
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.PackageObjectModel.Portable.dll

using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Portable
{
  public sealed class WorkLogMap
  {
    private static WorkLogMap instance;
    private static object instanceLock = new object();
    private ConcurrentDictionary<WorkerLogKey, LogAnnotationAttribute> logMap;

    public static WorkLogMap Instance
    {
      get
      {
        lock (WorkLogMap.instanceLock)
        {
          if (WorkLogMap.instance == null)
            WorkLogMap.instance = new WorkLogMap();
          return WorkLogMap.instance;
        }
      }
    }

    private WorkLogMap()
    {
      this.logMap = new ConcurrentDictionary<WorkerLogKey, LogAnnotationAttribute>();
      Type type = typeof (WorkerLogKey);
      foreach (WorkerLogKey key in Enum.GetValues(type))
      {
        LogAnnotationAttribute customAttribute = type.GetRuntimeField(key.ToString()).GetCustomAttribute<LogAnnotationAttribute>();
        if (customAttribute != null)
        {
          customAttribute.EnumName = key.ToString();
          this.logMap.TryAdd(key, customAttribute);
        }
      }
    }

    public LogAnnotationAttribute GetLogData(WorkerLogKey logKey) => this.logMap[logKey];
  }
}
