using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day02
{
    internal class Day02part2 : IAocDay
    {
        public enum CubeColor
        {
            Red,
            Green,
            Blue
        }

        private static Dictionary<string, CubeColor> ColorLabelToEnum = new()
        {
            { "green", CubeColor.Green },
            { "blue", CubeColor.Blue },
            { "red", CubeColor.Red },
        };

        private static Dictionary<CubeColor, int> BagTotals = new()
        {
            { CubeColor.Red, 12 },
            { CubeColor.Green, 13 },
            { CubeColor.Blue, 14 }
        };

        public async Task Run()
        {
            var input = await ParseInput("Day02/input.txt");

            var fewestCounts = input.Select(i => FewestCounts(i.Value));
            var powerSets = fewestCounts.Select(counts => counts[CubeColor.Red] * counts[CubeColor.Green] * counts[CubeColor.Blue]);

            var sum = powerSets.Sum();

            Console.WriteLine(sum);
        }

        private async Task<Dictionary<int, IEnumerable<Dictionary<CubeColor, int>>>> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            var parsed = input.Select(ParseLine).ToDictionary(p => p.gameNumber, p => p.counts);

            return parsed;
        }

        private (int gameNumber, IEnumerable<Dictionary<CubeColor, int>> counts) ParseLine(string line)
        {
            var colonSplit = line.Split(":");
            var gameNumber = int.Parse(colonSplit[0].Split(" ")[1]);

            var semiColonSplits = colonSplit[1].Split(";");

            return (gameNumber, semiColonSplits.Select(ParseCounts).ToList());
        }

        private Dictionary<CubeColor, int> ParseCounts(string counts)
        {
            var segments = counts.Split(",");

            return segments.Select(ParseSingleCount).ToList().ToDictionary(s => s.color, s => s.count);
        }

        private (CubeColor color, int count) ParseSingleCount(string singleCount)
        {
            var regex = new Regex(@"(?<count>\d+)\s+(?<color>[a-z]+)");

            var match = regex.Match(singleCount);

            return (ColorLabelToEnum[match.Groups["color"].Value], int.Parse(match.Groups["count"].Value));
        }

        private Dictionary<CubeColor, int> SumRounds(IEnumerable<Dictionary<CubeColor, int>> rounds)
        {
            Dictionary<CubeColor, int> totals = new()
            {
                { CubeColor.Red, 0 },
                { CubeColor.Green, 0 },
                { CubeColor.Blue, 0 }
            };

            foreach (var round in rounds)
            {
                foreach (var kvp in round)
                {
                    totals[kvp.Key] += kvp.Value;
                }
            }

            return totals;
        }

        private Dictionary<CubeColor, int> FewestCounts(IEnumerable<Dictionary<CubeColor, int>> rounds)
        {
            Dictionary<CubeColor, int> fewest = new()
            {
                { CubeColor.Red, 0 },
                { CubeColor.Green, 0 },
                { CubeColor.Blue, 0 }
            };

            foreach (var round in rounds)
            {
                foreach (var roundKvp in round)
                {
                    if (roundKvp.Value > fewest[roundKvp.Key])
                    {
                        fewest[roundKvp.Key] = roundKvp.Value;
                    }
                }
            }

            return fewest;
        }
    }
}
