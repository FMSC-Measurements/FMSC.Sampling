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
                    new SystematicCounter(iTreeFrequency + 1, // ajust i frequency because we were getting slightly too many i samples
                        SystematicCounter.CounterType.ON_RANDOM, Rand);
            }
        }

        public ThreePSelecter(
            int kz,
            int iTreeFrequency,
            int counter,
            int insuranceIndex, int insuranceCounter)
            : base(iTreeFrequency, counter)
        {
            KZ = kz;

            if (IsSelectingITrees)
            {
                ajustedKZ = CalcAjustedKZ(kz, iTreeFrequency);
                InsuranceSampler =
                    new SystematicCounter(iTreeFrequency + 1, // ajust i frequency because we were getting slightly too many i samples
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

        public SampleResult Sample(int kpi)
        {
            return Sample(kpi, out _);
        }

        public SampleResult Sample(int kpi, out int randomNumber)
        {
            randomNumber = Rand.Next(WorkingKZ);

            var isSample = kpi > randomNumber;

            if (isSample)
            {
                if (IsSelectingITrees && InsuranceSampler.Next())
                { return SampleResult.I; }
                else
                { return SampleResult.M; }
            }
            else
            { return SampleResult.C; }
        }
    }
}