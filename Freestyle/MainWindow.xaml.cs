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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using wf = System.Windows.Forms;
using System.Windows.Interop;
using System.Threading;
using System.Diagnostics;
using MSHTML;
using System.IO;

namespace Freestyle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Topmost = true;
            Top = Left = 0;
            InitializeComponent();

            App.ViewModel.Initialize();
        }

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x = 0;
            public int y = 0;
        }

        [DllImport("User32", EntryPoint = "ClientToScreen", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);

        private wf.ToolStripMenuItem MakeItem(string name, Action Click)
        {
            var item = new wf.ToolStripMenuItem();
            item.Text = name;
            item.Click += (s, e) => Click();

            return item;
        }

        private void InvokeMenu()
        {
            var menu = new wf.ContextMenuStrip();
            menu.Items.Add(MakeItem("Exit", () => Environment.Exit(0)));
            menu.Items.Add(new wf.ToolStripSeparator());

            var app = App.ViewModel.CurrentApp;
            if (app != null)
            {
                foreach (var profInner in app.Profiles)
                {
                    var p = profInner; // Closure Fix
                    if (p.HasAction)
                    {
                        menu.Items.Add(MakeItem(p.Name,
                        () =>
                        {
                            if (p.HasActionValue)
                            {
                                OpenProfileWindow(app, app.AppDocument, p);
                            }
                            else
                            {
                                p.ApplyAction(app, app.AppDocument, null);
                            }
                        }));
                    }
                }

                // Add more items
                menu.Items.Add(MakeItem("Export to disk", () =>
                {
                    ExportPage.ExportAll("App Document", app.AppDocument);
                }));

                var Frames = app.Frames;
                if (Frames.Count ==1)
                {
                    var frame = Frames.Values.First();
                    menu.Items.Add(MakeItem("Reader Mode", () =>
                    {
                        Reader.EnableReaderMode(app, frame);
                    }));
                }
                menu.Items.Add(new wf.ToolStripSeparator());

                foreach(var kv2 in app.Frames)
                {
                    var kv = kv2; // Closure Fix

                    wf.ToolStripMenuItem item = new wf.ToolStripMenuItem();
                    item.Text = kv.Key;
                    foreach (var profInner in app.Profiles)
                    {
                        var p = profInner; // Closure Fix
                        if (p.HasAction)
                        {
                            item.DropDownItems.Add(MakeItem(
                                p.Name,
                                () =>
                                {
                                    if (p.HasActionValue)
                                    {
                                        OpenProfileWindow(app, kv.Value, p);
                                    }
                                    else
                                    {
                                        p.ApplyAction(app, kv.Value, null);
                                    }
                                }));
                        }
                    }

                    item.DropDownItems.Add(new wf.ToolStripSeparator());

                    item.DropDownItems.Add(MakeItem("Reader Mode", () =>
                    {
                        Reader.EnableReaderMode(app, kv.Value);
                    }));
                    item.DropDownItems.Add(MakeItem("Export to disk", () =>
                    {
                        ExportPage.ExportAll(kv.Key, kv.Value);
                    }));
                    menu.Items.Add(item);
                }
            }

            POINT pt = new POINT();
            ClientToScreen(new WindowInteropHelper(this).Handle, pt);
            menu.Show(pt.x, pt.y);
        }

        private void OpenProfileWindow(WWAApp app, HTMLDocument doc, Profile p)
        {
            //Thread thread = new Thread(() =>
           // {
                SliderWindow ct = new SliderWindow(app, doc, p);
                ct.Show();
            //    System.Windows.Threading.Dispatcher.Run();
             //   Trace.WriteLine("UI Thread Exiting");
            //});
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Environment.Exit(0);
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            InvokeMenu();
        }

        bool TouchActionIsMove = false;
        private void Window_TouchDown(object sender, TouchEventArgs e)
        {
            TouchActionIsMove = false;
        }

        private void Window_TouchMove(object sender, TouchEventArgs e)
        {
            //this.DragMove();
            
            TouchActionIsMove = true;
        }

        private void Window_TouchUp(object sender, TouchEventArgs e)
        {
            //if (!TouchActionIsMove)
            //{
                InvokeMenu();
            //}
        }
    }
}
