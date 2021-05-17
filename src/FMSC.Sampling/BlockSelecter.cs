using System;
using System.Linq;

namespace FMSC.Sampling
{
    public class BlockSelecter : FrequencySelecter
    {
        private const char SAMPLE_VALUE = 'x';
        private const char NONSAMPLE_VALUE = '-';
        private const int SAMPLES_PER_SUBBLOCK = 2; // this should never change, it is tightly tied to the sampling logic
        private const int NUM_SUBBLOCKS = 5; // this value could concevabley change but we wont change it.

        protected bool[] Block { get; set; }

        public string BlockState
        {
            get
            {
                var block = Block;
                if (block == null) return "";
                return new String(block.Select(x => (x) ? SAMPLE_VALUE : NONSAMPLE_VALUE).ToArray());
            }
            protected set
            {
                if (value != null)
                { Block = value.Select(x => char.ToLower(x) == SAMPLE_VALUE).ToArray(); }
                else { Block = null; }
            }
        }

        public BlockSelecter(int frequency,
            int iTreeFrequency)
            : base(frequency, iTreeFrequency)
        {
            Block = GenerateBlock(frequency, Rand);
        }

        public BlockSelecter(int frequency, int iTreeFrequency, string blockState, int counter, int insuranceIndex, int insuranceCounter)
            : base(frequency, iTreeFrequency, counter, insuranceIndex, insuranceCounter)
        {
            Frequency = frequency;

            if (blockState != null && blockState != "")
            {
                if (blockState.Length != CalcBlockSize(frequency)) throw new ArgumentException("blockstate length invalid");
                BlockState = blockState;
            }
            else
            {
                Block = GenerateBlock(frequency, Rand);
            }
        }

        public override SampleResult Sample()
        {
            lock (this)
            {
                var frequency = Frequency;
                var block = Block;

                var count = Count;
                var index = count % block.Length;

                var isInsuranceSample = false;
                var isSample = block[index];
                if (!isSample && IsSelectingITrees) // if tree IS  a sample and we are selecting insureance samples
                {
                    isInsuranceSample = InsuranceSampler.Next();
                }

                // update count and generate new block if needed
                count = count + 1;
                if (count == block.Length)
                {
                    Block = GenerateBlock(frequency, Rand);
                }
                Count = count;

                if (isInsuranceSample) { return SampleResult.I; }
                else if (isSample) { return SampleResult.M; }
                else { return SampleResult.C; }
            }
        }

        public static int CalcBlockSize(int frequency)
        {
            return frequency * SAMPLES_PER_SUBBLOCK * NUM_SUBBLOCKS;
        }

        public static bool[] GenerateBlock(int frequency, System.Random rand = null)
        {
            if (rand == null) { rand = MersenneTwister.Instance; }
            if (frequency <= 0) { throw new ArgumentOutOfRangeException(nameof(frequency)); }

            var blockSize = CalcBlockSize(frequency);

            var block = new bool[blockSize];

            // select subblock samples
            // we will select one sample for each subblock
            for (int i = 0; i < NUM_SUBBLOCKS; i++)
            {
                //calculate the range of the current subblock
                var subBlockStart = i * frequency * SAMPLES_PER_SUBBLOCK;
                var subBlockEnd = (i + 1) * frequency * SAMPLES_PER_SUBBLOCK;

                var sampleIndex = rand.Next(subBlockStart, subBlockEnd);
                block[sampleIndex] = true;
            }

            // select samples over the whole block
            // for x = the number of sub blocks we will select a sample from the whole block
            for (int i = 0; i < NUM_SUBBLOCKS; i++)
            {
                int sampleIndex = -1;
                //keep sampleing until we find a index without a sample
                //that you dont already have
                do
                {
                    sampleIndex = rand.Next(0, blockSize);
                } while (block[sampleIndex] == true);

                block[sampleIndex] = true;
            }

            return block;
        }
    }
}