using Recorder.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Point Coordinate { get; set; }

    }
    /// <summary>
    /// OperationRecord.xaml 的交互逻辑
    /// </summary>
    public partial class OperationRecord : UserControl
    {
        KeyboardHook keyHook = new KeyboardHook();
        MouseHook mouseHook = new MouseHook();
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
            keyHook.SetHook();
            keyHook.OnKeyDownEvent += Hook_OnKeyDownEvent;

            mouseHook.SetHook();
            mouseHook.OnMouseActivity += MouseHook_OnMouseActivity;
        }
        private void Stop()
        {
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

        public OperationRecord()
        {
            InitializeComponent();
        }
    }

}
