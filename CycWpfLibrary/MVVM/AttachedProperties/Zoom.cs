﻿using CycWpfLibrary.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static System.Math;


namespace CycWpfLibrary.MVVM
{
  /// <summary>
  /// 提供縮放相依屬性的靜態類別。
  /// </summary>
  public static class Zoom
  {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
      "IsEnabled",
      typeof(bool),
      typeof(Zoom),
      new PropertyMetadata(OnIsEnabledChanged));
    [AttachedPropertyBrowsableForType(typeof(UIElement))]
    public static bool GetIsEnabled(UIElement element)
      => (bool)element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(UIElement element, bool value)
      => element.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is FrameworkElement element))
        throw new NotSupportedException($"Can only set the {IsEnabledProperty} attached behavior on a UIElement.");

      if ((bool)e.NewValue)
      {
        element.PreviewMouseWheel += Element_PreviewMouseWheel;
        element.MouseLeave += Element_MouseLeave;
        element.EnsureTransforms();
        element.RenderTransformOrigin = new Point(0, 0);
        element.Parent.SetValue(UIElement.ClipToBoundsProperty, true);
      }
      else
      {
        element.PreviewMouseWheel -= Element_PreviewMouseWheel;
        element.MouseLeave -= Element_MouseLeave;
      }
    }

    private static double LeaveTime = 1;
    private static void Element_MouseLeave(object sender, MouseEventArgs e)
    {
      var element = sender as UIElement;
      var transforms = (element.RenderTransform as TransformGroup).Children;
      var translate = transforms.GetTranslate();
      var scale = transforms.GetScale();
      translate.AnimateTo(TranslateTransform.XProperty, 1d, LeaveTime);
      translate.AnimateTo(TranslateTransform.XProperty, 1d, LeaveTime);
      translate.AnimateTo(TranslateTransform.YProperty, 1d, LeaveTime);
      scale.AnimateTo(ScaleTransform.ScaleXProperty, 1d, LeaveTime);
      scale.AnimateTo(ScaleTransform.ScaleYProperty, 1d, LeaveTime);

      //translate.X = translate.Y = scale.ScaleX = scale.ScaleY = 1;
      //Animation
    }

    private static double WheelTime = 0.1;
    private static void Element_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      var element = sender as FrameworkElement;
      var parent = element.Parent as FrameworkElement;
      var transforms = (element.RenderTransform as TransformGroup).Children;
      var translate = transforms.GetTranslate();
      var scale = transforms.GetScale();

      double zoom = e.Delta > 0 ? .2 : -.2;

      var relative = e.GetPosition(element);
      var absolute = e.GetAbsolutePosition(element);
      //必須是scale先，translate後
      var ToScale = Math.Clamp(scale.ScaleX + zoom, GetMaximum(element), 1);
      var ToX = Math.Clamp(absolute.X - relative.X * ToScale, 0, parent.ActualWidth - element.ActualWidth * ToScale);
      var ToY = Math.Clamp(absolute.Y - relative.Y * ToScale, 0, parent.ActualHeight - element.ActualHeight * ToScale);
      scale.AnimateTo(ScaleTransform.ScaleXProperty, ToScale, WheelTime);
      scale.AnimateTo(ScaleTransform.ScaleYProperty, ToScale, WheelTime);
      translate.AnimateTo(TranslateTransform.XProperty, ToX, WheelTime);
      translate.AnimateTo(TranslateTransform.YProperty, ToY, WheelTime);
      
    }


    public static readonly DependencyProperty MaximumProperty = DependencyProperty.RegisterAttached(
      "Maximum",
      typeof(double),
      typeof(Zoom),
      new PropertyMetadata(5d, OnMaximumChanged));
    [AttachedPropertyBrowsableForType(typeof(UIElement))]
    public static double GetMaximum(UIElement element)
      => (double)element.GetValue(MaximumProperty);
    public static void SetMaximum(UIElement element, double value)
      => element.SetValue(MaximumProperty, value);

    private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var value = (double)e.NewValue;
      if (value < 1)
      {
        throw new InvalidOperationException($"{MaximumProperty} must be greater than inital scale (1).");
      }
    }

  }
}