using FluentAssertions;
using FMSC.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            BigInteger Factorial(long x)
            {
                return MathNet.Numerics.SpecialFunctions.Factorial(x);
            }

            BigInteger Combination(long a, long b)
            {
                if (a <= 1)
                    return 1;

                var divisor = (Factorial(b) * Factorial(a - b));

                return Factorial(a) / divisor;

            }

            double probOfFailures = 1 - probabilityOfSuccess;

            var c = Combination(trials, successes);
            double px = Math.Pow(probabilityOfSuccess, successes);
            double qnx = Math.Pow(probOfFailures, trials - successes);

            return (px * qnx) * (double)c;
        }

        [Fact]
        public void TestThreePSelecter()
        {
            double tolarance = .5;

            int iFrequency = 5;
            int aveKpi = 20;
            int freq = 500;
            int popSize = 10_000;
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

                if (result == SampleResult.M)
                { sampleCounter++; }
                else if (result == SampleResult.I)
                { iSampleCounter++; }
            }

            // taking the double sided binomial probability with the number of samples recieved against our frequency
            // will tell us the liklyhood of recieving the number of samples we recieved
            // check that probability against our tolarance

            var likelihood_twoSided = 2 * BinomialProbability(popSize, sampleCounter, 1 / (double)freq);
            likelihood_twoSided.Should().BeLessThan(tolarance);


            if (iFrequency > 1)
            {
                // because insurance trees should be generated for x number of 
                // regular samples. We should know exactly how many insurance samples 
                // to expect give or take one
                var actualExpectediSamples = sampleCounter / (double)(iFrequency);

                var iSampleDiff = Math.Abs(iSampleCounter - numIsamples);

                iSampleDiff.Should().BeLessOrEqualTo(1);
            }

        }
    }
}