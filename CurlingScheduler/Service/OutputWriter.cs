using CurlingScheduler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CurlingScheduler.Service
{
    internal class OutputWriter
    {
        internal string FormatGameSchedule(
            Schedule schedule,
            int sheetCount)
        {
            var lines = new List<string>();

            var sheets = GetSheetList(sheetCount);

            for (int weekIndex = 0; weekIndex < schedule.Weeks.Count(); weekIndex++)
            {
                var week = schedule.Weeks[weekIndex];

                if (weekIndex != 0)
                {
                    lines.Add(string.Empty);
                }
                lines.Add($"Week {weekIndex + 1}");

                for (int drawIndex = 0; drawIndex < week.Draws.Count(); drawIndex++)
                {
                    if (week.Draws[drawIndex].Games.Count() != 0)
                    {
                        var draw = week.Draws[drawIndex];

                        lines.Add($"  Draw { drawIndex + 1}");

                        var drawGames = new List<string>();

                        foreach (var game in draw.Games)
                        {
                            drawGames.Add($"    {sheets[game.Sheet]} - {game.Teams.ElementAt(0).Name} vs {game.Teams.ElementAt(1).Name}");
                        }

                        lines.AddRange(drawGames.OrderBy(l => l).ToArray());
                    }
                }
            }

            return string.Join(Environment.NewLine, lines);
        }

        internal string FormatStoneSchedule(
            Schedule schedule,
            int sheetCount)
        {
            var lines = new List<string>();

            var sheets = GetSheetList(sheetCount);

            for (int weekIndex = 0; weekIndex < schedule.Weeks.Count(); weekIndex++)
            {
                var week = schedule.Weeks[weekIndex];

                if (weekIndex != 0)
                {
                    lines.Add(string.Empty);
                }
                lines.Add($"Week {weekIndex + 1}");

                var sheetStoneMapping = new Dictionary<int, int>();

                for (int drawIndex = 0; drawIndex < week.Draws.Count(); drawIndex++)
                {
                    var draw = week.Draws[drawIndex];

                    foreach (var game in draw.Games)
                    {
                        sheetStoneMapping[game.Sheet] = game.Stones;
                    }
                }

                var weekSheetStones = new List<string>();

                foreach (var key in sheetStoneMapping.Keys)
                {
                    weekSheetStones.Add($"  {sheets[key]} - {sheetStoneMapping[key]}");
                }

                lines.AddRange(weekSheetStones.OrderBy(l => l).ToArray());
            }

            return string.Join(Environment.NewLine, lines);
        }

        private string[] GetSheetList(int sheetCount)
        {
            var ordered = Enumerable.Range(0, sheetCount)
                                    .Select(i => (char)(i + 'A'))
                                    .ToList();

            var result = new List<string>();

            foreach (var index in Enumerable.Range(0, sheetCount))
            {
                var middle = ordered[ordered.Count() / 2];

                result.Add($"{middle}");

                ordered.Remove(middle);
            }

            return result.ToArray();
        }

        internal void Write(
            Schedule schedule,
            string filename)
        {
            File.Delete(filename);

            var writer = File.AppendText(filename);

            writer.Write(JsonConvert.SerializeObject(schedule, Formatting.Indented));

            writer.Close();

            Process.Start(filename);
        }

        internal void Write(
            Dictionary<string, Team> teams,
            string filename)
        {
            File.Delete(filename);

            var writer = File.AppendText(filename);

            writer.Write(JsonConvert.SerializeObject(teams, Formatting.Indented));

            writer.Close();

            Process.Start(filename);
        }
    }
}