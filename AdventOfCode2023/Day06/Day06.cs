using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day06
{
    internal class Day06 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day06/input.txt");

            var raceTimes = BuildRaceTimes(parsed.RaceRecords.Max(race => race.time));

            var betterTimes = parsed.RaceRecords.ToDictionary(race => race.time, race => GetBetterDistances(raceTimes, race.time, race.distanceRecord));

            var multiplied = betterTimes.Values.Aggregate(1, (current, next) => current * next);

            Console.WriteLine(multiplied);
        }

        /// <summary>
        /// Gets a 2d matrix with the button time held as the row and the distance traveled at each time as the column
        /// </summary>
        /// <param name="maxTime">Maximum race time</param>
        /// <returns></returns>
        private int[,] BuildRaceTimes(int maxTime)
        {
            int[,] distances = new int[maxTime+1, maxTime+1];

            for (int timeHeld=1; timeHeld<distances.GetLength(0); timeHeld++)
            {
                int distance = 0;
                for (int movingTime=timeHeld; movingTime < distances.GetLength(0); movingTime++)
                {
                    distances[timeHeld, movingTime] = distance;
                    distance += timeHeld;
                }
            }

            return distances;
        }

        private int GetBetterDistances(int[,] distances, int raceTime, int currentBest)
        {
            int betterCount = 0;
            for (int timeHeld=0; timeHeld<Math.Min(distances.GetLength(0), raceTime); timeHeld++)
            {
                if (distances[timeHeld, raceTime] > currentBest)
                {
                    betterCount++;
                }
            }

            return betterCount;
        }

        private async Task<ProblemData> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            var topRow = input[0].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            var bottomRow = input[1].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

            List<RaceRecord> records = new();

            for (int i=0; i<topRow.Count(); i++)
            {
                var time = topRow.ElementAt(i);
                var distance = bottomRow.ElementAt(i);

                records.Add(new RaceRecord(time, distance));
            }

            return new ProblemData(records);
        }

        private record RaceRecord(int time, int distanceRecord);

        private record ProblemData(IEnumerable<RaceRecord> RaceRecords);
    }
}
