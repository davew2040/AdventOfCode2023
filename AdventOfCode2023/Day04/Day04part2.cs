using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day04
{
    internal class Day04part2 : IAocDay
    {
        public async Task Run()
        {
            var parsed = await ParseInput("Day04/input.txt");

            var totalScore = GetGameTotal(parsed);

            Console.WriteLine(totalScore);
        }

        private async Task<Dictionary<int, Card>> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            return input.Select(ParseLine).ToDictionary(parsed => parsed.cardNumber, parsed => parsed.cardValues);
        }

        private (int cardNumber, Card cardValues) ParseLine(string line)
        {
            var split = line.Split(":");
            var numbersOnly = split[1].Split("|");

            var winning = numbersOnly[0];
            var currentHand = numbersOnly[1];

            var gameNumberRegex = new Regex(@"\d+");

            return (
                int.Parse(gameNumberRegex.Match(split[0]).Value),
                new Card(
                    WinningNumbers: winning.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToHashSet(),
                    CurrentHand: currentHand.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToHashSet()
            ));
        }

        private int GetGameTotal(Dictionary<int, Card> game)
        {
            Dictionary<int, int> copies = new();

            foreach (var kvp in game)
            {
                copies[kvp.Key] = 1;
            }

            int max = game.Max(g => g.Key);

            for (int i=game.Min(g => g.Key); i<=max; i++)
            {
                var card = game[i];
                var score = GetCardScore(card);

                for (int j=0; j<score; j++)
                {
                    int nextIndex = i + j + 1;
                    if (nextIndex <= max)
                    {
                        copies[nextIndex] = copies[nextIndex] + copies[i];
                    }
                }
            }

            return copies.Sum(c => c.Value);
        }

        private int GetCardScore(Card card)
        {
            return card.CurrentHand.Count(current => card.WinningNumbers.Contains(current));
        }

        private record Card(HashSet<int> WinningNumbers, HashSet<int> CurrentHand);
    }
}
