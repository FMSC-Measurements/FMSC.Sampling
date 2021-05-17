using FluentAssertions;
using FMSC.Sampling;
using System;
using System.Linq;
using Xunit;

namespace Sampling.Test
{
    public class SystematicSelecterTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void TestFreqIsOne(int iFreq)
        {
            var freq = 1;
            var selecter = new SystematicSelecter(freq, iFreq, true);
            selecter.Invoking(x => x.Sample()).ShouldNotThrow();

            foreach (var i in Enumerable.Range(0, freq * 10))
            {
                selecter.Sample().Should().Be(SampleResult.M);
            }
        }

        [Theory]
        [InlineData(1, 0, false)]
        [InlineData(2, 0, false)]
        [InlineData(1, 0, true)]
        [InlineData(2, 0, true)]
        public void SystematicSelecter(int freq, int iFreq, bool randStart)
        {
            var selecter = new SystematicSelecter(freq, iFreq, randStart);
            selecter.Invoking(x => x.Sample()).ShouldNotThrow();

            foreach (var i in Enumerable.Range(0, freq * 10))
            {
                selecter.Invoking(x => x.Sample()).ShouldNotThrow();
            }
        }

        [Fact]
        public void TestSystmaticSelecter()
        {
            decimal tolarance = .01m;
            int freqency = 15;
            int iFrequency = 0;
            SystematicSelecter selecter = new SystematicSelecter(freqency, iFrequency, true);

            int numSamples = freqency * 10000;
            int[] results = new int[numSamples];
            int totalSamples = 0;
            int totalISamples = 0;

            for (int i = 0; i < numSamples; i++)
            {
                var result = selecter.Sample();

                if (result == SampleResult.M)
                {
                    results[i] = 1;
                    totalSamples++;
                }
                else if (result == SampleResult.I)
                {
                    results[i] = 2;
                    totalISamples++;
                }
                else
                {
                    results[i] = 0;
                }
            }

            selecter.ITreeFrequency.ShouldBeEquivalentTo(iFrequency);

            //this.TestContext.WriteLine(" numsamples  = {0}", numSamples.ToString());
            //this.TestContext.WriteLine("total samples  = {0}", totalSamples.ToString());
            //this.TestContext.WriteLine("total Isamples = {0}", totalISamples.ToString());

            decimal observedFreq = (totalSamples / (decimal)numSamples);
            decimal observediFreq = (totalISamples / (decimal)totalSamples);
            //this.TestContext.WriteLine("Observed freq = {0}", observedFreq.ToString());
            //this.TestContext.WriteLine("Observed iFreq = {0}", observediFreq.ToString());

            decimal dFreq = Math.Abs((1 / (decimal)freqency) - observedFreq);
            dFreq.Should().BeLessOrEqualTo(tolarance);

            if (iFrequency > 0)
            {
                decimal dIFreq = Math.Abs((1 / (decimal)iFrequency) - observediFreq);
                dIFreq.Should().BeLessOrEqualTo(tolarance);
            }
        }
    }
}