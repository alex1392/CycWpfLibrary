﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycWpfLibrary.MVVM
{
  public class DebugConverter : ValueConverterBase<DebugConverter>
  {
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //Debugger.Break();
      return value;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      //Debugger.Break();
      return value;
    }
  }
}
