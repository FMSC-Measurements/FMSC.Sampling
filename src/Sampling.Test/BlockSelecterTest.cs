using FluentAssertions;
using FMSC.Sampling;
using System;
using Xunit;

namespace Sampling.Test
{
    public class BlockSelecterTest
    {
        [Fact]
        public void TestBlockSelecter()
        {
            decimal tolarance = .01m;
            int freqency = 15;
            int iFrequency = 0;
            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            int numSamples = freqency * 2 * 5 * 1000;
            int[] results = new int[numSamples];
            int totalSamples = 0;
            int totalISamples = 0;

            for (int i = 0; i < numSamples; i++)
            {
                SampleItem item = selecter.NextItem();

                if (item != null && item.IsInsuranceItem)
                {
                    results[i] = 2;
                    totalISamples++;
                }
                else if (item != null && item.IsSelected)
                {
                    results[i] = 1;
                    totalSamples++;
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

            decimal dFreq = Math.Abs((1 / (decimal)freqency) - observedFreq);

            dFreq.Should().BeLessOrEqualTo(tolarance, $"Observed freq = {observedFreq}; Observed iFreq = {observediFreq}; delta freq  = {dFreq}");

            if (iFrequency > 0)
            {
                decimal dIFreq = Math.Abs((1 / (decimal)iFrequency) - observediFreq);
                dIFreq.Should().BeLessOrEqualTo(tolarance, $"delta iFreq = {dIFreq}");
            }
        }
    }
}