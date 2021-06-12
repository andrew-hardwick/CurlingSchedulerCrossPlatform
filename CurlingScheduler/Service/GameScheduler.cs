using CurlingScheduler.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CurlingScheduler.Service
{
    internal class GameScheduler
    {
        private Random _random = new Random();

        internal Schedule Schedule(
            ref Dictionary<string, Team> teams,
            int weekCount)
        {
            var allPairs = CreateAllPairs(teams).ToArray();

            var weeks = new List<Week>();

            foreach (var i in Enumerable.Range(0, weekCount))
            {
                weeks.Add(
                    CreateWeek(
                        ref teams,
                        allPairs[i % allPairs.Length]
                    )
                );
            }

            return new Schedule { Weeks = weeks };
        }

        private List<List<(string a, string b)>> CreateAllPairs(
            Dictionary<string, Team> teams)
        {
            var set = new List<List<(string, string)>>();

            var keys = teams.Keys.ToArray();

            var teamCount = keys.Length;

            var numPairs = teamCount / 2;

            var restKeys = keys.Skip(1).ToArray();
            var restLength = restKeys.Length;

            for(int i = 1; i < teamCount; i++)
            {
                var pairs = new List<(string a, string b)>();

                pairs.Add((keys[0], keys[i]));

                for(int offset = 1; offset < numPairs; offset++)
                {
                    var aIndex = (i - 1 - offset + restLength) % restLength;
                    var bIndex = (i - 1 + offset + restLength) % restLength;

                    pairs.Add((restKeys[aIndex], restKeys[bIndex]));
                }

                set.Add(pairs);
            }

            return set;
        }

        private Game CreateGame(
            ref Dictionary<string, Team> teams,
            string teamOneName,
            string teamTwoName)
        {
            teams[teamOneName].OpposingTeamCounts[teamTwoName]++;
            teams[teamTwoName].OpposingTeamCounts[teamOneName]++;

            return new Game { Teams = new Team[] { teams[teamOneName], teams[teamTwoName] } };
        }

        private Week CreateWeek(
            ref Dictionary<string, Team> teams,
            IEnumerable<(string t1, string t2)> games)
        {
            var week = new Week();

            foreach (var game in games)
            {
                week.GamesWithoutDrawAssignment.Add(CreateGame(ref teams, game.t1, game.t2));
            }

            return week;
        }
    }
}