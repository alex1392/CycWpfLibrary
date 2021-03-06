﻿using System.Collections.Generic;
using System.Linq;

namespace CycWpfLibrary
{
  public static class ImageFileExtensions
  {
    public static string String => "*" + string.Join(";*", List);

    public static List<string> List => LowerCase.Concat(UpperCase).ToList();

    private static List<string> UpperCase => LowerCase.Select(s => s.ToUpper()).ToList();

    private static readonly List<string> LowerCase = new List<string>
    {
      ".jpg",
      ".bmp",
      ".png",
    };
  }
}
