using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day07
{
    internal class Day07 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day06/sample.txt");

            //Console.WriteLine(multiplied);
        }

        private async Task<ProblemData> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

             
        }

        private record Hand(IEnumerable<char> cards, int bid);

        private record ProblemData(IEnumerable<Hand> hands);
    }
}
