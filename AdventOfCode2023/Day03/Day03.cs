using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day03
{
    internal class Day03 : IAocDay
    {
        public async Task Run()
        {
            var input = await ParseInput("Day03/input.txt");

            var numsWithAdjacent = FindNumbers(input);

            Dictionary<GridIndex, List<int>> bySymbol = new();

            foreach (var numWithAdjacent in numsWithAdjacent)
            {
                foreach (var adjacent in numWithAdjacent.adjacent)
                {
                    if (!bySymbol.ContainsKey(adjacent))
                    {
                        bySymbol[adjacent] = new List<int>();
                    }

                    bySymbol[adjacent].Add(numWithAdjacent.value);
                }
            }

            var gearRatios = bySymbol.Where(kvp => kvp.Value.Count == 2);

            var sum = gearRatios.Sum(ratio => ratio.Value.First() * ratio.Value.Last());

            Console.WriteLine(sum);
        }

        private IEnumerable<(int value, HashSet<GridIndex> adjacent)> FindNumbers(char[,] schematic)
        {
            var marked = new bool[schematic.GetLength(0), schematic.GetLength(1)];
            var nums = new List<(int value, HashSet<GridIndex> adjacent)>();

            for (int r = 0; r < schematic.GetLength(0); r++)
            {
                for (int c = 0; c < schematic.GetLength(1); c++) 
                {
                    if (marked[r,c] == false)
                    {
                        if (char.IsDigit(schematic[r,c]))
                        {
                            var run = FindRun(r, c, schematic, marked);
                            nums.Add((run.number, run.adjacent));
                        }

                        marked[r,c] = true;
                    }
                }
            }

            return nums;
        }
        
        private (int number, HashSet<GridIndex> adjacent) FindRun(int r, int c, char[,] schematic, bool[,] marked)
        {
            var buffer = new StringBuilder();

            HashSet<GridIndex> adjacent = new();

            while (c < schematic.GetLength(1))
            {
                if (char.IsDigit(schematic[r,c]))
                {
                    buffer.Append(schematic[r,c]);

                    var nextAdjacents = AdjacentSymbols(r, c, schematic);

                    foreach (var nextAdjacent in nextAdjacents)
                    {
                        if (!adjacent.Contains(nextAdjacent))
                        {
                            adjacent.Add(nextAdjacent);
                        }
                    }
                    marked[r,c] = true;
                }
                else
                {
                    break;
                }

                c++;
            }


            return (int.Parse(buffer.ToString()), adjacent);
        }

        private HashSet<GridIndex> AdjacentSymbols(int r, int c, char[,] schematic)
        {
            var adjacent = GetValidAdjacent(r, c, schematic);

            return adjacent.Where(a => IsSymbol(schematic[a.Row, a.Column])).ToHashSet();
        }

        private bool IsSymbol(char c) => c == '*';

        private IEnumerable<GridIndex> GetValidAdjacent<T>(int r, int c, T[,] grid)
        {
            var adjacent = new List<GridIndex>();

            for (int rOffset=-1; rOffset<=1; rOffset++)
            {
                for (int cOffset=-1; cOffset<=1; cOffset++)
                {
                    if (!(rOffset == 0 && cOffset == 0))
                    {
                        adjacent.Add(new GridIndex(r+rOffset, c+cOffset));
                    }
                }
            }

            return adjacent.Where(a => IsValidIndex(a.Row, a.Column, grid));
        }

        private bool IsValidIndex<T>(int r, int c, T[,] schematic)
        {
            return r >= 0 && r < schematic.GetLength(0)
                && c >= 0 && c < schematic.GetLength(1);
        }

        private async Task<char[,]> ParseInput(string filename)
        {
            var input = await File.ReadAllLinesAsync(filename);

            var maxLength = input.Max(line => line.Length);

            var result = new char[input.Length, maxLength];

            int r = 0;
            foreach (var line in input)
            {
                for (int c=0; c<line.Length; c++)
                {
                    result[r,c] = line[c];
                }

                r++;
            }

            return result; 
        }

        private record GridIndex(int Row, int Column)
        {
            public override int GetHashCode()
            {
                return (Row, Column).GetHashCode();
            }
        }
    }
}
