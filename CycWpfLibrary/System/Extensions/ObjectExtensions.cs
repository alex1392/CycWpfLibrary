﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycWpfLibrary
{
  public static class ObjectExtensions
  {
    public static bool IsNumeric(this object obj)
    {
      switch (Type.GetTypeCode(obj.GetType()))

      {
        case TypeCode.Byte:
        case TypeCode.SByte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.Decimal:
        case TypeCode.Double:
        case TypeCode.Single:
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// return empty string if object is null
    /// </summary>
    public static string ToStringEx(this object obj)
    {
      return (obj ?? "").ToString();
    }
  }
}
