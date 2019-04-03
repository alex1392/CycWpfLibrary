﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CycWpfLibrary
{
  public interface IViewValidation
  {
    bool IsViewValid { get; set; }
  }

  public static class TextBoxBehaviors
  {
    public static void TextBox_LostFocus(object sender, RoutedEventArgs e, IViewValidation viewModel, DependencyObject view)
    {
      if (sender is TextBox tb)
        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
      else if (sender is PasswordBox pb)
        pb.GetBindingExpression(PasswordObserver.PasswordProperty).UpdateSource();
      viewModel.IsViewValid = ValidationHelpers.IsValid(view);
    }

    public static void TextBox_Error(object sender, ValidationErrorEventArgs e)
    {
      (sender as TextBox).Clear();
    }

    public static void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      (sender as TextBox).SelectAll();
    }

    public static void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
    {
      (sender as TextBox).SelectAll();
    }

    public static void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        Keyboard.ClearFocus();
      }
    }
  }
}
