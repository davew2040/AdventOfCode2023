﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day07
{
    internal class Day07 : IAocDay
    {
        private enum HandType
        {
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard
        }

        private static Dictionary<HandType, int> HandPriorities = new()
        {
            { HandType.FiveOfAKind, 7 },
            { HandType.FourOfAKind, 6 },
            { HandType.FullHouse, 5 },
            { HandType.ThreeOfAKind, 4 },
            { HandType.TwoPair, 3 },
            { HandType.OnePair, 2 },
            { HandType.HighCard, 1 }
        };

        private static Dictionary<char, int> CardPriorities = new()
        {
            { 'A', 14 },
            { 'K', 13 },
            { 'Q', 12 },
            { 'J', 11 },
            { 'T', 10 },
            { '9', 9 },
            { '8', 8 },
            { '7', 7 },
            { '6', 6 },
            { '5', 5 },
            { '4', 4 },
            { '3', 3 },
            { '2', 2 },
        };

        public async Task Run()
        {
            var parsed = await ParseInput("Day07/input.txt");

            var score = GetHandsScore(parsed.hands);
        }

        private long GetHandsScore(IEnumerable<Hand> hands)
        {
            var handComparer = new HandComparer();
            var ordered = hands.OrderBy(h => h, handComparer);

            var withValues = ordered.Select((item, index) =>
            {
                return (index + 1) * item.bid;
            });

            return withValues.Sum();
        }

        private static HandType GetHandType(Hand hand)
        {
            Dictionary<char, int> cardCount = new();

            foreach (var c in hand.cards)
            {
                if (!cardCount.ContainsKey(c))
                {
                    cardCount[c] = 0;
                }

                cardCount[c]++;
            }

            var orderedByCount = cardCount.OrderByDescending(c => c.Value);

            if (orderedByCount.First().Value == 5)
            {
                return HandType.FiveOfAKind;
            }
            else if (orderedByCount.First().Value == 4)
            {
                return HandType.FourOfAKind;
            }
            else if (orderedByCount.First().Value == 3 && orderedByCount.ElementAt(1).Value == 2)
            {
                return HandType.FullHouse;
            }
            else if (orderedByCount.First().Value == 3)
            {
                return HandType.ThreeOfAKind;
            }
            else if (orderedByCount.ElementAt(0).Value == 2 && orderedByCount.ElementAt(1).Value == 2)
            {
                return HandType.TwoPair;
            }
            else if (orderedByCount.ElementAt(0).Value == 2)
            {
                return HandType.OnePair;
            }
            else
            {
                return HandType.HighCard;
            }
        }

        private async Task<ProblemData> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            return new ProblemData(
                input.Select(line =>
                {
                    var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    var cards = split[0];
                    var bid = int.Parse(split[1]);

                    return new Hand(cards, bid);
                })
            );
        }

        private record Hand(IEnumerable<char> cards, int bid);

        private record ProblemData(IEnumerable<Hand> hands);

        private class HandComparer : Comparer<Hand>
        {
            // compares by length, height, and width.
            public override int Compare(Hand a, Hand b)
            {
                var aType = GetHandType(a);
                var bType = GetHandType(b);

                if (aType != bType)
                {
                    return HandPriorities[aType].CompareTo(HandPriorities[bType]);
                }
                else
                {
                    for (int i=0; i<a.cards.Count(); i++)
                    {
                        var aCard = a.cards.ElementAt(i);
                        var bCard = b.cards.ElementAt(i);

                        var priorityDiff = CardPriorities[aCard] - CardPriorities[bCard];

                        if (priorityDiff != 0)
                        {
                            return priorityDiff;
                        }
                    }
                }

                return 0;
            }
        }
    }
}
