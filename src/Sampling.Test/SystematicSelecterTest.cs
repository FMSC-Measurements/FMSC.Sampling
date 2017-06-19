﻿using FluentAssertions;
using FMSC.Sampling;
using System;
using Xunit;

namespace Sampling.Test
{
    public class SystematicSelecterTest
    {
        [Fact]
        public void TestFreqIsOne()
        {
            var selecter = new SystematicSelecter(1, 1, true);

            selecter.Invoking(x => x.NextItem()).ShouldNotThrow();
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