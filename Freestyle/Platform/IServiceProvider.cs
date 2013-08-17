using System;
using System.Runtime.InteropServices;

namespace Freestyle.Platform
{
    [ComImport]
    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IServiceProvider
    {
        void QueryService(ref Guid guidService, ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
    }
}

public static class IServicePvodierExtensions
{
    public static T QueryService<T>(this Freestyle.Platform.IServiceProvider sp)
    {
        Guid IID = typeof(T).GUID;
        object ret;
        sp.QueryService(ref IID, ref IID, out ret);
        return (T)ret;
    }
}
