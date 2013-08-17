using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Freestyle
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainViewModel _ViewModel = null;
        public static MainViewModel ViewModel
        {
            // Note: not thread safe
            get
            {
                if (_ViewModel == null)
                {
                    _ViewModel = new MainViewModel();
                }
                return _ViewModel;
            }
        }
    }
}
