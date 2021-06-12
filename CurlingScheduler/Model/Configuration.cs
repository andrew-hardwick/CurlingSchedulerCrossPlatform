using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurlingScheduler.Model
{
    public class Configuration
    {
        public string[] Teams { get; set; } = new string[] { };
        public int WeekCount { get; set; } = 7;
        public int DrawCount { get; set; } = 1;
        public int SheetCount { get; set; } = 5;
        public int StoneCount { get; set; } = 5;
        public string DrawAlignment { get; set; }
    }
}
