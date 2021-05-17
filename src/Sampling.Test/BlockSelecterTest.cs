using FluentAssertions;
using FMSC.Sampling;
using System;
using System.Linq;
using Xunit;

namespace Sampling.Test
{
    public class BlockSelecterTest
    {
        const int BLOCK_SAMPLING_MARGIN_OF_ERROR = 5; // because a block has 5 samples that can be distributed anywhere, 
                                                      // a block could be front heavy 5 samples causing us to get 5 more samples that we expected in a hypethetical 
                                                      // worst case where we finish sampleing 6 (5 + 1) samples into a new block 





        [Theory]
        //[InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        public void TestBlockSelecter(int freqency, int iFrequency)
        {
            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            var sResult = selecter.Sample();
            sResult.Should().NotBeNull();
            selecter.BlockState.Should().NotBeEmpty();
        }

        [Theory]
        //[InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        public void BlockSelecter_Rehidrate(int freqency, int iFrequency)
        {
            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            var selecterAgian = new BlockSelecter(selecter.Frequency,
                selecter.ITreeFrequency,
                selecter.BlockState,
                selecter.Count,
                selecter.InsuranceIndex,
                selecter.InsuranceCounter);
        }

        [Theory]
        //[InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(3, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        public void BlockSelecter_Rehidrate_AfterSample(int freqency, int iFrequency)
        {
            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            selecter.Sample();

            var selecterAgian = new BlockSelecter(selecter.Frequency,
                selecter.ITreeFrequency,
                selecter.BlockState,
                selecter.Count,
                selecter.InsuranceIndex,
                selecter.InsuranceCounter);
        }

        [Theory]
        [InlineData(15, 0)]
        [InlineData(15, 2)]
        public void TestBlockSelector_2(int frequency, int iFrequency)
        {
            var block = new String(BlockSelecter.GenerateBlock(frequency).Select(x => x ? 'x' : '-').ToArray());

            BlockSelecter selecter = new BlockSelecter(frequency, iFrequency, block, 3, 0, 0);

            
        }

        [Theory]
        [InlineData(15, 0, 10_000)]
        [InlineData(15, 2, 10_000)]
        public void BlockSelector_Sample_Test(int freq, int iFreq, int numSamples)
        {
            BlockSelecter selecter = new BlockSelecter(freq, iFreq);

            ValidateBlockSelecter(selecter, freq, iFreq, numSamples);
        }

        protected void ValidateBlockSelecter(BlockSelecter selecter, int freq, int iFreq, int numSamples, int blockMarginOfError = BLOCK_SAMPLING_MARGIN_OF_ERROR)
        {
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

            selecter.ITreeFrequency.ShouldBeEquivalentTo(iFreq);

            //this.TestContext.WriteLine(" numsamples  = {0}", numSamples.ToString());
            //this.TestContext.WriteLine("total samples  = {0}", totalSamples.ToString());
            //this.TestContext.WriteLine("total Isamples = {0}", totalISamples.ToString());

            decimal observedFreq = (totalSamples / (decimal)numSamples);
            decimal observediFreq = (totalISamples / (decimal)numSamples);

            var expectedSamples = numSamples / freq;

            // if we are sampleing insurance trees then some regualr samples will get converted
            // into insurance samples, so we need to ajust expected samples to account for insuance trees
            var expectedITrees = (iFreq > 0) ? numSamples / (iFreq * freq) : 0;

            Math.Abs(expectedSamples - totalSamples)
                .Should().BeLessOrEqualTo(blockMarginOfError, $"expected samples:{expectedSamples} actual samples {totalSamples}"); // difference between actual samples and expected should be less than 1, allowing for rounding errors


            if (iFreq > 0)
            {
                var iSampleMarginOfError = Math.Max(blockMarginOfError / iFreq, 1);

                Math.Abs(expectedITrees - totalISamples)
                    .Should().BeLessOrEqualTo(iSampleMarginOfError, $"expected i samples:{expectedITrees} actual i samples:{totalISamples}");

            }
        }

        [Fact]
        // run the sampler so that number of samples lines up with 
        // block size so there is no margin for error
        public void BlockSelecter_Sample_exact_test()
        {
            int freqency = 15;
            int numSamples = freqency * 2 * 5 * 100;
            int iFrequency = 0;

            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            ValidateBlockSelecter(selecter, freqency, iFrequency, numSamples, 0);
        }

        [Fact]
        // run the sampler so that number of samples lines up with 
        // block size so there is no margin for error
        public void BlockSelecter_Sample_exact_with_iTrees()
        {
            int freqency = 15;
            int numSamples = freqency * 2 * 5 * 100;
            int iFrequency = 2;

            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            ValidateBlockSelecter(selecter, freqency, iFrequency, numSamples, 0);
        }

        [Theory]
        [InlineData(15)]
        public void GenerateBlock(int frequency)
        {
            var block = BlockSelecter.GenerateBlock(frequency);

            block.Should().HaveCount(2 * 5 * frequency);

            var numSamples = block.Where(x => x == true).Count();
            numSamples.Should().Be(10);
        }
    }
}