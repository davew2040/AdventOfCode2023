using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day05
{
    internal class Day05 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day05/input.txt");

            var mapped = parsed.Seeds.Select(seed => MapChain(seed, parsed));

            Console.WriteLine(mapped.Min());
        }

        private async Task<ProblemData> ParseInput(string filename)
        {
            var input = await File.ReadAllTextAsync(filename);

            IEnumerable<long> finalSeeds = Enumerable.Empty<long>();
            Dictionary<string, IEnumerable<ResourceMapRange>> finalRanges = new();

            var splitByNewline = input.Split("\r\n\r\n").Select(newlines => newlines.Split("\n"));

            var seedsRegex = new Regex(@"seeds: ");
            var mapRegex = new Regex(@"(?<source>[a-z]+)\-to\-(?<dest>[a-z]+)");

            foreach (var split in splitByNewline)
            {
                if (seedsRegex.IsMatch(split[0]))
                {
                    finalSeeds = split[0].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
                    continue;
                }

                var match = mapRegex.Match(split[0]);

                if (match.Success)
                {
                    var from = resourceTypeLabels[match.Groups["source"].Value];
                    var to = resourceTypeLabels[match.Groups["dest"].Value];

                    var ranges = split[1..].Select(ParseRange).ToList();

                    finalRanges[GetRangeKey(from, to)] = ranges;
                }
            }

            return new ProblemData(finalSeeds, finalRanges);
        }

        private long MapChain(long value, ProblemData problem)
        {
            //Seed 79, soil 81, fertilizer 81, water 81, light 74, temperature 78, humidity 78, location 82.

            value = MapValue(value, ResourceType.Seed, ResourceType.Soil, problem.ResourceMaps);
            value = MapValue(value, ResourceType.Soil, ResourceType.Fertilizer, problem.ResourceMaps);
            value = MapValue(value, ResourceType.Fertilizer, ResourceType.Water, problem.ResourceMaps);
            value = MapValue(value, ResourceType.Water, ResourceType.Light, problem.ResourceMaps);
            value = MapValue(value, ResourceType.Light, ResourceType.Temperature, problem.ResourceMaps);
            value = MapValue(value, ResourceType.Temperature, ResourceType.Humidity, problem.ResourceMaps);
            value = MapValue(value, ResourceType.Humidity, ResourceType.Location, problem.ResourceMaps);

            return value;
        }

        private string GetRangeKey(ResourceType source, ResourceType dest)
        {
            return $"{source}:{dest}";
        }

        private long MapValue(long value, ResourceType from, ResourceType to, Dictionary<string, IEnumerable<ResourceMapRange>> allMaps)
        {
            var key = GetRangeKey(from, to);
            var maps = allMaps[key];

            foreach (var map in maps)
            {
                if (IsInRange(value, map.Min, map.Max))
                {
                    return value + map.Offset;
                }
            }

            return value;
        }

        private bool IsInRange(long
            target, long min, long max)
        {
            return target >= min && target <= max;
        }

        private ResourceMapRange ParseRange(string s)
        {
            var split = s.Split(" ");
            var destRangeStart = long.Parse(split[0]);
            var sourceRangeStart = long.Parse(split[1]);
            var size = long.Parse(split[2]);

            return new ResourceMapRange(sourceRangeStart, sourceRangeStart + size - 1, destRangeStart - sourceRangeStart);
        }

        private enum ResourceType
        {
            Seed,
            Soil,
            Fertilizer,
            Water,
            Light,
            Temperature,
            Humidity,
            Location
        }

        private static Dictionary<string, ResourceType> resourceTypeLabels = new()
        {
            { "seed", ResourceType.Seed },
            { "soil", ResourceType.Soil },
            { "fertilizer", ResourceType.Fertilizer },
            { "water", ResourceType.Water },
            { "light", ResourceType.Light },
            { "temperature", ResourceType.Temperature },
            { "humidity", ResourceType.Humidity },
            { "location", ResourceType.Location }
        };

        private record ResourceMapRange(long Min, long Max, long Offset);

        private record ProblemData(IEnumerable<long> Seeds, Dictionary<string, IEnumerable<ResourceMapRange>> ResourceMaps);
    }
}
