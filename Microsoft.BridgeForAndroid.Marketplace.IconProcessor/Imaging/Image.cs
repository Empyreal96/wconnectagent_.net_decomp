// Decompiled with JetBrains decompiler
// Type: Microsoft.Arcadia.Marketplace.IconProcessor.Imaging.Image
// Assembly: Microsoft.BridgeForAndroid.Marketplace.IconProcessor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F2F7352B-0630-411B-B3C3-A48FD0224AA6
// Assembly location: .\\AowDebugger\Agent\Microsoft.BridgeForAndroid.Marketplace.IconProcessor.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Microsoft.Arcadia.Marketplace.IconProcessor.Imaging
{
  public class Image
  {
    private const int BufferPixelSize = 4;
    private const int BufferAlphaChannelOffset = 3;
    private const int BufferBlueChannelOffset = 2;
    private const int BufferRedChannelOffset = 0;
    private const int BufferGreenChannelOffset = 1;
    private const ColorManagementMode ColorManagement = (ColorManagementMode) 1;
    private const BitmapAlphaMode AlphaMode = (BitmapAlphaMode) 1;
    private const ExifOrientationMode OrientationMode = (ExifOrientationMode) 1;
    private const BitmapPixelFormat PixelFormat = (BitmapPixelFormat) 30;

    public Image(int width, int height, double dpiX, double dpiY)
      : this(width, height, dpiX, dpiY, new byte[width * height * 4])
    {
    }

    private Image(int width, int height, double dpiX, double dpiY, byte[] pixelBuffer)
    {
      if (width < 0)
        throw new ArgumentException("width cannot be less than 0.");
      if (height < 0)
        throw new ArgumentException("height cannot be less than 0.");
      if (dpiX < 0.0)
        throw new ArgumentException("dpiX cannot be less than 0.");
      if (dpiY < 0.0)
        throw new ArgumentException("dpiY cannot be less than 0.");
      this.Pixels = pixelBuffer != null ? pixelBuffer : throw new ArgumentNullException(nameof (pixelBuffer));
      this.Width = width;
      this.Height = height;
      this.DpiX = dpiX;
      this.DpiY = dpiY;
    }

    private Image(BitmapDecoder decoder)
    {
      this.Pixels = decoder != null ? Image.LoadPixelBufferAsync(decoder).Result : throw new ArgumentNullException(nameof (decoder));
      this.Width = (int) decoder.OrientedPixelWidth;
      this.Height = (int) decoder.OrientedPixelHeight;
      this.DpiX = decoder.DpiX;
      this.DpiY = decoder.DpiY;
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public double DpiX { get; private set; }

    public double DpiY { get; private set; }

    [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Direct manipulation of underlying buffer is desired.")]
    public byte[] Pixels { get; private set; }

    public static async Task<Image> LoadAsync(string filePath)
    {
      if (filePath == null)
        throw new ArgumentNullException(nameof (filePath));
      StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
      Image image;
      using (IRandomAccessStream fileStream = await file.OpenAsync((FileAccessMode) 0))
      {
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fileStream);
        image = new Image(decoder);
      }
      return image;
    }

    public Image Clone()
    {
      byte[] pixelBuffer = new byte[this.Width * this.Height * 4];
      Array.Copy((Array) this.Pixels, (Array) pixelBuffer, this.Pixels.Length);
      return new Image(this.Width, this.Height, this.DpiX, this.DpiY, pixelBuffer);
    }

    public Color GetPixel(int x, int y)
    {
      if (x < 0 || x > this.Width - 1 || (y < 0 || y > this.Height - 1))
        throw new ArgumentException("The X or Y coordinate is invalid.");
      int index = y * this.Width * 4 + x * 4;
      byte pixel1 = this.Pixels[index + 2];
      byte pixel2 = this.Pixels[index + 1];
      byte pixel3 = this.Pixels[index];
      return Color.FromArgb(this.Pixels[index + 3], pixel3, pixel2, pixel1);
    }

    public void SetPixel(int x, int y, Color color)
    {
      if (x > this.Width - 1)
        throw new ArgumentException("x cannot be greater than the underlying image's width.");
      if (y > this.Height - 1)
        throw new ArgumentException("y cannot be greater than underlying image's height.");
      int index = y * this.Width * 4 + x * 4;
      this.Pixels[index + 3] = color.A;
      this.Pixels[index + 2] = color.B;
      this.Pixels[index + 1] = color.G;
      this.Pixels[index] = color.R;
    }

    public async Task<Image> CropAsync(
      int cropSourceX,
      int cropSourceY,
      int cropSourceWidth,
      int cropSourceHeight)
    {
      if (cropSourceX < 0)
        throw new ArgumentException("crossSourceX cannot be less than 0.");
      if (cropSourceY < 0)
        throw new ArgumentException("crossSourceY cannot be less than 0.");
      if (cropSourceX + cropSourceWidth > this.Width)
        throw new ArgumentException("Width of the cropping must not be greater than the sum of the offset and the underlying image's width.", nameof (cropSourceWidth));
      if (cropSourceY + cropSourceHeight > this.Height)
        throw new ArgumentException("Height of the cropping must not be greater than the sum of the offset and the underlying image's height.", nameof (cropSourceHeight));
      Image image;
      using (InMemoryRandomAccessStream cropperRas = new InMemoryRandomAccessStream())
      {
        BitmapEncoder cropperEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, (IRandomAccessStream) cropperRas);
        cropperEncoder.SetPixelData((BitmapPixelFormat) 30, (BitmapAlphaMode) 1, (uint) this.Width, (uint) this.Height, this.DpiX, this.DpiY, this.Pixels);
        cropperEncoder.BitmapTransform.put_Bounds(new BitmapBounds()
        {
          Height = (__Null) cropSourceHeight,
          Width = (__Null) cropSourceWidth,
          X = (__Null) cropSourceX,
          Y = (__Null) cropSourceY
        });
        await cropperEncoder.FlushAsync();
        BitmapDecoder postCropDecoder = await BitmapDecoder.CreateAsync((IRandomAccessStream) cropperRas);
        image = new Image(postCropDecoder);
      }
      return image;
    }

    public async Task<Image> ResizeAsync(int targetWidth, int targetHeight)
    {
      if (targetWidth < 0)
        throw new ArgumentException("targetWidth must not be less than 0.");
      if (targetHeight < 0)
        throw new ArgumentException("targetHeight must not be less than 0.");
      Image image;
      using (InMemoryRandomAccessStream scalerRas = new InMemoryRandomAccessStream())
      {
        BitmapEncoder scaleEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, (IRandomAccessStream) scalerRas);
        scaleEncoder.SetPixelData((BitmapPixelFormat) 30, (BitmapAlphaMode) 1, (uint) this.Width, (uint) this.Height, this.DpiX, this.DpiY, this.Pixels);
        scaleEncoder.BitmapTransform.put_InterpolationMode((BitmapInterpolationMode) 3);
        scaleEncoder.BitmapTransform.put_ScaledHeight((uint) targetHeight);
        scaleEncoder.BitmapTransform.put_ScaledWidth((uint) targetWidth);
        await scaleEncoder.FlushAsync();
        BitmapDecoder postScaleDecoder = await BitmapDecoder.CreateAsync((IRandomAccessStream) scalerRas);
        image = new Image(postScaleDecoder);
      }
      return image;
    }

    public Image Composite(Image foreground, int offsetLeft, int offsetTop)
    {
      if (foreground == null)
        throw new ArgumentNullException(nameof (foreground));
      if (foreground.Width + offsetLeft > this.Width)
        throw new ArgumentException("Superimposed image overflows the background image's width with specified left offset.", nameof (offsetLeft));
      if (foreground.Height + offsetTop > this.Height)
        throw new ArgumentException("Superimposed image overflows the background image's height with specified top offset.", nameof (offsetTop));
      Image image = this.Clone();
      for (int x1 = 0; x1 < foreground.Width; ++x1)
      {
        for (int y1 = 0; y1 < foreground.Height; ++y1)
        {
          int x2 = offsetLeft + x1;
          int y2 = offsetTop + y1;
          Color color = ColorUtils.BlendColor(image.GetPixel(x2, y2), foreground.GetPixel(x1, y1));
          image.SetPixel(x2, y2, color);
        }
      }
      return image;
    }

    public void FillRectangle(Color brushColor, int x, int y, int width, int height)
    {
      if (x + width > this.Width)
        throw new ArgumentException("width cannot be larger than the underlying image width.", nameof (width));
      if (y + height > this.Height)
        throw new ArgumentException("height cannot be larger than the underlying image height.", nameof (height));
      for (int x1 = x; x1 < x + width; ++x1)
      {
        for (int y1 = y; y1 < y + height; ++y1)
          this.SetPixel(x1, y1, brushColor);
      }
    }

    public async Task SaveAsPngAsync(string filePath)
    {
      if (filePath == null)
        throw new ArgumentNullException(nameof (filePath));
      StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(filePath));
      StorageFile file = await folder.CreateFileAsync(Path.GetFileName(filePath), (CreationCollisionOption) 1);
      using (IRandomAccessStream fileStream = await file.OpenAsync((FileAccessMode) 1))
      {
        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
        encoder.SetPixelData((BitmapPixelFormat) 30, (BitmapAlphaMode) 1, (uint) this.Width, (uint) this.Height, this.DpiX, this.DpiY, this.Pixels);
        await encoder.FlushAsync();
      }
    }

    private static async Task<byte[]> LoadPixelBufferAsync(BitmapDecoder decoder)
    {
      PixelDataProvider dataProvider = await decoder.GetPixelDataAsync((BitmapPixelFormat) 30, (BitmapAlphaMode) 1, new BitmapTransform(), (ExifOrientationMode) 1, (ColorManagementMode) 1);
      return dataProvider.DetachPixelData();
    }
  }
}
