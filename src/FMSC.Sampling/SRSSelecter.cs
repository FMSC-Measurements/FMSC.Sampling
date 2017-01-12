using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    public class SRSSelecter : SampleSelecter
    {
        //fields
        private int frequency = -1;

        //private Utility.SystematicCounter insuranceCounter = null;

        //constructers
        public SRSSelecter()
            : base()
        { }

        public SRSSelecter(int frequency,
            int iTreeFrequency)
            : base(iTreeFrequency)
        {
            this.Frequency = frequency;

            if( base.IsSelectingITrees )
            {
                InsuranceCounter =
                    new SystematicCounter(Frequency * base.ITreeFrequency,
                        SystematicCounter.CounterType.ON_RANDOM, this.Rand);
            }
        }

        //properties

        [XmlAttribute]
        public int Frequency{
            get
            {
                return this.frequency;
            }
            set
            {
                this.frequency = (value > 0) ? value : -1;
            }
        }

        //[XmlElementAttribute(ElementName = "insuranceCounter",
        //    IsNullable = false, Type = typeof(Utility.SystematicCounter))]
        //public Utility.SystematicCounter InsuranceCounter
        //{
        //    get { return insuranceCounter; }
        //    set
        //    {
        //        if (insuranceCounter == null && value != null)
        //        {
        //            insuranceCounter = value;
        //        }
        //        else if (value == null)
        //        {
        //            throw new System.ArgumentNullException("can't set insuranceCounter to null");
        //        }
        //        else if (insuranceCounter != null)
        //        {
        //            throw new System.InvalidOperationException("can't reset insuranceCounter once it has been initialized");
        //        }
        //    }

        //}

        //methods

        public override SampleItem NextItem()
        {
            this.Ready(true);

            boolItem item = null;

            if (base.Rand.Next(Frequency) == 0)
            {
                item = new boolItem(this.Count);
            }
            else
            {
                if (IsSelectingITrees && InsuranceCounter.Next())
                {
                    item = new boolItem(Count);
                    item.IsInsuranceItem = true;
                }
            }

            base.Count++;

            return item;
        }

        public override bool Ready(bool throwException)
        {
            if (this.frequency == -1 )
            {
                if (throwException)
                {
                    throw new System.InvalidOperationException("invalid field in SRS");
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
    }
}