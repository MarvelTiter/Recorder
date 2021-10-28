using MT.Controls;
using MT.Mvvm;
using Recorder.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Operation> _Records;
        public ObservableCollection<Operation> Records
        {
            get { return _Records; }
            set { SetValue(ref _Records, value); }
        }

        private OperationSetting _Setting = new OperationSetting();
        public OperationSetting Setting
        {
            get { return _Setting; }
            set { SetValue(ref _Setting, value); }
        }

        public RelayCommand ExecuteCommand => new RelayCommand(async () =>
        {
            var count = Setting.LoopTimes;
            var actionInterval = Setting.ActionInterval;
            var loopInterval = Setting.LoopInterval;
            await Task.Run(async () =>
            {
                while (count > 0)
                {
                    foreach (var item in Records)
                    {
                        item.Do.Invoke(item);
                        await Task.Delay(actionInterval);
                    }
                    count--;
                    await Task.Delay(loopInterval);
                }
            });
        });


        public RelayCommand ClearCommand => new RelayCommand(() =>
        {
            Records?.Clear();
        });

        public RelayCommand ExitCommand => new RelayCommand(() =>
        {
            Application.Current.Shutdown();
        });
    }
}
