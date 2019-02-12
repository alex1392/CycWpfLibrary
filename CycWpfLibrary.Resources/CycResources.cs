﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CycWpfLibrary.Resources
{
  public class CycResources
  {
    public CycResources() { } // for xaml initialization

    /// <summary>
    /// Usage: <see cref="PackUri"/> + "AssemblyName;component/fileName"
    /// </summary>
    public static string PackUri { get; } = @"pack://application:,,,/";

    /// <summary>
    /// Usage: <see cref="CurrentPackUri"/> + "filePath/fileName"
    /// </summary>
    public static string CurrentPackUri => PackUri + Assembly.GetEntryAssembly().GetName().Name + @";component/";

    public static string CycResourcesPackUri { get; } = PackUri + @"CycWpfLibrary.Resources;component/";

    public static Cursor PanCursor { get; } = new Cursor(Application.GetResourceStream(new Uri(CycResourcesPackUri + "pan.cur", UriKind.Absolute)).Stream);

    public static DrawingBrush CrossboardBrush { get; } = new DrawingBrush
    {
      TileMode = TileMode.Tile,
      Viewport = new Rect(0, 0, 32, 32),
      ViewportUnits = BrushMappingMode.Absolute,
      Drawing = new GeometryDrawing
      {
        Brush = Brushes.LightGray,
        Geometry = Geometry.Parse("M0,0 H16 V16 H32 V32 H16 V16 H0Z"),
      }
    };

    public static SolidColorBrush ShadowBrush { get; } = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.3), 0, 0, 0));

    public static SolidColorBrush DarkShadowBrush { get; } = new SolidColorBrush(Color.FromArgb((byte)(255 * 0.7), 0, 0, 0));

    public static Uri MouseLeftButtonUri { get; } = new Uri(CycResourcesPackUri + "MouseLeftButton.png");
    public static Uri MouseRightButtonUri { get; } = new Uri(CycResourcesPackUri + "MouseRightButton.png");
    public static Uri MouseWheelUri { get; } = new Uri(CycResourcesPackUri + "MouseWheel.png");

    /// <summary>
    /// Usage: Prepare "app/lang/default.xaml" and "app/lang/YOURLANGUAGE.xaml" files, and call this method in <see cref="Application.OnStartup(StartupEventArgs)"/>
    /// </summary>
    public static void LoadLanguage()
    {
      CultureInfo currentInfo = CultureInfo.CurrentCulture;

      ResourceDictionary rd = null;

      try
      {
#if DEBUG
        rd = Application.LoadComponent(new Uri(@"app/lang/" + "zh-TW" + ".xaml", UriKind.Relative)) as ResourceDictionary;
#else
        if (currentInfo.Name.Contains("zh"))
        {
          rd = Application.LoadComponent(new Uri(@"app/lang/" + "zh-TW" + ".xaml", UriKind.Relative)) as ResourceDictionary;
        }
#endif
      }
      catch { }

      if (rd != null)
      {
        var md = Application.Current.Resources.MergedDictionaries;
        if (md.Count > 0)
        {
          foreach (var d in md)
          {
            var uri = new Uri(CurrentPackUri + @"app/lang/default.xaml", UriKind.Absolute);
            if (d.Source == uri)
            {
              d.Clear();
              break;
            }
          }
        }
        md.Add(rd);
      }
    }
  }
}
