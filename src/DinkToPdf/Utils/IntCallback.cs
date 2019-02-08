using System;
using System.Runtime.InteropServices;

namespace DinkToPdf
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IntCallback(IntPtr converter, int integer);
}