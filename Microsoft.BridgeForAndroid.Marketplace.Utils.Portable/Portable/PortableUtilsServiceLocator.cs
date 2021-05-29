// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Utils.Portable.PortableUtilsServiceLocator
// Assembly: Microsoft.BridgeForAndroid.Marketplace.Utils.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E3FB3D6-22B1-4B07-A618-479FD1819BFC
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.Utils.Portable.dll

using Microsoft.Arcadia.Marketplace.Utils.Interfaces.Portable;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
  public static class PortableUtilsServiceLocator
  {
    private static volatile bool isInit = false;
    private static object initLock = new object();

    public static IPortableFileUtils FileUtils { get; private set; }

    public static IPortableResourceUtils ResourceUtils { get; private set; }

    public static IProcessRunnerFactory ProcessRunnerFactory { get; private set; }

    public static bool Initialized
    {
      get => PortableUtilsServiceLocator.isInit;
      private set => PortableUtilsServiceLocator.isInit = value;
    }

    public static void Initialize(
      IPortableFileUtils fileUtils,
      IPortableResourceUtils resourceUtils,
      IProcessRunnerFactory runnerFactory)
    {
      lock (PortableUtilsServiceLocator.initLock)
      {
        if (PortableUtilsServiceLocator.Initialized)
          throw new UtilsException("Portable utilities already initialized!");
        PortableUtilsServiceLocator.FileUtils = fileUtils;
        PortableUtilsServiceLocator.ResourceUtils = resourceUtils;
        PortableUtilsServiceLocator.ProcessRunnerFactory = runnerFactory;
        PortableUtilsServiceLocator.Initialized = true;
      }
    }
  }
}
