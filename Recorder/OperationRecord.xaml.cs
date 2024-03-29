﻿using Recorder.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Recorder
{
    public enum OperationType
    {
        Keyboard,
        Mouse
    }
    public interface IOperationResult<ResultType>
    {
        ResultType GetResult();
    }
    public class Operation
    {
        public OperationType Type { get; set; }
        public Key Key { get; set; }
        public MouseButtonState ButtonState { get; set; }
        public MouseButton Button { get; set; }
        public Point Coordinate { get; set; }
        public Action<Operation> Do { get; set; }

    }
    /// <summary>
    /// OperationRecord.xaml 的交互逻辑
    /// </summary>
    public partial class OperationRecord : UserControl
    {
        KeyboardHook keyHook = new KeyboardHook();
        MouseHook mouseHook = new MouseHook();
        IntPtr current = IntPtr.Zero;
        public ObservableCollection<Operation> Operations
        {
            get { return (ObservableCollection<Operation>)GetValue(OperationsProperty); }
            set { SetValue(OperationsProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Operations.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OperationsProperty =
            DependencyProperty.Register(nameof(Operations), typeof(ObservableCollection<Operation>), typeof(OperationRecord), new PropertyMetadata(default(ObservableCollection<Operation>)));

        public bool Toggle
        {
            get { return (bool)GetValue(ToggleProperty); }
            set { SetValue(ToggleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Toggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToggleProperty =
            DependencyProperty.Register(nameof(Toggle), typeof(bool), typeof(OperationRecord), new PropertyMetadata(default(bool), OnToggleChanged));

        private static void OnToggleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = d as OperationRecord;
            if ((bool)e.NewValue)
            {
                // 开始记录
                i?.Start();
            }
            else
            {
                // 停止记录
                i?.Stop();
            }
        }

        private void Start()
        {
            if (Operations == null)
            {
                Operations = new ObservableCollection<Operation>();
            }
            else
            {
                Operations.Clear();
            }
            Process cProcess = Process.GetCurrentProcess();
            ProcessModule cModule = cProcess.MainModule;
            current = Win32Api.GetModuleHandle(cModule.ModuleName);
            keyHook.SetHook(current);
            keyHook.OnKeyDownEvent += Hook_OnKeyDownEvent;

            mouseHook.SetHook(current);
            mouseHook.OnMouseActivity += MouseHook_OnMouseActivity;
        }
        private void Stop()
        {
            Operations?.RemoveAt(Operations.Count - 1);
            keyHook.UnHook();
            keyHook.OnKeyDownEvent -= Hook_OnKeyDownEvent;

            mouseHook.UnHook();
            mouseHook.OnMouseActivity -= MouseHook_OnMouseActivity;
        }

        private void MouseHook_OnMouseActivity(object sender, Win32.MouseEventArgs args)
        {
            if (args.MouseButtonState == MouseButtonState.Released) return;
            Operation p = new Operation
            {
                Type = OperationType.Mouse,
                Coordinate = args.Coordinate,
                Button = args.MouseButton,
                ButtonState = args.MouseButtonState,
                Do = MouseAction
            };
            Operations.Add(p);
        }

        private void Hook_OnKeyDownEvent(object sender, Win32.KeyEventArgs args)
        {
            Operation p = new Operation
            {
                Type = OperationType.Keyboard,
                Key = args.Key,
            };
            Operations.Add(p);
        }

        public void MouseAction(Operation op)
        {
            var action = 0;
            int x = (int)op.Coordinate.X;
            int y = (int)op.Coordinate.Y;
            Win32Api.mouse_event(Win32Api.MOUSEEVENTF_ABSOLUTE |Win32Api.MOUSEEVENTF_MOVE, x, y, 0, 0);
            if (op.Button == MouseButton.Left)
                action = Win32Api.MOUSEEVENTF_ABSOLUTE | Win32Api.MOUSEEVENTF_LEFTDOWN | Win32Api.MOUSEEVENTF_LEFTUP;
            else if (op.Button == MouseButton.Right)
                action = Win32Api.MOUSEEVENTF_ABSOLUTE | Win32Api.MOUSEEVENTF_RIGHTDOWN | Win32Api.MOUSEEVENTF_RIGHTUP;
            Win32Api.mouse_event(action, x, y, 0, 0);
        }

        public OperationRecord()
        {
            InitializeComponent();
        }
    }

}
