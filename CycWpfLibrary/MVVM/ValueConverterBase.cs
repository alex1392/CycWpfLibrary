﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CycWpfLibrary.MVVM
{
  public abstract class ValueConverterBase<T> : MarkupExtension, IValueConverter 
    where T : class, new()
  {
    static T converter;
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return converter ?? (converter = new T());
    }

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
  }
}
