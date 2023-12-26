using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day06
{
    internal class Day06part2 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day06/input.txt");

            //var test = GetBetterHoldTimes(new RaceRecord(7, 9));

            var betterTimes = GetBetterHoldTimes(parsed.RaceRecords.First());

            Console.WriteLine(betterTimes);
        }

        private long GetBetterHoldTimes(RaceRecord raceRecord)
        {
            long betterCount = 0;

            List<long> distances = new();

            for (int i=1; i<raceRecord.time; i++) 
            {
                distances.Add(TotalDistance(i, raceRecord.time));
            }

            return distances.Where(d => d > raceRecord.distanceRecord).Count();
        }

        private bool BeatsBestDistance(long holdTime, long maxTime, long bestDistance)
        {
            long totalDistance = holdTime * (maxTime - holdTime);

            return bestDistance < TotalDistance(holdTime, maxTime);
        }


        private long TotalDistance(long holdTime, long maxTime)
        {
            return holdTime * (maxTime - holdTime);
        }

        private async Task<ProblemData> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            var topRow = input[0].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Aggregate(string.Empty, (current, next) => current + next);
            var bottomRow = input[1].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Aggregate(string.Empty, (current, next) => current + next);

            var topRowParsed = long.Parse(topRow);
            var bottomRowParsed = long.Parse(bottomRow);

            List<RaceRecord> records = new();

            records.Add(new RaceRecord(topRowParsed, bottomRowParsed));

            return new ProblemData(records);
        }

        private record RaceRecord(long time, long distanceRecord);

        private record ProblemData(IEnumerable<RaceRecord> RaceRecords);
    }
}
