using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace NationWord
{
    public class Task1
    {
        //private static int SolvePart1(string str)
        //{
        //    var run = 0;
        //    var result = 0;
        //    foreach (var c in str + '$')
        //    {
        //        if (c == 'a')
        //        {
        //            run++;
        //        }
        //        else
        //        {
        //            if (run == 0)
        //                result += 2;
        //            else if (run == 1)
        //                result += 1;
        //            else if (run > 2)
        //                return -1;

        //            run = 0;
        //        }

        //    }

        //    return result;
        //}

        public int Task1a(string input)
        {
            if (input.Contains("aaa")) return -1;
            var doubles = input.Contains("aa");
            var singles = input.Contains("a");
            var nonAs = input.Count(c => c != 'a');
            return 0;
        }

        public int Task1b(string input)
        {
            if (input.Contains("aaa")) return -1;
            var numberOfAs = 0;
            var counter = 2;
            foreach (var c in input)
            {
                if (c == 'a')
                {
                    --counter;
                }
                else
                {
                    numberOfAs += counter;
                    counter = 2;
                }
            }

            return numberOfAs + counter;
        }

        public int Solve(ReadOnlySpan<char> input)
        {
            var numberOfAs = 0;
            var counter = 2;
            foreach (var c in input)
            {
                if (c == 'a')
                {
                    if(--counter == -1) return -1;
                }
                else
                {
                    numberOfAs  +=  counter;
                    counter = 2;
                }
            }

            return numberOfAs + counter;

            //if (input.Count(x => x == 'a') >= 3)
            //{
            //    return -1;
            //}

            //var numberOfNonAs = input.Replace("a", "").Length;
            //if (numberOfNonAs > 0)
            //{
            //    return 2 + (numberOfNonAs * 2);
            //}


            //if (input.Length > 0 && !input.Contains('a'))
            //    return 4;

            //return input.Length switch
            //{
            //    0 => 2,
            //    1 => 1,
            //    2 => 0,
            //    3 => -1,
            //    _ => -2
            //};
        }
    }
    public class Task1Tests
    {
        [Theory]
        [InlineData("", 2)]
        [InlineData("a", 1)]
        [InlineData("aa", 0)]
        [InlineData("aaa", -1)]
        [InlineData("b", 4)] //aabaa
        [InlineData("abaa", 1)] //aabaa
        [InlineData("bb", 6)] //aabaabaa
        [InlineData("dog", 8)] //aadaaoaagaa
        public void ShouldReturnCorrectAs(string input, int expectedOutput)
        {
            var nationWord = new Task1();
            var task1 = nationWord.Solve(input);

            task1.ShouldBe(expectedOutput);
        }

        [Theory]
        [InlineData("aaab")]
        [InlineData("aaaab")]
        [InlineData("baaaa")]
        [InlineData("baaa")]
        [InlineData("aaaabaaaa")]
        [InlineData("aaabaaa")]
        public void ShouldReturnMinusOneIfThreeOrMoreAsAnywhereInTheString(string input)
        {
            var expectedOutput = -1;
            var nationWord = new Task1();
            
            var task1 = nationWord.Solve(input);

            task1.ShouldBe(expectedOutput);
        }
    }
}
