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
        }

        public FrequencySelecter(int frequency, int iFrequency, int counter, int insuranceIndex, int insuranceCounter)
            : base(iFrequency, counter, insuranceIndex, insuranceCounter)
        {
            Frequency = frequency;
        }


        public abstract char Sample();

        /// <summary>
        /// Determins if the next tree is a sample
        /// </summary>
        /// <returns>true if the next tree is a sample</returns>
        [Obsolete]
        public bool Next()
        {
            var sampleResult = Sample();
            return sampleResult == 'M';
        }
    }
}