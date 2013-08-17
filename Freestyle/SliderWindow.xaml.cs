using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using MSHTML;

namespace Freestyle
{
    /// <summary>
    /// Interaction logic for SliderWindow.xaml
    /// </summary>
    public partial class SliderWindow : Window
    {
        WWAApp app;
        Profile p;
        HTMLDocument doc;

        int currentValue = 14;

        public SliderWindow(WWAApp app, HTMLDocument doc, Profile p)
        {
            this.app = app;
            this.p = p;
            this.doc = doc;

            Top = Left = 0;
            Topmost = true;

            InitializeComponent();


            App.ViewModel.CurrentAppChanging += () =>
                {
                    this.Invoke(() => Close());
                };
        }

        private void btnLarger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                p.ApplyAction(app, doc, (++currentValue).ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("*** Error Invoking Action: " + ex);
            }
        }

        private void btnSmaller_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                p.ApplyAction(app, doc, (--currentValue).ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("*** Error Invoking Action: " + ex);
            }
        }
    }
}
