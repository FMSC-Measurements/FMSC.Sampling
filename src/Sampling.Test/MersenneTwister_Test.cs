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
    public class MersenneTwister_Test
    {
        [Theory]
        [InlineData(uint.MinValue)]
        [InlineData(uint.MaxValue)]
        public void MersenneTwister(uint seed)
        {
            var rand = new MersenneTwister(seed);

            TestMersenneTwister(rand);
        }

        [Fact]
        public void MersenneTwister_fusk()
        {
            for(int i = 0; i < 1000; i++)
            {
                var mt = new MersenneTwister((uint)Math.Abs(DateTime.Now.Ticks));
                TestMersenneTwister(mt);
            }
        }


        void TestMersenneTwister(Random random)
        {
            for(int i = 0; i < 10000; i++)
            {
                random.Invoking(x => x.NextDouble()).ShouldNotThrow();
            }
        }

        [Fact]
        public void Next_max()
        {
            var mt = new MersenneTwister((uint)Math.Abs(DateTime.Now.Ticks));

            for (int i = 0; i < 1000; i++)
            {
                var result = mt.Next(2);
                result.Should().BeLessThan(2);
                result.Should().BeGreaterOrEqualTo(0);
            }
        }
    }
}
