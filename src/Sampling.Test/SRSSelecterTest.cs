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
    public class SRSSelecterTest
    {

        [Theory]
        [InlineData(15, 0)]
        public void Sample_Test(int freq, int iFreq)
        {
            var margin = 1;
            var iMargin = margin;

            var selector = new SRSSelecter(freq, iFreq);

            var numRuns = 100_000;

            var numSamples = 0;
            var numISamples = 0;
            for(int i = 0; i < numRuns; i++)
            {
                var result = selector.Sample();
                if(result == 'M')
                { numSamples++; }
                else if (result == 'I')
                { numISamples++; }
            }

            var expectedNumSamples = numRuns / freq;
            var expectedNumISamples = (iFreq > 0 ) ? numRuns / (freq * iFreq) : 0;

            expectedNumSamples = expectedNumSamples - expectedNumISamples;

            var actualFreq = numRuns / numSamples;
            var actualIFreq = (numISamples > 0) ? numRuns / numISamples : 0;

            actualFreq.Should().BeInRange(freq - margin, freq + margin);

            if(iFreq > 0)
            {
                actualIFreq.Should().BeInRange(iFreq - iMargin, iFreq - iMargin);
            }
        }
    }
}
