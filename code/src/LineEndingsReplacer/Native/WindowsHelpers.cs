/*
 * MIT License
 * 
 * Copyright (c) 2020 plexdata.de
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Plexdata.LineEndingsReplacer.Native
{
    internal static class WindowsHelpers
    {
        #region Public Methods

        public static Icon GetSystemIcon(String filename, Boolean small, Boolean folder, Boolean overlay)
        {
            UInt32 flags = WindowsHelpers.SHGFI_ICON | WindowsHelpers.SHGFI_USEFILEATTRIBUTES;
            UInt32 attributes = folder ? WindowsHelpers.FILE_ATTRIBUTE_DIRECTORY : WindowsHelpers.FILE_ATTRIBUTE_NORMAL;

            if (overlay)
            {
                flags |= WindowsHelpers.SHGFI_LINKOVERLAY;
            }

            if (small)
            {
                flags |= WindowsHelpers.SHGFI_SMALLICON;
            }
            else
            {
                flags |= WindowsHelpers.SHGFI_LARGEICON;
            }

            WindowsHelpers.SHFILEINFO shfi = new WindowsHelpers.SHFILEINFO();

            WindowsHelpers.SHGetFileInfo(filename, attributes, ref shfi, (UInt32)Marshal.SizeOf(shfi), flags);

            Icon icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();

            WindowsHelpers.DestroyIcon(shfi.hIcon);

            return icon;
        }

        public static String GetSystemType(String filename, Boolean folder)
        {
            UInt32 flags = WindowsHelpers.SHGFI_TYPENAME | WindowsHelpers.SHGFI_USEFILEATTRIBUTES;

            UInt32 attributes = folder ? WindowsHelpers.FILE_ATTRIBUTE_DIRECTORY : WindowsHelpers.FILE_ATTRIBUTE_NORMAL;

            WindowsHelpers.SHFILEINFO shfi = new WindowsHelpers.SHFILEINFO();

            WindowsHelpers.SHGetFileInfo(filename, attributes, ref shfi, (UInt32)Marshal.SizeOf(shfi), flags);

            return shfi.szTypeName;
        }

        #endregion

        #region Native Access

        private const UInt32 SHGFI_ICON = 0x000000100;
        private const UInt32 SHGFI_TYPENAME = 0x000000400;
        private const UInt32 SHGFI_LINKOVERLAY = 0x000008000;
        private const UInt32 SHGFI_LARGEICON = 0x000000000;
        private const UInt32 SHGFI_SMALLICON = 0x000000001;
        private const UInt32 SHGFI_USEFILEATTRIBUTES = 0x000000010;

        private const UInt32 FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const UInt32 FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public Int32 iIcon;
            public UInt32 dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [DllImport("Shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
            String pszPath,
            UInt32 dwFileAttributes,
            ref SHFILEINFO psfi,
            UInt32 cbFileInfo,
            UInt32 uFlags
        );

        [DllImport("User32.dll")]
        private static extern Int32 DestroyIcon(IntPtr hIcon);

        #endregion
    }
}
