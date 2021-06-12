using CurlingScheduler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;

namespace CurlingScheduler.Service
{
    internal class StoneBalancer
    {
        internal void Schedule(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekCount,
            int drawCount,
            int stoneCount)
        {
            var sheetStoneCounts =
                new Dictionary<int, Dictionary<int, int>>();

            foreach (var sheet in Enumerable.Range(0, stoneCount))
            {
                sheetStoneCounts[sheet] = new Dictionary<int, int>();

                foreach (var stone in Enumerable.Range(0, stoneCount))
                {
                    sheetStoneCounts[sheet][stone] = 0;
                }
            }

            foreach (var weekIndex in Enumerable.Range(0, weekCount))
            {
                BalanceStones(
                    ref teams,
                    ref schedule,
                    weekIndex,
                    drawCount,
                    stoneCount,
                    ref sheetStoneCounts);
            }
        }

        /// <summary>
        /// This function balances stones for a week by searching through all
        /// possible permutations for the global minimum.
        /// 
        /// With large numbers of sheets, this could take a while to execute.
        /// Time complexity equals sheetCount! (factorial)
        /// </summary>
        /// <param name="teams"></param>
        /// <param name="schedule"></param>
        /// <param name="weekIndex"></param>
        /// <param name="drawCount"></param>
        /// <param name="stoneCount"></param>
        private void BalanceStones(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekIndex,
            int drawCount,
            int stoneCount,
            ref Dictionary<int,Dictionary<int, int>> sheetStoneCounts)
        {
            var stoneList = Enumerable.Range(0, stoneCount).ToList();

            var stonePerms = GetPermutations(stoneList, stoneCount).ToArray();

            int minimumHighTeamStoneCount = int.MaxValue;
            int mimimumHighSheetStoneCount = int.MaxValue;
            int minimumIndex = 0;

            var week = schedule.Weeks[weekIndex].Draws.SelectMany(d => d.Games);

            int index = 0;

            foreach (var perm in stonePerms)
            {
                var teamStoneCounts = new Dictionary<string, Dictionary<int, int>>();

                foreach (var team in teams.Keys)
                {
                    teamStoneCounts[team] = new Dictionary<int, int>();

                    foreach (var stone in Enumerable.Range(0, stoneCount))
                    {
                        teamStoneCounts[team][stone] = teams[team].StoneCounts[stone];
                    }
                }

                var sheetStoneCountsClone = new Dictionary<int, Dictionary<int, int>>();

                foreach (var sheet in Enumerable.Range(0, stoneCount))
                {
                    sheetStoneCountsClone[sheet] = new Dictionary<int, int>();

                    foreach (var stone in Enumerable.Range(0, stoneCount))
                    {
                        sheetStoneCountsClone[sheet][stone] = sheetStoneCounts[sheet][stone];
                    }
                }

                var permArray = perm.ToArray();
             
                foreach (var game in week)
                {
                    teamStoneCounts[game.Teams.ElementAt(0).Name][permArray[game.DrawGameIndex]]++;
                    teamStoneCounts[game.Teams.ElementAt(1).Name][permArray[game.DrawGameIndex]]++;

                    sheetStoneCountsClone[game.DrawGameIndex][permArray[game.DrawGameIndex]]++;
                }

                var highestTeamStoneCount = teamStoneCounts.SelectMany(t => t.Value)
                                                       .Select(p => p.Value)
                                                       .Max();

                var highestSheetStoneCount = sheetStoneCountsClone.SelectMany(t => t.Value)
                                                                  .Select(p => p.Value)
                                                                  .Max() ;

                if (highestTeamStoneCount < minimumHighTeamStoneCount)
                {
                    minimumHighTeamStoneCount = highestTeamStoneCount;
                    mimimumHighSheetStoneCount = highestSheetStoneCount;
                    minimumIndex = index;
                }
                else if (highestTeamStoneCount == minimumHighTeamStoneCount && highestSheetStoneCount < mimimumHighSheetStoneCount)
                {
                    mimimumHighSheetStoneCount = highestSheetStoneCount;
                    minimumIndex = index;
                }

                index++;
            }

            var selectedStonePerm = stonePerms[minimumIndex].ToArray();

            for(int drawIndex = 0; drawIndex < drawCount; drawIndex++)
            {
                for(int gameIndex = 0; gameIndex < schedule.Weeks[weekIndex].Draws[drawIndex].Games.Count(); gameIndex++)
                {
                    var drawGameIndex = schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].DrawGameIndex;
                    var stones = selectedStonePerm[drawGameIndex];

                    schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Stones = stones;

                    var teamOne = schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Teams.ElementAt(0).Name;
                    var teamTwo = schedule.Weeks[weekIndex].Draws[drawIndex].Games[gameIndex].Teams.ElementAt(1).Name;

                    teams[teamOne].StoneCounts[stones]++;
                    teams[teamTwo].StoneCounts[stones]++;
                }
            }

            foreach (var sheetIndex in Enumerable.Range(0, stoneCount))
            {
                sheetStoneCounts[sheetIndex][selectedStonePerm[sheetIndex]]++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private IEnumerable<IEnumerable<int>> GetPermutations(
            IEnumerable<int> source,
            int length)
        {
            if (length == 1) return source.Select(t => new int[] { t });

            return GetPermutations(source, length - 1)
                .SelectMany(t => source.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new int[] { t2 }));
        }
    }
}