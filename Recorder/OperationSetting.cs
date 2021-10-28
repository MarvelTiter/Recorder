using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recorder
{
    public class OperationSetting
    {
        public int ActionInterval { get; set; } = 500;
        public int LoopInterval { get; set; } = 500;
        public int LoopTimes { get; set; } = 1;
    }
}
