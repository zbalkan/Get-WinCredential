using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GetWinCredential.Pinvoke
{
    internal static class USER32
    {
        private static readonly IntPtr TOPMOST = new IntPtr(-1);
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_SHOWWINDOW = 0x0040;

        internal static void SetOntop(IntPtr windowHandler) => _ = SetWindowPos(windowHandler, TOPMOST, Screen.PrimaryScreen.WorkingArea.Left, Screen.PrimaryScreen.WorkingArea.Top, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}