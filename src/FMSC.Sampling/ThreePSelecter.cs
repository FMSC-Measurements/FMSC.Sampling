using System;

namespace FMSC.Sampling
{
    public class ThreePSelecter : SampleSelecter, IThreePSelector
    {
        //Fields
        private int kz = -1;

        private int ajustedKZ = -1;

        protected int KZ
        {
            get
            {
                return this.kz;
            }
            set
            {
                this.kz = (value > 0) ? value : throw new ArgumentOutOfRangeException("KZ value (" + value.ToString() + ") is out of range");
            }
        }

        protected int AjustedKZ
        {
            get
            {
                return ajustedKZ;
            }
            set
            {
                ajustedKZ = (value > 0) ? value : -1;
            }
        }

        protected int WorkingKZ
        {
            get
            {
                if (ajustedKZ != -1)
                {
                    return ajustedKZ;
                }
                return kz;
            }
        }

        public ThreePSelecter(
            int kz,
            int iTreeFrequency)
            : base(iTreeFrequency)
        {
            KZ = kz;

            if (IsSelectingITrees)
            {
                ajustedKZ = CalcAjustedKZ(kz, iTreeFrequency);
                InsuranceSampler =
                    new SystematicCounter(base.ITreeFrequency + 1, // ajust i frequency because we were getting slightly too many i samples
                        SystematicCounter.CounterType.ON_RANDOM, Rand);
            }
        }

        public ThreePSelecter(
            int kz,
            int iTreeFrequency,
            int insuranceIndex, int insuranceCounter)
            : base(iTreeFrequency, 0, insuranceIndex, insuranceCounter)
        {
            KZ = kz;

            if (IsSelectingITrees)
            {
                var iFreq = ITreeFrequency;
                ajustedKZ = CalcAjustedKZ(kz, iFreq);
                InsuranceSampler =
                    new SystematicCounter(iFreq + 1, // ajust i frequency because we were getting slightly too many i samples
                        insuranceIndex, insuranceCounter);
            }
        }

        //methods
        private int CalcAjustedKZ(int kz, int iTreeFrequency)
        {
            double ajustedKZ;
            ajustedKZ = (double)kz / (1.0 + (1.0 / (double)iTreeFrequency));
            return (int)Math.Round(ajustedKZ);
        }

        public char Sample(int kpi)
        {
            return Sample(kpi, out var randomNumber);
        }

        public char Sample(int kpi, out int randomNumber)
        {
            randomNumber = Rand.Next(WorkingKZ);

            var isSample = kpi > randomNumber;

            if (isSample)
            {
                if (IsSelectingITrees && InsuranceSampler.Next())
                { return 'I'; }
                else
                { return 'M'; }
            }
            else
            { return 'C'; }
        }
    }
}