using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day04
{
    internal class Day04 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day04/input.txt");

            var sum = parsed.Sum(GetCardScore);
        }

        private async Task<IEnumerable<Card>> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            return input.Select(ParseLine).ToList();
        }

        private Card ParseLine(string line)
        {
            var numbersOnly = line.Split(":")[1].Split("|");

            var winning = numbersOnly[0];
            var currentHand = numbersOnly[1];

            return new Card(
                WinningNumbers: winning.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToHashSet(),
                CurrentHand: currentHand.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToHashSet()
            );
        }

        private int GetCardScore(Card card)
        {
            int score = 0;

            foreach (var current in card.CurrentHand)
            {
                if (card.WinningNumbers.Contains(current))
                {
                    if (score == 0)
                    {
                        score = 1;
                    }
                    else
                    {
                        score *= 2;
                    }
                }
            }

            return score;
        }

        private record Card(HashSet<int> WinningNumbers, HashSet<int> CurrentHand);
    }
}
