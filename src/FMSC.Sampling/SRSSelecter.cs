namespace FMSC.Sampling
{
    public class SRSSelecter : FrequencySelecter
    {
        public SRSSelecter(int frequency,
            int iTreeFrequency)
            : base(frequency, iTreeFrequency)
        { }

        public override char Sample()
        {
            var isSample = Rand.Next(Frequency) == 0;
            Count++;

            if (isSample)
            {
                if (IsSelectingITrees && InsuranceSampler.Next())
                { return 'I'; }
                else { return 'M'; }
            }
            else { return 'C'; }
        }
    }
}