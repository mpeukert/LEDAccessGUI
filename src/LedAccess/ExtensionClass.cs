namespace LedAccess
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    public static class ExtensionClass
    {
        public static void AddOnUI<T>(this ObservableCollection<T> list, T element)
        {
            Action<T> addMethod = list.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, element);
        }
    }
}
