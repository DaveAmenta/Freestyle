using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices.Expando;
using System.Reflection;

namespace Freestyle
{
    public static class Extensions
    {
        public static object InvokeMember(this object o, string member)
        {
            return ((IExpando)o).InvokeMember(member, BindingFlags.InvokeMethod, Type.DefaultBinder, ((IExpando)o), new object[] { }, new ParameterModifier[] { }, null, new string[] { });
        }

        public static object GetProperty(this object o, string member)
        {
            var ifr = (IReflect)o;

            var props = ifr.GetProperties(BindingFlags.Default);

            foreach (var p in props)
            {
                //Trace.WriteLine(p.Name);
            }

            var exp = ((IExpando)o);

            return exp.InvokeMember(member, BindingFlags.GetProperty, Type.DefaultBinder, exp, new object[] { }, new ParameterModifier[] { }, null, new string[] { });
        }

        public static object SetProperty(this object o, string member, string propertyContent)
        {
            return ((IExpando)o).InvokeMember(member, BindingFlags.SetProperty, Type.DefaultBinder, ((IExpando)o), new object[] { propertyContent }, new ParameterModifier[] { }, null, new string[] { });
        } 
        public static void Invoke(this Window w, Action a)
        {
            try
            {
                w.Dispatcher.Invoke(DispatcherPriority.Input, (ThreadStart)(() =>
                {
                    try
                    {
                        a.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("*** Error Invoking Action: " + ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Trace.WriteLine("*** Error Invoking Action (Dispatcher): " + ex);
            }
        }
    }
}
