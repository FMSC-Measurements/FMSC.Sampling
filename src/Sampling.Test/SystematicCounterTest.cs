using FluentAssertions;
using FMSC.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sampling.Test
{
    public class SystematicCounter_Tests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        public void SystematicCounter_ctor_frequency(int frequency, bool inRange)
        {
            var counterMethod = SystematicCounter.CounterType.ON_RANDOM;

            var rand = new MersenneTwister((uint)Math.Abs(DateTime.Now.Ticks));

            Action a = () => new SystematicCounter(frequency, counterMethod, rand);

            if (inRange)
            {
                a.ShouldNotThrow();
            }
            else
            {
                a.ShouldThrow<ArgumentOutOfRangeException>();
            }

            Action a2 = () => new SystematicCounter(frequency, counterMethod, rand);

            if (inRange)
            {
                a2.ShouldNotThrow();
            }
            else
            {
                a2.ShouldThrow<ArgumentOutOfRangeException>();
            }
        }


        [Theory]
        [InlineData(10)]
        [InlineData(2)]
        public void Sample_OnRandom(int frequency)
        {
            var rand = new MersenneTwister((uint)Math.Abs(DateTime.Now.Ticks));
            var counter = new SystematicCounter(frequency, SystematicCounter.CounterType.ON_RANDOM, rand);

            var numRuns = 100;
            var expectedSamples = numRuns / frequency;
            var results = new bool[numRuns];


            var seenSamples = 0;
            for (int i = 0; i < numRuns; i++)
            {
                var result = counter.Next();
                if(result )
                {
                    seenSamples++;
                }
                results[i] = result;
            }

            //var seenSamples = results.Where(x => x == true).Count();
            seenSamples.Should().Be(expectedSamples);


        }
    }
}
