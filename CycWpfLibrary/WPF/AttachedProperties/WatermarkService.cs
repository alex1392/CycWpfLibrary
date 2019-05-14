﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace CycWpfLibrary
{
  /// <summary>
  /// Class that provides the Watermark attached property
  /// </summary>
  public static class WatermarkService
  {
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
       "Watermark",
       typeof(object),
       typeof(WatermarkService),
       new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(OnWatermarkChanged)));
    public static object GetWatermark(DependencyObject d) 
      => d.GetValue(WatermarkProperty);
    public static void SetWatermark(DependencyObject d, object value) 
      => d.SetValue(WatermarkProperty, value);

    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (Control)d;
      control.Loaded += Control_Loaded;

      if (d is ComboBox)
      {
        control.GotKeyboardFocus += Control_GotKeyboardFocus;
        control.LostKeyboardFocus += Control_Loaded;
      }
      else if (d is TextBox)
      {
        control.GotKeyboardFocus += Control_GotKeyboardFocus;
        control.LostKeyboardFocus += Control_Loaded;
        ((TextBox)control).TextChanged += Control_GotKeyboardFocus;
      }

      if (d is ItemsControl && !(d is ComboBox))
      {
        var i = (ItemsControl)d;

        // for Items property  
        i.ItemContainerGenerator.ItemsChanged += ItemsChanged;
        itemsControls.Add(i.ItemContainerGenerator, i);

        // for ItemsSource property  
        var prop = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, i.GetType());
        prop.AddValueChanged(i, ItemsSourceChanged);
      }
    }

    /// <summary>
    /// Dictionary of ItemsControls
    /// </summary>
    private static readonly Dictionary<object, ItemsControl> itemsControls = new Dictionary<object, ItemsControl>();

    /// <summary>
    /// Handle the GotFocus event on the control
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
    private static void Control_GotKeyboardFocus(object sender, RoutedEventArgs e)
    {
      var c = (Control)sender;
      if (ShouldShowWatermark(c))
      {
        ShowWatermark(c);
      }
      else
      {
        RemoveWatermark(c);
      }
    }

    /// <summary>
    /// Handle the Loaded and LostFocus event on the control
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="RoutedEventArgs"/> that contains the event data.</param>
    private static void Control_Loaded(object sender, RoutedEventArgs e)
    {
      var control = (Control)sender;
      if (ShouldShowWatermark(control))
      {
        ShowWatermark(control);
      }
    }

    /// <summary>
    /// Event handler for the items source changed event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
    private static void ItemsSourceChanged(object sender, EventArgs e)
    {
      var c = (ItemsControl)sender;
      if (c.ItemsSource != null)
      {
        if (ShouldShowWatermark(c))
        {
          ShowWatermark(c);
        }
        else
        {
          RemoveWatermark(c);
        }
      }
      else
      {
        ShowWatermark(c);
      }
    }

    /// <summary>
    /// Event handler for the items changed event
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ItemsChangedEventArgs"/> that contains the event data.</param>
    private static void ItemsChanged(object sender, ItemsChangedEventArgs e)
    {
      ItemsControl control;
      if (itemsControls.TryGetValue(sender, out control))
      {
        if (ShouldShowWatermark(control))
        {
          ShowWatermark(control);
        }
        else
        {
          RemoveWatermark(control);
        }
      }
    }

    /// <summary>
    /// Remove the watermark from the specified element
    /// </summary>
    /// <param name="control">Element to remove the watermark from</param>
    private static void RemoveWatermark(UIElement control)
    {
      var layer = AdornerLayer.GetAdornerLayer(control);

      // layer could be null if control is no longer in the visual tree
      if (layer != null)
      {
        var adorners = layer.GetAdorners(control);
        if (adorners == null)
        {
          return;
        }

        foreach (var adorner in adorners)
        {
          if (adorner is WatermarkAdorner)
          {
            adorner.Visibility = Visibility.Hidden;
            layer.Remove(adorner);
          }
        }
      }
    }

    /// <summary>
    /// Show the watermark on the specified control
    /// </summary>
    /// <param name="control">Control to show the watermark on</param>
    private static void ShowWatermark(Control control)
    {
      var layer = AdornerLayer.GetAdornerLayer(control);

      // layer could be null if control is no longer in the visual tree
      if (layer != null)
      {
        layer.Add(new WatermarkAdorner(control, GetWatermark(control)));
      }
    }

