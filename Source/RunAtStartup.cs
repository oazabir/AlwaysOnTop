using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
namespace AlwaysOnTop
{

    public static class RunAtStartup
    {        
        public static bool IsRunAtStartup()
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (rk.GetValue("AlwaysOnTop") == null)
                    return false;
                return rk.GetValue("AlwaysOnTop").ToString() == Application.ExecutablePath;
            }
        }
        public static void SetStartup(bool set)
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (set)
                    rk.SetValue("AlwaysOnTop", Application.ExecutablePath);
                else
                    if (rk.GetValue("AlwaysOnTop").ToString() == Application.ExecutablePath)
                        rk.DeleteValue("AlwaysOnTop");
            }
        }
    }
}
