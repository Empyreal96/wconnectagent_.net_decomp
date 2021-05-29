// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Debugging.AdbAgent.Mobile.UriLauncherMobile
// Assembly: Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A3725DA-8D01-4D08-90D4-BC0331796EE5
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Debugging.WConnectAgent.Mobile.dll

using Microsoft.Arcadia.Debugging.AdbAgent.Portable;
using Microsoft.Arcadia.Debugging.AdbAgent.Portable.Exceptions;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace Microsoft.Arcadia.Debugging.AdbAgent.Mobile
{
  public class UriLauncherMobile : IUriLauncher
  {
    public async Task LaunchUri(Uri uri)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException(nameof (uri));
      try
      {
        bool launchResult = await Launcher.LaunchUriAsync(uri);
        if (!launchResult)
          throw new LauncherUriException();
      }
      catch (LauncherUriException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new LauncherUriException(ex);
      }
    }
  }
}
