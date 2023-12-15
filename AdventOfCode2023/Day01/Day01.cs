using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day01
{
    internal class Day02 : IAocDay
    {
        public async Task Run()
        {
            var input = await ParseInput("Day01/real_input.txt");

            int sum = 0;

            foreach (var line in input)
            {
                sum += GetLineValue(line);
            }

            Console.WriteLine(sum);
        }

        private int GetLineValue(string line)
        {
            var nums = line.Where(c => char.IsDigit(c));

            return (int)(nums.Last() - '0') + (int)(nums.First() - '0') * 10;
        }

        private async Task<string[]> ParseInput(string filename)
        {
            var lines = await File.ReadAllLinesAsync(filename);

            return lines;
        }
    }
}
