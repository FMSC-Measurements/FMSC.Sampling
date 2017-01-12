using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    public class ThreePSelecter : SampleSelecter
    {
        //Fields
        private int kz = -1;

        private int maxKPI = -1;
        private int ajustedKZ = -1;

        //Constructers
        public ThreePSelecter()
            : base()
        { }

        public ThreePSelecter(
            int kz,
            int maxKPI,
            int iTreeFrequency)
            : base(iTreeFrequency)
        {
            this.KZ = kz;
            this.MaxKPI = maxKPI;

            if (base.IsSelectingITrees)
            {
                ajustedKZ = CalcAjustedKZ(kz, iTreeFrequency);
                InsuranceCounter =
                    new SystematicCounter(base.ITreeFrequency + 1,
                        SystematicCounter.CounterType.ON_FIRST, this.Rand);
            }
        }

        //methods
        private int CalcAjustedKZ(int kz, int iTreeFrequency)
        {
            double ajustedKZ;
            ajustedKZ = (double)kz / (1.0 + (1.0 / (double)iTreeFrequency));
            return (int)Math.Round(ajustedKZ);
        }

        public override SampleItem NextItem()
        {
            this.Ready(true);

            ThreePItem item;
            int randomNumber;
            randomNumber = base.Rand.Next(this.WorkingKZ);
            if (randomNumber < this.MaxKPI)
            {
                item = new ThreePItem(Count, randomNumber);
                Count++;
                return item;
            }
            else
            {
                Count++;
                return null;
            }
        }

        public override bool Ready(bool throwException)
        {
            if (this.KZ == -1 || this.MaxKPI == -1)
            {
                if (throwException)
                {
                    throw new System.InvalidOperationException("threeP is not ready");
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        //Properties
        [XmlAttribute]
        public int KZ
        {
            get
            {
                return this.kz;
            }
            set
            {
                this.kz = (value > 0) ? value : -1;
            }
        }

        [XmlAttribute]
        public int MaxKPI
        {
            get
            {
                return this.maxKPI;
            }
            set
            {
                this.maxKPI = (value > 0) ? value : -1;
            }
        }

        [XmlAttribute]
        public int AjustedKZ
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

        [XmlIgnore]
        public int WorkingKZ
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
    }
}