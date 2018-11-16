﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CycWpfLibrary.MVVM
{
  /// <summary>
  /// 提供具有<see cref="IValueConverter"/>以及<see cref="IMultiValueConverter"/>功能的類別。
  /// </summary>
  /// <typeparam name="ComboConverterType">要實作的ComboConverter類別。</typeparam>
  public abstract class ComboConverterBase<ComboConverterType> : XamlMarkupObject<ComboConverterType>, IValueConverter, IMultiValueConverter where ComboConverterType : class, new()
  {
    public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);

    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
  }
}
