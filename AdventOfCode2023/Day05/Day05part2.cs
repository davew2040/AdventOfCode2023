using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day05
{
    internal class Day05part2 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day05/input.txt");

            //var test = SplitRange(new Range(1, 10), new List<ResourceMapRange>()
            //{
            //    new ResourceMapRange(-10, 4, 100)
            //});

            var mapped = parsed.Seeds.Select(seed => MapChain(seed, parsed)).ToList();

            var lowestPerRange = mapped.Select(m => m.Min(t => t.Min));
            long lowest = lowestPerRange.Min(); 
            Console.WriteLine(lowest);
        }

        private async Task<ProblemData> ParseInput(string filename)
        {
            var input = await File.ReadAllTextAsync(filename);

            IEnumerable<Range> finalSeeds = Enumerable.Empty<Range>();
            Dictionary<string, IEnumerable<ResourceMapRange>> finalRanges = new();

            var splitByNewline = input.Split("\r\n\r\n").Select(newlines => newlines.Split("\n"));

            var seedsRegex = new Regex(@"seeds: ");
            var mapRegex = new Regex(@"(?<source>[a-z]+)\-to\-(?<dest>[a-z]+)");

            foreach (var split in splitByNewline)
            {
                if (seedsRegex.IsMatch(split[0]))
                {
                    var pairs = split[0].Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var seedRanges = new List<Range>();

                    for (int i=0; i<pairs.Length; i+=2)
                    {
                        long start = long.Parse(pairs[i]);
                        long size = long.Parse(pairs[i + 1]);
                        seedRanges.Add(new Range(start, start+size-1));
                    }

                    finalSeeds = seedRanges;
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

        private IEnumerable<Range> MapChain(Range range, ProblemData problem)
        {
            var current = new List<Range>() { range } as IEnumerable<Range>;

            current = MapRanges(current, ResourceType.Seed, ResourceType.Soil, problem.ResourceMaps);
            current = MapRanges(current, ResourceType.Soil, ResourceType.Fertilizer, problem.ResourceMaps);
            current = MapRanges(current, ResourceType.Fertilizer, ResourceType.Water, problem.ResourceMaps);
            current = MapRanges(current, ResourceType.Water, ResourceType.Light, problem.ResourceMaps);
            current = MapRanges(current, ResourceType.Light, ResourceType.Temperature, problem.ResourceMaps);
            current = MapRanges(current, ResourceType.Temperature, ResourceType.Humidity, problem.ResourceMaps);
            current = MapRanges(current, ResourceType.Humidity, ResourceType.Location, problem.ResourceMaps);

            return current;
        }

        private string GetRangeKey(ResourceType source, ResourceType dest)
        {
            return $"{source}:{dest}";
        }

        private IEnumerable<Range> MapRanges(IEnumerable<Range> sourceRanges, ResourceType from, ResourceType to, Dictionary<string, IEnumerable<ResourceMapRange>> allMaps)
        {
            var key = GetRangeKey(from, to);
            var mappings = allMaps[key];
            var newRanges = new List<Range>();

            foreach (var sourceRange in sourceRanges)
            {
                var split = SplitRange(sourceRange, mappings);
                newRanges.AddRange(split);
            }

            return newRanges;
        }

        private IEnumerable<Range> SplitRange(Range range, IEnumerable<ResourceMapRange> maps)
        {
            List<Range> newRanges = new();
            Range currentRange = new Range(range.Min, range.Max);
            var orderedMaps = maps.OrderBy(m => m.Min).ToList();

            foreach (var map in orderedMaps)
            {
                if (!HasIntersection(new Range(map.Min, map.Max), currentRange))
                {
                    continue;
                }

                if (currentRange.GetSize() <= 0)
                {
                    return newRanges;
                }

                Range before = new Range(currentRange.Min, map.Min - 1);
                if (before.GetSize() > 0)
                {
                    newRanges.Add(before);
                }

                Range middle = new Range(
                    Math.Max(currentRange.Min, map.Min),
                    Math.Min(currentRange.Max, map.Max)
                );
                if (middle.GetSize() > 0)
                {
                    newRanges.Add(new Range(middle.Min + map.Offset, middle.Max + map.Offset));
                }

                Range after = new Range(Math.Max(map.Max+1, currentRange.Min), currentRange.Max);
                currentRange = after;
            }

            if (currentRange.GetSize() > 0)
            {
                newRanges.Add(currentRange);
            }

            return newRanges;
        }

        private bool HasIntersection(Range one, Range two)
        {
            return !(two.Min > one.Max || two.Max < one.Min);
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

        private record Range(long Min, long Max)
        {
            public long GetSize() => Max - Min + 1;
        }

        private record ProblemData(IEnumerable<Range> Seeds, Dictionary<string, IEnumerable<ResourceMapRange>> ResourceMaps);
        
        private record SeedRange(long Start, long Size);

    }
}
