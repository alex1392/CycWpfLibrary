﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using CycWpfLibrary.Media;
using System.Drawing;
using CycWpfLibrary.Emgu;
using Emgu.CV.Structure;
using CycWpfLibrary;
using System;
using CycWpfLibrary.Resources;
using Emgu.CV;

namespace CycWpfLibrary.Tests
{
  [TestClass()]
  public class ImageConvertersTests
  {
    private Bitmap bitmap;

    public ImageConvertersTests()
    {
      //bitmap = new Bitmap(@"C:\Users\alex\Desktop\WPF\CycWpfLibrary\CycWpfLibraryTests\data.png");
      bitmap = new Bitmap(@"C:\Users\alex\Desktop\WPF\CycWpfLibrary\CycWpfLibraryTests\MouseWheel.png");
    }

    [TestMethod()]
    public void ToBitmapTest()
    {
      var bitmapSource = bitmap.ToBitmapSource();
      var bitmap2 = bitmapSource.ToBitmap();
      var image = bitmap.ToImage<Rgba, byte>();
      var image2 = bitmap2.ToImage<Rgba, byte>();
      Assert.IsTrue(image.Bytes.IsEqual(image2.Bytes));
    }
  }
}