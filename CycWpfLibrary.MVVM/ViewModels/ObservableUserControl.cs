﻿using System.ComponentModel;
using System.Windows.Controls;

namespace CycWpfLibrary.MVVM
{
  /// <summary>
  /// 提供實現了<see cref="INotifyPropertyChanged"/>介面的基底類別。
  /// 請將<see cref="UserControl"/>繼承自此類別，並使用Fody.PropertyChanged以自動實作<see cref="PropertyChanged"/>
  /// </summary>
  public class ObservableUserControl : UserControl, INotifyPropertyChanged
  {
    /// <summary>
    /// 當其他屬性變更時觸發此事件。
    /// 請安裝Fody.PropertyChanged套件以自動呼叫此事件。
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// 提供方法呼叫<see cref="PropertyChanged"/>事件。
    /// </summary>
    /// <param name="property">發生<see cref="PropertyChanged"/>的屬性。</param>
    public void OnPropertyChanged(string property)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
