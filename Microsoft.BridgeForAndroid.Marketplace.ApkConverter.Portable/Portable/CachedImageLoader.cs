// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.Converter.Portable.CachedImageLoader
// Assembly: Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 705177B0-BC5D-4AC6-AF21-50FBFD0416B4
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.ApkConverter.Portable.dll

using Microsoft.Arcadia.Marketplace.IconProcessor.Imaging;
using Microsoft.Arcadia.Marketplace.Utils.Log;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Microsoft.Arcadia.Marketplace.Converter.Portable
{
  public class CachedImageLoader
  {
    private ConcurrentDictionary<string, Image> imageCache = new ConcurrentDictionary<string, Image>();

    public async Task<Image> LoadImageAsync(string filePath)
    {
      string lowerFilePath = !string.IsNullOrWhiteSpace(filePath) ? filePath.ToLower() : throw new ArgumentException("Path must be valid.", nameof (filePath));
      Image loadedImage = (Image) null;
      if (this.imageCache.TryGetValue(lowerFilePath, out loadedImage))
      {
        LoggerCore.Log(LoggerCore.LogLevels.Info, "Returning {0} from image cache.", (object) lowerFilePath);
      }
      else
      {
        loadedImage = await Image.LoadAsync(lowerFilePath);
        LoggerCore.Log(LoggerCore.LogLevels.Info, "Adding {0} to image cache.", (object) lowerFilePath);
        this.imageCache.AddOrUpdate(lowerFilePath, loadedImage, (Func<string, Image, Image>) ((key, oldValue) => loadedImage));
      }
      return loadedImage;
    }

    public void ClearCache() => this.imageCache.Clear();
  }
}
