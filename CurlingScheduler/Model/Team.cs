using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CurlingScheduler.Model
{
    public class Team
    {
        public Team(
            string name,
            IEnumerable<string> allTeamNames,
            int drawCount,
            int sheetCount)
        {
            Name = name;

            foreach (var opponent in allTeamNames.Where(n => !n.Equals(name)))
            {
                OpposingTeamCounts[opponent] = 0;
            }

            foreach (var index in Enumerable.Range(0, drawCount))
            {
                DrawCounts[index] = 0;
            }

            foreach (var index in Enumerable.Range(0, sheetCount))
            {
                SheetCounts[index] = 0;
                StoneCounts[index] = 0;
            }
        }

        public string Name { get; set; }

        [JsonIgnore]
        public Dictionary<string, int> OpposingTeamCounts { get; } =
            new Dictionary<string, int>();

        [JsonIgnore]
        public Dictionary<int, int> DrawCounts { get; } =
            new Dictionary<int, int>();

        [JsonIgnore]
        public Dictionary<int, int> SheetCounts { get; } =
            new Dictionary<int, int>();

        [JsonIgnore]
        public Dictionary<int, int> StoneCounts { get; } =
            new Dictionary<int, int>();
    }
}