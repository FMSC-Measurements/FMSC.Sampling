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
    public class ThreePSelecterTest
    {
        [Fact]
        public void TestThreePSelecter()
        {
            decimal tolarance = .04m;

            int iFrequency = 5;
            int aveKpi = 20;
            int freq = 500;
            int popSize = 1_000_000;
            int numSamples = popSize / freq;
            //int numIsamples = (iFrequency > 0) ? numSamples / iFrequency : 0; // same as other implementation but less consistant with how i test other methods
            int numIsamples = (iFrequency > 0) ? popSize / (iFrequency * freq) : 0;

            int popVol = aveKpi * popSize;
            int kz = popVol / numSamples;

            var selecter = new ThreePSelecter(kz, iFrequency);

            int sampleCounter = 0;
            int iSampleCounter = 0;

            for (int i = 0; i < popSize; i++)
            {
                var result = selecter.Sample(aveKpi);
                
                if(result == SampleResult.M)
                { sampleCounter++; }
                else if (result == SampleResult.I)
                { iSampleCounter++; }
            }

            var sampleDiff = sampleCounter - numSamples;
            decimal sampleErr = (decimal)Math.Abs(sampleDiff) / (decimal)numSamples;

            //this.TestContext.WriteLine($"sampleDiff = {sampleDiff}");
            //this.TestContext.WriteLine($"sampleErr = {sampleErr}");

            var seenFreq = popSize / sampleCounter;
            //seenFreq.Should().BeInRange(freq - 1, freq + 1);

            sampleErr.Should().BeLessOrEqualTo(tolarance);

            if (iFrequency > 0)
            {
                var iSampleDiff = iSampleCounter - numIsamples;
                decimal iSampleErr = (decimal)Math.Abs(iSampleDiff) / (decimal)numIsamples;

                //this.TestContext.WriteLine($"iSampleDiff = {iSampleDiff}");
                //this.TestContext.WriteLine($"iSampleErr = {iSampleErr}");

                iSampleErr.Should().BeLessOrEqualTo(tolarance);
            }
            
        }
    }
}