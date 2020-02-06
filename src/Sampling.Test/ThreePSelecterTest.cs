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
        private double BinomialProbability(int trials, int successes,
                           double probabilityOfSuccess)
        {
            long Factorial(long x)
            {
                long result = 1;
                while (x != 1)
                {
                    result *= x;
                    x -= 1;
                }
                return result;
            }

            long Combination(long a, long b)
            {
                if (a <= 1)
                    return 1;

                return Factorial(a) / (Factorial(b) * Factorial(a - b));
            }

            double probOfFailures = 1 - probabilityOfSuccess;

            double c = Combination(trials, successes);
            double px = Math.Pow(probabilityOfSuccess, successes);
            double qnx = Math.Pow(probOfFailures, trials - successes);

            return c * px * qnx;
        }

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
                
                if(result == 'M')
                { sampleCounter++; }
                else if (result == 'I')
                { iSampleCounter++; }
            }

            var likelihood_twoSided = 2 * BinomialProbability(popSize, numIsamples, 1 / (double)freq);
            likelihood_twoSided.Should().BeLessThan(0.04);
            


            //sampleErr.Should().BeLessOrEqualTo(tolarance);

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