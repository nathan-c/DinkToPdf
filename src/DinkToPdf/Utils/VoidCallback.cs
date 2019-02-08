using System;
using System.Runtime.InteropServices;

namespace DinkToPdf
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VoidCallback(IntPtr converter);
}