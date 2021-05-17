namespace FMSC.Sampling
{
    public class SRSSelecter : FrequencySelecter
    {
        public SRSSelecter(int frequency,
            int iTreeFrequency)
            : base(frequency, iTreeFrequency)
        { }

        public override SampleResult Sample()
        {
            var isSample = Rand.Next(Frequency) == 0;
            Count++;

            if (isSample)
            {
                if (IsSelectingITrees && InsuranceSampler.Next())
                { return SampleResult.I; }
                else { return SampleResult.M; }
            }
            else { return SampleResult.C; }
        }
    }
}