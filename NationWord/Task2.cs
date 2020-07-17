using System;
using System.Buffers;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Shouldly;
using Xunit;

namespace NationWord
{
    class Program
    {
        static void Main(string[] args)
        {
            var defaultConfig = DefaultConfig.Instance.With(ConfigOptions.DisableOptimizationsValidator);

            var _ = BenchmarkRunner.Run<BenchmarkTests>(defaultConfig);


        }
    }

    [DisassemblyDiagnoser(printSource: true)]
    [MemoryDiagnoser]
    public class BenchmarkTests
    {
        private Task2 _service;


        private string[] _input;

        public BenchmarkTests()
        {
            _service = new Task2();
            _input = new[] {"ezy", "jnx", "btp", "co", "dil", "ity", "abc", "mno", "qwerty", "a", "b", "c", "d", "ef",
                "gh", "ij", "klm"};
        }

        [Benchmark(Baseline = true)]
        public void Original()
        {
            //_service.Solve("ezy", "jnx", "btp");
            //_service.Solve("ez", "jn", "bt");
            //_service.Solve("ab", "cd", "ac");
            //_service.Solve("co", "dil", "ity");
            //_service.Solve("co", "dil", "ity", "abc", "mno");
            //_service.Solve("banana", "racecar", "potato");
            _service.Solve(_input);//, "nop", "qrt"); //, "uvwx");
        }

        [Benchmark]
        public void Solve3()
        {
            //_service.Solve3("ezy", "jnx", "btp");
            //_service.Solve3("ez", "jn", "bt");
            //_service.Solve3("ab", "cd", "ac");
            //_service.Solve3("co", "dil", "ity");
            //_service.Solve3("co", "dil", "ity", "abc", "mno");
            //_service.Solve3("banana", "racecar", "potato");
            _service.Solve3(_input);//, "nop", "qrt"); //, "uvwx");
        }

        [Benchmark]
        public void Solve5()
        {
            //_service.Solve5("ezy", "jnx", "btp");
            //_service.Solve5("ez", "jn", "bt");
            //_service.Solve5("ab", "cd", "ac");
            //_service.Solve5("co", "dil", "ity");
            //_service.Solve5("co", "dil", "ity", "abc", "mno");
            //_service.Solve5("banana", "racecar", "potato");
            _service.Solve5(_input);//, "nop", "qrt"); //, "uvwx");
        }

        //[Benchmark]
        //public void Solve5Short()
        //{
        //    _service.Solve5("ezy", "jnx", "btp");
        //    _service.Solve5("ez", "jn", "bt");
        //    _service.Solve5("ab", "cd", "ac");
        //    _service.Solve5("co", "dil", "ity");
        //    _service.Solve5("co", "dil", "ity", "abc", "mno");
        //    _service.Solve5("banana", "racecar", "potato");
        //}

    }


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

        private static bool Ok2(string str, ref int v)
        {
            v = 0;
            foreach (var c in str)
            {
                var bitNumber = c - 'a';
                if ((v & (1 << bitNumber)) != 0) return false;
                v |= 1 << bitNumber;
            }
            return true;
        }

        public int Solve4(params string[] input)
        {
            var bits = input.Where(Ok).Select(StringToBits).ToArray();

            var max = Combine(bits, 0, 0);

            if (max == 0) max = -1;

            return max;
        }

        private static int Combine(int[] bits, int maxSoFor, int accum = 0)
        {
            var possibleBits = bits.Where(possible => !Overlaps(accum, possible));
            if (possibleBits.Count() == 0)
            {
                maxSoFor = System.Math.Max(maxSoFor, NumberOfSetBits(accum));
            }

            foreach (var possible in possibleBits)
            {
                var bitsWithOut = bits.Where(b => b != possible).ToArray();
                maxSoFor = Combine(bitsWithOut, maxSoFor, accum | possible);
            }
            return maxSoFor;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Overlaps(int lhs, int rhs) => (lhs & rhs) != 0;

        static int NumberOfSetBits(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }

        public int Solve3(params string[] input)
        {
            var numberOfInputs = 0;
            var arrayPool = ArrayPool<int>.Shared;
            var bits = arrayPool.Rent(input.Length);
            try
            {
                for (var i = 0; i < input.Length; i++)
                {
                    if (Ok2(input[i], ref bits[numberOfInputs]))
                    {
                        numberOfInputs++;
                    }
                }

                //var bits = input.Where(Ok).Select(StringToBits).ToArray();

                var max = -1;
                for (var i = 0; i < numberOfInputs; i++)
                {
                    for (var j = i; j < numberOfInputs; j++)
                    {
                        var combined = 0;
                        for (var k = i; k <= j; k++)
                        {
                            if (!Overlaps(combined, bits[k]))
                            {
                                combined = combined + bits[k];

                                max = System.Math.Max(NumberOfSetBits(combined), max);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                return max;
            }
            finally
            {
                arrayPool.Return(bits);
            }
        }


        public int Solve5(params string[] input)
        {
            var numberOfInputs = 0;
            Span<int> bits = stackalloc int[input.Length];
            Span<int> numberOfBitsSet = stackalloc int[input.Length];
            var max = -1;

            for (var i = 0; i < input.Length; i++)
            {
                if (Ok2(input[i], ref bits[numberOfInputs]))
                {
                    numberOfBitsSet[numberOfInputs] = NumberOfSetBits(bits[numberOfInputs]);

                    if (numberOfBitsSet[numberOfInputs] > max) max = numberOfBitsSet[numberOfInputs];

                    numberOfInputs++;
                }
            }

            for (var i = 0; i < numberOfInputs; i++)
            {
                for (var j = i; j < numberOfInputs; j++)
                {
                    var combined = bits[i];
                    var set = numberOfBitsSet[i];

                    for (var k = i+1; k <= j; k++)
                    {
                        var b = bits[k];
                        if ((combined & b) != 0)
                        {
                            break;
                        }

                        combined = combined | b;
                        set = set + numberOfBitsSet[k];

                        if (set > max) max = set;
                    }
                }
            }
            return max;
        }

        public int Solve(params string[] input)
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
            var invalidCombinations = input.SelectMany(a => input.Select(b => new { A = a, B = b }))
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
        [InlineData(5, "co", "dil", "ity")]
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

        [Theory]
        [InlineData(9, "ezy", "jnx", "btp")]
        [InlineData(6, "ez", "jn", "bt")]
        [InlineData(4, "ab", "cd", "ac")]
        [InlineData(5, "co", "dil", "ity")]
        [InlineData(9, "co", "dil", "ity", "abc", "mno")]
        [InlineData(-1, "banana", "racecar", "potato")]
        public void ShouldReturnTheCorrectOutput5(int expectedOutput, params string[] input)
        {
            var sut = new Task2();
            var output = sut.Solve5(input);

            output.ShouldBe(expectedOutput);
        }
    }

}
