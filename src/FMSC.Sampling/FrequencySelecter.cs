using System;

namespace FMSC.Sampling
{
    public abstract class FrequencySelecter : SampleSelecter, IFrequencyBasedSelecter
    {
        private int _frequency;

        public int Frequency
        {
            get
            {
                return this._frequency;
            }
            protected set
            {
                _frequency = (value > 0) ? value : throw new ArgumentOutOfRangeException("Frequency value ( " + value.ToString() + " out of range");
            }
        }

        public FrequencySelecter(int frequency, int iFrequency)
            : base(iFrequency)
        {
            Frequency = frequency;

            if (iFrequency > 1 && frequency > 1)
            {
                var ajustedIFrequency = (frequency - 1) * iFrequency;
                InsuranceSampler = new SystematicCounter(ajustedIFrequency, SystematicCounter.CounterType.ON_RANDOM, Rand);
            }
        }

        public FrequencySelecter(int frequency, int iFrequency, int counter, int insuranceIndex, int insuranceCounter)
            : base(iFrequency, counter)
        {
            Frequency = frequency;

            if (iFrequency > 1 && frequency > 1)
            {
                var ajustedIFrequency = (frequency - 1) * iFrequency;
                InsuranceSampler = new SystematicCounter(ajustedIFrequency, insuranceIndex, insuranceCounter);
            }
        }

        public abstract SampleResult Sample();
    }
}