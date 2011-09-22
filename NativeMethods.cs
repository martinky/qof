using System;
using System.Runtime.InteropServices;

namespace QuickOpenFile
{
    public static class NativeMethods
    {

        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
    }
}
