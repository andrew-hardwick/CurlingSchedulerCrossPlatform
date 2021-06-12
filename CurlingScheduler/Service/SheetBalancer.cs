using CurlingScheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace CurlingScheduler.Service
{
    internal class SheetBalancer
    {
        internal void Schedule(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekCount,
            int drawCount,
            int sheetCount)
        {
            foreach (var weekIndex in Enumerable.Range(0, weekCount))
            {
                BalanceSheets(
                    ref teams,
                    ref schedule,
                    weekIndex,
                    drawCount,
                    sheetCount);

            }
        }

        private void BalanceSheets(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekIndex,
            int drawCount,
            int sheetCount)
        {
            foreach (var drawIndex in Enumerable.Range(0, drawCount))
            {
                var availableSheetIndices = Enumerable.Range(0, sheetCount).ToList();

                var gamesByLowestSheetCount = schedule.Weeks[weekIndex].Draws[drawIndex].Games.OrderBy(g => g.Teams.Min(t => t.SheetCounts.Values.Min()));

                var games = schedule.Weeks[weekIndex].Draws[drawIndex].Games;

                foreach (var gameIndex in gamesByLowestSheetCount.Select(g => games.IndexOf(g)))
                {
                    var sheetCountsPerTeam = schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Teams.Select(t => t.SheetCounts);

                    var sheetCounts = new Dictionary<int, int>();
                    foreach (var team in sheetCountsPerTeam)
                    {
                        foreach (var key in team.Keys)
                        {
                            if (!sheetCounts.ContainsKey(key))
                            {
                                sheetCounts[key] = 0;
                            }
                            sheetCounts[key] += team[key];
                        }
                    }

                    var rankedDesiredSheets = sheetCounts.ToList().OrderBy(p => p.Value).Select(p => p.Key).ToList();

                    var foundMatch = false;

                    var choice = 0;

                    while (!foundMatch)
                    {
                        choice = rankedDesiredSheets.ElementAt(0);

                        if (availableSheetIndices.Contains(choice))
                        {
                            foundMatch = true;
                        }

                        rankedDesiredSheets.Remove(choice);
                    }

                    availableSheetIndices.Remove(choice);
                    schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Sheet = choice;
                    schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Stones = choice;

                    var name = schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Teams.ElementAt(0).Name;
                    teams[name].SheetCounts[choice]++;

                    name = schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Teams.ElementAt(1).Name;
                    teams[name].SheetCounts[choice]++;
                }
                schedule.Weeks[weekIndex].Draws[drawIndex].Games = schedule.Weeks[weekIndex].Draws[drawIndex].Games.OrderBy(g => g.Sheet).ToList();

                for(int drawGameIndex = 0; drawGameIndex < schedule.Weeks[weekIndex].Draws[drawIndex].Games.Count(); drawGameIndex++)
                {
                    schedule.Weeks[weekIndex].Draws[drawIndex].Games[drawGameIndex].DrawGameIndex = drawGameIndex;
                }
            }
        }
    }
}
