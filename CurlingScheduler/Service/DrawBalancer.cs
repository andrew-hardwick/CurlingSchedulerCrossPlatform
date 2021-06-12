using CurlingScheduler.Model;
using System.Collections.Generic;
using System.Linq;

namespace CurlingScheduler.Service
{
    internal class DrawBalancer
    {
        internal void Schedule(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekCount,
            int drawCount,
            int drawGameCount,
            DrawAlignment drawAlignment)
        {
            foreach (var weekIndex in Enumerable.Range(0, weekCount))
            {
                switch (drawAlignment)
                {
                    case DrawAlignment.Balanced:
                        BalanceDraws(
                            ref teams,
                            ref schedule,
                            weekIndex,
                            drawCount);

                        break;

                    default:
                    case DrawAlignment.Squished:
                        SquishDraws(
                            ref teams,
                            ref schedule,
                            weekIndex,
                            drawCount,
                            drawGameCount);

                        break;
                }
            }
        }

        private void BalanceDraws(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekIndex,
            int drawCount)
        {
            var draws = Enumerable.Range(0, drawCount)
                                  .Select(i => new Draw())
                                  .ToList();

            while (true)
            {
                foreach (var drawIndex in Enumerable.Range(0, drawCount))
                {
                    if (schedule.Weeks[weekIndex].GamesWithoutDrawAssignment.Count() == 0)
                    {
                        schedule.Weeks[weekIndex].Draws = draws;
                        return;
                    }

                    var orderedGames =
                        schedule.Weeks[weekIndex].GamesWithoutDrawAssignment.OrderBy(
                            g => g.Teams.Select(t => t.DrawCounts[drawIndex]).Sum()
                            );

                    var selectedGame = orderedGames.ElementAt(0);

                    schedule.Weeks[weekIndex].GamesWithoutDrawAssignment.Remove(selectedGame);

                    teams[selectedGame.Teams.ElementAt(0).Name].DrawCounts[drawIndex]++;
                    teams[selectedGame.Teams.ElementAt(1).Name].DrawCounts[drawIndex]++;

                    draws[drawIndex].Games.Add(selectedGame);
                }
            }
        }

        private void SquishDraws(
            ref Dictionary<string, Team> teams,
            ref Schedule schedule,
            int weekIndex,
            int drawCount,
            int sheetCount)
        {
            var draws = Enumerable.Range(0, drawCount)
                                  .Select(i => new Draw())
                                  .ToList();

            foreach (var drawIndex in Enumerable.Range(0, drawCount))
            {
                foreach (var sheetIndex in Enumerable.Range(0, sheetCount))
                {
                    if (schedule.Weeks[weekIndex].GamesWithoutDrawAssignment.Count() == 0)
                    {
                        schedule.Weeks[weekIndex].Draws = draws;
                        return;
                    }

                    var orderedGames =
                        schedule.Weeks[weekIndex].GamesWithoutDrawAssignment.OrderBy(
                            g => g.Teams.Select(t => t.DrawCounts[drawIndex]).Sum()
                            );

                    var selectedGame = orderedGames.ElementAt(0);

                    schedule.Weeks[weekIndex].GamesWithoutDrawAssignment.Remove(selectedGame);

                    teams[selectedGame.Teams.ElementAt(0).Name].DrawCounts[drawIndex]++;
                    teams[selectedGame.Teams.ElementAt(1).Name].DrawCounts[drawIndex]++;

                    draws[drawIndex].Games.Add(selectedGame);
                }
            }

            schedule.Weeks[weekIndex].Draws = draws;
        }
    }
}