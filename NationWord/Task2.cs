using System.Linq;
using System.Collections.Generic;
using System.Text;
using Shouldly;
using Xunit;

namespace NationWord
{
    public class Task2
    {
        private static int StringToBits(string str)
        {
            var v = 0;
            foreach (var c in str)
            {
                var bitNumber = c - 'a';
                v |= 1 << bitNumber;
            }
            return v;
        }

        public int Solve4(string[] input)
        {
            var bits = input.Where(Ok).Select(StringToBits).ToArray();

            var results = Combine(bits, new List<int>()).ToArray();

            var max = results.Max(NumberOfSetBits);

            if (max == 0) max = -1;

            return max;
        }

        private static List<int> Combine(int[] bits, List<int> results, int accum = 0)
        {
            var possibleBits = bits.Where(possible => !Overlaps(accum, possible));
            if (possibleBits.Count() == 0) results.Add(accum);  //No more to add.

            foreach (var possible in possibleBits)
            {
                var bitsWithOut = bits.Where(b => b != possible).ToArray();
                Combine(bitsWithOut, results, accum | possible);
            }
            return results;
        }

        private static bool Overlaps(int lhs, int rhs) => (lhs & rhs) != 0;

        static int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public int Solve3(string[] input)
        {
            var bits = input.Where(Ok).Select(StringToBits).ToArray();

            var max = -1;
            for (var i = 0; i < bits.Length; i++)
            {
                for (var j = i; j < bits.Length; j++)
                {
                    var combined = 0;
                    for (var k = i; k <= j; k++)
                    {
                        if (!Overlaps(combined, bits[k]))
                        {
                            combined = combined + bits[k];

                            max = System.Math.Max(NumberOfSetBits(combined) , max);
                        }
                    }
                }
            }
            return max;
        }


        public int Solve(string[] input)
        {
            var max = -1;
            for (var i = 0; i < input.Length; i++)
            {
                for (var j = i; j < input.Length; j++)
                {
                    var combined = "";
                    for (var k = i; k <= j; k++)
                    {
                        combined = combined + input[k];
                        
                        if (combined.Length > max && Ok(combined))
                        {
                            max = combined.Length;
                        }
                    }
                }
            }
            return max;
        }

        public int Solve2(string[] input)
        {
            var invalidCombinations = input.SelectMany(a => input.Select(b => new {A = a, B = b}))
                .Where(x => !Ok(x.A + x.B))
                .ToList();

            return invalidCombinations.SelectMany(x => 
                    input.Where(y => y != x.A)
                        .Concat(input.Where(y => y != x.B)).Distinct()
            )
            .GroupBy(x => x)
            .Count();
        }
        public bool Ok(string input) => input.Length - input.Distinct().Count() == 0;
    }


    public class Task2Tests
    {
        [Theory]
        [InlineData(9, "ezy", "jnx", "btp")]
        [InlineData(6, "ez", "jn", "bt")]
        [InlineData(4, "ab", "cd", "ac")]
        [InlineData(5, "co","dil","ity")]
        [InlineData(9, "co", "dil", "ity", "abc", "mno")]
        [InlineData(-1, "banana", "racecar", "potato")]
        public void ShouldReturnTheCorrectOutput1(int expectedOutput, params string[] input)
        {
            var sut = new Task2();
            var output = sut.Solve(input);

            output.ShouldBe(expectedOutput);
        }

        //[Theory]
        //[InlineData(9, "ezy", "jnx", "btp")]
        //[InlineData(6, "ez", "jn", "bt")]
        //[InlineData(4, "ab", "cd", "ac")]
        //[InlineData(5, "co", "dil", "ity")]
        //[InlineData(9, "co", "dil", "ity", "abc", "mno")]
        //[InlineData(-1, "banana", "racecar", "potato")]
        //public void ShouldReturnTheCorrectOutput2(int expectedOutput, params string[] input)
        //{
        //    var sut = new Task2();
        //    var output = sut.Solve2(input);

        //    output.ShouldBe(expectedOutput);
        //}

        [Theory]
        [InlineData(9, "ezy", "jnx", "btp")]
        [InlineData(6, "ez", "jn", "bt")]
        [InlineData(4, "ab", "cd", "ac")]
        [InlineData(5, "co", "dil", "ity")]
        [InlineData(9, "co", "dil", "ity", "abc", "mno")]
        [InlineData(-1, "banana", "racecar", "potato")]
        public void ShouldReturnTheCorrectOutput3(int expectedOutput, params string[] input)
        {
            var sut = new Task2();
            var output = sut.Solve3(input);

            output.ShouldBe(expectedOutput);
        }

        [Theory]
        [InlineData(9, "ezy", "jnx", "btp")]
        [InlineData(6, "ez", "jn", "bt")]
        [InlineData(4, "ab", "cd", "ac")]
        [InlineData(5, "co", "dil", "ity")]
        [InlineData(9, "co", "dil", "ity", "abc", "mno")]
        [InlineData(-1, "banana", "racecar", "potato")]
        public void ShouldReturnTheCorrectOutput4(int expectedOutput, params string[] input)
        {
            var sut = new Task2();
            var output = sut.Solve4(input);

            output.ShouldBe(expectedOutput);
        }
    }

}
