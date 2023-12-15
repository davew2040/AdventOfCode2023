using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day01
{
    internal class Day01part2 : IAocDay
    {
        private static Dictionary<string, int> valueMap = new()
        {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "zero", 0 }
        };

        public async Task Run()
        {
            var input = await ParseInput("Day01/real_input_part2.txt");

            int sum = 0;

            foreach (var line in input)
            {
                sum += GetLineValue(line);
            }

            Console.WriteLine(sum);
        }

        private int GetLineValue(string line)
        {
            int first = -1;

            for (int i=0; i<line.Length; i++)
            {
                int found = IndexContainsDigit(line, i);
                if (found != -1)
                {
                    first = found;
                    break;
                }
            }

            int last = -1;

            for (int i = line.Length-1; i >= 0; i--)
            {
                int found = IndexContainsDigit(line, i);
                if (found != -1)
                {
                    last = found;
                    break;
                }
            }

            return first * 10 + last;
        }

        private int IndexContainsDigit(string line, int index)
        {
            if (char.IsDigit(line[index]))
            {
                return line[index] - '0';
            }

            foreach (var key in valueMap.Keys)
            {
                if (MemoryExtensions.Equals(line.AsSpan(index, Math.Min(key.Length, line.Length - index)), key.AsSpan(), StringComparison.Ordinal))
                {
                    return valueMap[key];
                }
            }

            return -1;
        }

        private async Task<string[]> ParseInput(string filename)
        {
            var lines = await File.ReadAllLinesAsync(filename);

            return lines;
        }
    }
}
