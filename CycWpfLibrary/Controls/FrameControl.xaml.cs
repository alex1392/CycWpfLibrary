﻿using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CycWpfLibrary.Controls
{
  /// <summary>
  /// FrameControl.xaml 的互動邏輯
  /// </summary>
  public partial class FrameControl : UserControl
  {
    public FrameControl()
    {
      InitializeComponent();

      gridMain.DataContext = this;
      
    }

    public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register(nameof(PageManager), typeof(IPageManager), typeof(FrameControl));
    public IPageManager PageManager
    {
      get { return (IPageManager)GetValue(CurrentPageProperty); }
      set { SetValue(CurrentPageProperty, value); }
    }
  }
}
