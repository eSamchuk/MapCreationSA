using System;
using System.Runtime.InteropServices;
using System.Text;


namespace MapCreationSA
{
    class iniReader
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(int hWnd, String text, String caption, uint type);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileString(String sSection, String sKey, String sDefault, StringBuilder sString, int iSize, String sFile);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool WritePrivateProfileString(String sSection, String sKey, String sString, String sFile);

    }
}
