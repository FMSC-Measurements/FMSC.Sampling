namespace FMSC.Sampling
{
    public class SystematicSelecter : FrequencySelecter, IFrequencyBasedSelecter
    {

        protected SystematicCounter SystematicCounter { get; }

        public SystematicSelecter(int frequency, int iFrequency, bool randomStart)
            : base(frequency, iFrequency)
        {
            SystematicCounter = new SystematicCounter(frequency, (randomStart) ? SystematicCounter.CounterType.ON_RANDOM : SystematicCounter.CounterType.ON_LAST, Rand);
        }

        public SystematicSelecter(int frequency, int iFrequency, int counter, int insuranceIndex, int insuranceCounter, int hitIndex)
            : base(frequency, iFrequency, counter, insuranceIndex, insuranceCounter)
        {
            SystematicCounter = new SystematicCounter(frequency, hitIndex, counter);
        }


        public override int Count
        {
            get { return SystematicCounter.Counter; }
        }

        public int HitIndex
        {
            get { return SystematicCounter.HitIndex; }
        }

        public override char Sample()
        {
            var isSample = SystematicCounter.Next();

            if (isSample)
            {
                if (IsSelectingITrees && InsuranceSampler.Next())
                {
                    return 'I';
                }
                else { return 'M'; }
            }
            else
            { return 'C'; }
        }
    }
}