using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recorder.Win32
{
    public class KeyEventArgs : EventArgs
    {
        public Key Key { get; }
        public KeyEventArgs(Key key)
        {
            Key = key;
        }

    }
    public delegate void KeyEvent(object sender, KeyEventArgs args);
    public class KeyboardHook
    {
        int hHook;
        Win32Api.HookProc KeyboardHookDelegate;
        public event KeyEvent OnKeyDownEvent;
        public event KeyEvent OnKeyUpEvent;
        public KeyboardHook() { }
        public void SetHook()
        {
            if (hHook == 0)
            {
                KeyboardHookDelegate = new Win32Api.HookProc(KeyboardHookProc);
                Process cProcess = Process.GetCurrentProcess();
                ProcessModule cModule = cProcess.MainModule;
                var mh = Win32Api.GetModuleHandle(cModule.ModuleName);
                hHook = Win32Api.SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
            }
        }

        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
            hHook = 0;
        }

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
            if ((nCode >= 0) && (OnKeyDownEvent != null || OnKeyUpEvent != null))
            {
                Win32Api.KeyboardHookStruct KeyDataFromHook = (Win32Api.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));

                Key keyData = KeyInterop.KeyFromVirtualKey(KeyDataFromHook.vkCode);


                //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件
                if (OnKeyDownEvent != null && (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN))
                {
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    OnKeyDownEvent(this, e);
                }

                //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 
                if (OnKeyUpEvent != null && (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))
                {
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    OnKeyUpEvent(this, e);
                }
            }

            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);

        }
    }
}