/// <summary>
    /// Indicates whether or not the watermark should be shown on the specified control
    /// </summary>
    /// <param name="c"><see cref="Control"/> to test</param>
    /// <returns>true if the watermark should be shown; false otherwise</returns>
    private static bool ShouldShowWatermark(Control c)
    {
      if (c is ComboBox)
      {
        return (c as ComboBox).Text == string.Empty;
      }
      else if (c is TextBoxBase)
      {
        return (c as TextBox).Text == string.Empty;
      }
      else if (c is ItemsControl)
      {
        return (c as ItemsControl).Items.Count == 0;
      }
      else
      {
        return false;
      }
    }

  }

  /// <summary>
  /// Adorner for the watermark
  /// </summary>
  internal class WatermarkAdorner : Adorner
  {
    /// <summary>
    /// <see cref="ContentPresenter"/> that holds the watermark
    /// </summary>
    private readonly ContentPresenter contentPresenter;

    /// <summary>
    /// Initializes a new instance of the <see cref="WatermarkAdorner"/> class
    /// </summary>
    /// <param name="adornedElement"><see cref="UIElement"/> to be adorned</param>
    /// <param name="watermark">The watermark</param>
    public WatermarkAdorner(UIElement adornedElement, object watermark) :
       base(adornedElement)
    {
      IsHitTestVisible = false;

      contentPresenter = new ContentPresenter();
      contentPresenter.Content = watermark;
      contentPresenter.Opacity = 0.5;
      contentPresenter.Margin = new Thickness(Control.Margin.Left + Control.Padding.Left, Control.Margin.Top + Control.Padding.Top, 0, 0);

      if (Control is ItemsControl && !(Control is ComboBox))
      {
        contentPresenter.VerticalAlignment = VerticalAlignment.Center;
        contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
      }

      // Hide the control adorner when the adorned element is hidden
      var binding = new Binding("IsVisible");
      binding.Source = adornedElement;
      binding.Converter = new BooleanToVisibilityConverter();
      SetBinding(VisibilityProperty, binding);
    }

    /// <summary>
    /// Gets the number of children for the <see cref="ContainerVisual"/>.
    /// </summary>
    protected override int VisualChildrenCount => 1;

    /// <summary>
    /// Gets the control that is being adorned
    /// </summary>
    private Control Control => (Control)AdornedElement;

    /// <summary>
    /// Returns a specified child <see cref="Visual"/> for the parent <see cref="ContainerVisual"/>.
    /// </summary>
    /// <param name="index">A 32-bit signed integer that represents the index value of the child <see cref="Visual"/>. The value of index must be between 0 and <see cref="VisualChildrenCount"/> - 1.</param>
    /// <returns>The child <see cref="Visual"/>.</returns>
    protected override Visual GetVisualChild(int index)
    {
      return contentPresenter;
    }

    /// <summary>
    /// Implements any custom measuring behavior for the adorner.
    /// </summary>
    /// <param name="constraint">A size to constrain the adorner to.</param>
    /// <returns>A <see cref="Size"/> object representing the amount of layout space needed by the adorner.</returns>
    protected override Size MeasureOverride(Size constraint)
    {
      // Here's the secret to getting the adorner to cover the whole control
      contentPresenter.Measure(Control.RenderSize);
      return Control.RenderSize;
    }
    /// <summary>
    /// When overridden in a derived class, positions child elements and determines a size for a <see cref="FrameworkElement"/> derived class. 
    /// </summary>
    /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
    /// <returns>The actual size used.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
      contentPresenter.Arrange(new Rect(finalSize));
      return finalSize;
    }

  }
}
