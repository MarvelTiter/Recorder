using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Recorder.Win32
{
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(MouseButton mouseButton, MouseButtonState mouseButtonState, int clickCount, Point p)
        {
            MouseButton = mouseButton;
            MouseButtonState = mouseButtonState;
            ClickCount = clickCount;
            Coordinate = p;
        }

        public MouseButton MouseButton { get; }
        public MouseButtonState MouseButtonState { get; }
        public int ClickCount { get; }
        public Point Coordinate { get; }
    }
    public delegate void MouseEvent(object sender, MouseEventArgs args);
    /// <summary>
    /// 鼠标全局钩子
    /// </summary>
    public class MouseHook
    {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// 点
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// 钩子结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public const int WH_MOUSE_LL = 14; // mouse hook constant


        // 全局的鼠标事件
        public event MouseEvent OnMouseActivity;


        // 声明鼠标钩子事件类型
        private Win32Api.HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄

        /// <summary>
        /// 构造函数
        /// </summary>
        public MouseHook()
        {

        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MouseHook()
        {
            UnHook();
        }

        /// <summary>
        /// 启动全局钩子
        /// </summary>
        public void SetHook(IntPtr handle)
        {
            // 安装鼠标钩子
            if (_hMouseHook == 0)
            {
                // 生成一个HookProc的实例.
                _mouseHookProcedure = new Win32Api.HookProc(MouseHookProc);                
                _hMouseHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, handle, 0);
            }
        }

        /// <summary>
        /// 停止全局钩子
        /// </summary>
        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(_hMouseHook);
            _hMouseHook = 0;
        }

        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {            
            // 假设正常执行而且用户要监听鼠标的消息
            if (nCode >= 0 && wParam != WM_MOUSEMOVE && (OnMouseActivity != null))
            {
                MouseButton button = default;
                MouseButtonState buttonstate = default;
                int clickCount = 0;
                switch (wParam)
                {

                    case WM_LBUTTONDOWN:
                        button = MouseButton.Left;
                        buttonstate = MouseButtonState.Pressed;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButton.Left;
                        buttonstate = MouseButtonState.Released;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButton.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButton.Right;
                        buttonstate = MouseButtonState.Pressed;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButton.Right;
                        buttonstate = MouseButtonState.Released;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButton.Right;
                        clickCount = 2;
                        break;
                }

                // 从回调函数中得到鼠标的信息
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                var p = new Point() { X = MyMouseHookStruct.pt.x, Y = MyMouseHookStruct.pt.y };
                MouseEventArgs e = new MouseEventArgs(button, buttonstate, clickCount, p);
                OnMouseActivity(this, e);
            }

            // 启动下一次钩子
            return Win32Api.CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }
    }
}
