﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using CycWpfLibrary.MVVM;

namespace CycWpfLibrary.FluentDesign
{
  public class RevealWindow : CustomWindowBase
  {
    static RevealWindow()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(RevealWindow), new FrameworkPropertyMetadata(typeof(RevealWindow)));
    }
  }
}
