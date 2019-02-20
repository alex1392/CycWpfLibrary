﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColorWinForm = System.Drawing.Color;
using PixelFormatsWpf = System.Windows.Media.PixelFormats;
using PixelFormatWinForm = System.Drawing.Imaging.PixelFormat;
using PointWinForm = System.Drawing.Point;
using PointWpf = System.Windows.Point;
using SizeWinForm = System.Drawing.Size;
using SizeWpf = System.Windows.Size;

namespace CycWpfLibrary
{
  #region Helper classes
  internal struct IconInfo
  {
    public bool fIcon;
    public int xHotspot;
    public int yHotspot;
    public IntPtr hbmMask;
    public IntPtr hbmColor;
  }
  internal class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
  {
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyIcon([In] IntPtr hIcon);
    private SafeIconHandle()
        : base(true)
    {
    }
    public SafeIconHandle(IntPtr hIcon)
        : base(true)
    {
      this.SetHandle(hIcon);
    }
    protected override bool ReleaseHandle()
    {
      return DestroyIcon(this.handle);
    }
  }
  #endregion

  public static class ImageExtensions
  {
    #region Converters
    // Uri
    public static BitmapImage ToBitmapImage(this Uri uri)
    {
      return new BitmapImage(uri);
    }

    public static Bitmap ToBitmap(this Uri uri)
    {
      return uri.ToBitmapImage().ToBitmap();
    }

    public static BitmapSource ToBitmapSource(this Uri uri)
    {
      return uri.ToBitmapImage().ToBitmap().ToBitmapSource();
    }

    public static PixelBitmap ToPixelBitmap(this Uri uri)
    {
      return uri.ToBitmapImage().ToPixelBitmap();
    }

    public static Cursor ToCursor(this Uri uri)
    {
      return new Cursor(Application.GetResourceStream(uri).Stream);
    }

    // BitmapImage
    public static Bitmap ToBitmap(this BitmapImage bitmapImage)
    {
      using (MemoryStream outStream = new MemoryStream())
      {
        var enc = new PngBitmapEncoder(); // 使用PngEncoder才不會流失透明度
        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        enc.Save(outStream);
        return new Bitmap(outStream);
      }
    }

    // BitmapSource
    [Obsolete]
    public static Bitmap ToBitmapOld(this BitmapSource bitmapsource)
    {
      //convert image format
      var src = new FormatConvertedBitmap();
      src.BeginInit();
      src.Source = bitmapsource;
      src.DestinationFormat = PixelFormatsWpf.Bgra32;
      src.EndInit();

      //copy to bitmap
      Bitmap bitmap = new Bitmap(src.PixelWidth, src.PixelHeight, PixelFormatWinForm.Format32bppArgb);
      var data = bitmap.LockBits(new Rectangle(PointWinForm.Empty, bitmap.Size), ImageLockMode.WriteOnly, PixelFormatWinForm.Format32bppArgb);
      src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
      bitmap.UnlockBits(data);

      return bitmap;
    }

    public static Bitmap ToBitmap(this BitmapSource bitmapSource)
    {
      var bitmap = new Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, PixelFormatWinForm.Format32bppPArgb);
      var data = bitmap.LockBits(new Rectangle(PointWinForm.Empty, bitmap.Size), ImageLockMode.WriteOnly, PixelFormatWinForm.Format32bppPArgb);
      bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
      bitmap.UnlockBits(data);
      return bitmap;
    }

    // Bitmap
    public static BitmapImage ToBitmapImage(this Bitmap bitmap)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        bitmap.Save(stream, ImageFormat.Png);

        stream.Position = 0;
        BitmapImage result = new BitmapImage();
        result.BeginInit();
        result.CacheOption = BitmapCacheOption.OnLoad;
        result.StreamSource = stream;
        result.EndInit();
        result.Freeze();
        return result;
      }
    }
    public static BitmapSource ToBitmapSource(this Bitmap bitmap)
    {
      IntPtr hBitmap = bitmap.GetHbitmap();
      BitmapSource retval;

      try
      {
        retval = Imaging.CreateBitmapSourceFromHBitmap(
                     hBitmap,
                     IntPtr.Zero,
                     Int32Rect.Empty,
                     BitmapSizeOptions.FromEmptyOptions());
      }
      finally
      {
        DeleteObject(hBitmap);
      }

      return retval;
    }
    public static PixelBitmap ToPixelBitmap(this Bitmap bitmap)
    {
      return new PixelBitmap(bitmap);
    }
    public static Cursor ToCursor(this Bitmap bitmap, PointWpf hotSpot)
    {
      /// get icon from input bitmap
      IconInfo ico = new IconInfo();
      GetIconInfo(bitmap.GetHicon(), ref ico);

      /// set the hotspot
      ico.xHotspot = (int)(hotSpot.X * bitmap.Width);
      ico.yHotspot = (int)(hotSpot.Y * bitmap.Height);
      ico.fIcon = false;

      /// create a cursor from iconinfo
      IntPtr cursor = CreateIconIndirect(ref ico);
      return CursorInteropHelper.Create(new SafeIconHandle(cursor));
    }
    public static Cursor ToCursor(this FrameworkElement element, PointWpf hotSpot)
    {// size changed? color changed?
      var width = element.Width;
      var height = element.Height;
      element.Arrange(new Rect(new SizeWpf(width, height)));

      // Render to a bitmapSource
      var bitmapSource = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormatsWpf.Pbgra32);
      bitmapSource.Render(element);

      var bitmap = bitmapSource.ToBitmap();
      return bitmap.ToCursor(hotSpot);
    }

    // Not straight forward
    public static BitmapSource ToBitmapSource(this PixelBitmap pixelBitmap) => pixelBitmap.Bitmap.ToBitmapSource();
    public static PixelBitmap ToPixelBitmap(this BitmapImage bitmapImage) => bitmapImage.ToBitmap().ToPixelBitmap();
    public static PixelBitmap ToPixelBitmap(this BitmapSource bitmapSource) => new PixelBitmap(bitmapSource.ToBitmap());
    #endregion

    #region Modifications
    public static Bitmap Crop(this Bitmap bitmap, Rectangle rect)
    {
      var output = new Bitmap(rect.Width, rect.Height);
      using (Graphics g = Graphics.FromImage(output))
        g.DrawImage(bitmap, -rect.X, -rect.Y);
      return output;
    }

    public static Bitmap Crop(this Bitmap bitmap, Rect rect)
    {
      return bitmap.Crop(new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
    }

    public static Bitmap SetPixelFormat(this Bitmap bitmap, PixelFormatWinForm pixelFormat)
    {
      var bitmapFormatted = new Bitmap(bitmap.Width, bitmap.Height, pixelFormat);
      using (Graphics g = Graphics.FromImage(bitmapFormatted))
      {
        g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
      }
      return bitmapFormatted;
    }
    #endregion

    #region Helper methods
    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);
    [DllImport("user32.dll")]
    private static extern IntPtr CreateIconIndirect(ref IconInfo piconinfo);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo piconinfo);
    #endregion
  }
}
