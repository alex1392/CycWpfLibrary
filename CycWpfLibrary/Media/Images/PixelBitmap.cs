﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using Colors = System.Windows.Media.Colors;
using DependencyObject = System.Windows.DependencyObject;
using Grid = System.Windows.Controls.Grid;
using Image = System.Windows.Controls.Image;
using SizeWinForm = System.Drawing.Size;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using Window = System.Windows.Window;

namespace CycWpfLibrary
{
  public class PixelBitmap : DependencyObject, ICloneable, IEquatable<PixelBitmap>
  {
    public readonly PixelFormat PixelFormat = PixelFormat.Format32bppArgb;
    public readonly int Depth = 4; //根據Format32bppArgb
    private readonly object _key = new object();

    public int Width { get; private set; }
    public int Height { get; private set; }
    public SizeWinForm Size { get; private set; }
    public int Stride { get; private set; }  //根據Format32bppArgb

    private Bitmap _bitmap;
    /// <summary>
    /// 自動設定<see cref="Width"/>, <see cref="Height"/>, <see cref="Size"/>, <see cref="Stride"/>等欄位。
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private Bitmap _Bitmap
    {
      get => _bitmap;
      set
      {
        _bitmap = value;
        Width = _Bitmap.Width;
        Height = _Bitmap.Height;
        Size = _Bitmap.Size;
        Stride = _Bitmap.Width * Depth;
      }
    }
    /// <summary>
    /// Bitmap物件不可同時被多執行緒使用，需使用互斥鎖，getter回傳副本。
    /// </summary>
    public Bitmap Bitmap
    {
      get
      {
        Bitmap bitmap;
        lock (_key)
        {
          bitmap = _Bitmap.Clone() as Bitmap;
        }
        return bitmap;
      }
      set
      {
        lock (_key)
        {
          _Bitmap = value;
          BitmapToPixel();
        }
      }
    }
    private byte[] _pixel;
    public byte[] Pixel
    {
      get => _pixel;
      set
      {
        _pixel = value;
        PixelToBitmap();
      }
    }

    // Constructors
    public PixelBitmap()
    {

    }
    public PixelBitmap(Bitmap bitmap)
    {
      // 注意! 24bpp轉成32bpp時，alpha值會被覆寫成255
      if (bitmap.PixelFormat != PixelFormat)
        bitmap = bitmap.SetPixelFormat(PixelFormat);
      Bitmap = new Bitmap(bitmap); //更新pixel
    }
    public PixelBitmap(SizeWinForm sizeWinForm)
    {
      _Bitmap = new Bitmap(sizeWinForm.Width, sizeWinForm.Height, PixelFormat); //不更新pixel
    }
    public PixelBitmap(byte[] pixel, SizeWinForm size)
    {
      _Bitmap = new Bitmap(size.Width, size.Height, PixelFormat); //不更新pixel
      Pixel = pixel; //更新bitmap
    }
    public object Clone()
    {
      if (_Bitmap == null)
      {
        return new PixelBitmap();
      }
      else
      {
        var pixelbitmap = new PixelBitmap();
        pixelbitmap._Bitmap = Bitmap;
        pixelbitmap._pixel = _pixel.Clone() as byte[];
        return pixelbitmap;
      }
    } //比 new 一個物件快十倍

    // public methods
    public void Show()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        var window = new Window
        {
          Background = new SolidColorBrush(Colors.LightGray),
        };
        var grid = new Grid
        {
          Background = null,
        };
        var image = new Image();
        window.Content = grid;
        grid.Children.Add(image);
        image.Source = this.ToBitmapSource();
        window.ShowDialog();

      });
    }

    /// <summary>
    /// 取得圖像的三維陣列，像素深度按A,R,G,B排列
    /// </summary>
    public byte[,,] GetPixel3Argb()
    {
      var pixel = _pixel.Clone() as byte[];
      var pixel3 = new byte[Width, Height, Depth];
      int idx;
      for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
        {
          idx = x * Depth + y * Stride;
          pixel3[x, y, 3] = pixel[idx + 0]; //B
          pixel3[x, y, 2] = pixel[idx + 1]; //G
          pixel3[x, y, 1] = pixel[idx + 2]; //R
          pixel3[x, y, 0] = pixel[idx + 3]; //A
        }
      return pixel3;
    }

    // private methods
    private void PixelToBitmap()
    {
      //將image鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的imageData
      var bitmapData = _bitmap.LockBits(
      new Rectangle(0, 0, Width, Height),
      ImageLockMode.ReadOnly,
      PixelFormat);

      //複製pixel到bitmapData中
      Marshal.Copy(_pixel, 0, bitmapData.Scan0, _pixel.Length);

      //解鎖
      _bitmap.UnlockBits(bitmapData);
    }
    private void BitmapToPixel()
    {
      //將image鎖定到系統內的記憶體的某個區塊中，並將這個結果交給BitmapData類別的imageData
      var bitmapData = _bitmap.LockBits(
        new Rectangle(0, 0, Width, Height),
        ImageLockMode.ReadOnly,
        PixelFormat);

      //初始化pixel陣列，用來儲存所有像素的訊息
      _pixel = new byte[bitmapData.Stride * Height];

      //複製imageData的RGB信息到pixel陣列中
      Marshal.Copy(bitmapData.Scan0, _pixel, 0, _pixel.Length);

      //解鎖
      _bitmap.UnlockBits(bitmapData); //其他地方正在使用物件.....

    }

    public bool Equals(PixelBitmap other) => Pixel.IsEqual(other.Pixel);
  }
}
