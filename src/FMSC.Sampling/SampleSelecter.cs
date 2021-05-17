using System;

namespace FMSC.Sampling
{
    public abstract class SampleSelecter : ISampleSelector
    {
        #region fields

        protected MersenneTwister _rand = null;

        private int _iTreeFrequency;
        private int _count;

        #endregion fields

        #region Ctor

        protected SampleSelecter(int iTreeFrequency)
        {
            ITreeFrequency = iTreeFrequency;
        }

        protected SampleSelecter(int iTreeFrequency, int count)
        {
            Count = count;
            ITreeFrequency = iTreeFrequency;

        }

        #endregion Ctor

        #region properties

        public string StratumCode { get; set; }

        public string SampleGroupCode { get; set; }

        public int InsuranceCounter => InsuranceSampler?.Counter ?? 0;

        public int InsuranceIndex => InsuranceSampler?.HitIndex ?? 0;

        /// <summary>
        /// gets and sets ITreeFrequency. If value is not valid
        /// isSelectingITrees is set to false and ITreeFrequency to -1
        /// </summary>
        public int ITreeFrequency
        {
            get { return _iTreeFrequency; }
            protected set
            {
                _iTreeFrequency = (value >= 0) ? value : throw new ArgumentOutOfRangeException();
            }
        }

        protected SystematicCounter InsuranceSampler { get; set; }

        /// <summary>
        /// gets IsSelectingItrees. value is true if
        /// ITreeFrequency is not -1.
        /// </summary>
        public virtual bool IsSelectingITrees => ITreeFrequency > 1;

        public virtual int Count
        {
            get => _count;
            protected set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Count));
                _count = value;
            }
        }

        protected Random Rand
        {
            get { return _rand ?? MersenneTwister.Instance; }
        }

        #endregion properties
    }
}