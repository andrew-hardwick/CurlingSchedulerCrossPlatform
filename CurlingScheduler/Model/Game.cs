using System.Collections.Generic;

namespace CurlingScheduler.Model
{
    internal class Game
    {
        public IEnumerable<Team> Teams { get; set; }

        public int Sheet { get; set; }

        public int DrawGameIndex { get; set; }

        public int Stones { get; set; }
    }
}