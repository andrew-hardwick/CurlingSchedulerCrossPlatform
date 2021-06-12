using Newtonsoft.Json;
using System.Collections.Generic;

namespace CurlingScheduler.Model
{
    internal class Week
    {
        public List<Draw> Draws { get; set; }

        [JsonIgnore]
        public List<Game> GamesWithoutDrawAssignment { get; set; } = new List<Game>();
    }
}