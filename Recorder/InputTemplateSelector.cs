using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Recorder
{
    internal class InputTemplateSelector : DataTemplateSelector
    {
        public DataTemplate KeyboardTemplate { get; set; }
        public DataTemplate MouseTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Operation p && p != null)
            {
                if (p.Type == OperationType.Keyboard)
                    return KeyboardTemplate;
                else
                    return MouseTemplate;
            }
            return KeyboardTemplate;
        }
    }
}
