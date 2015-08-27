using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    //note: systematic counter is not zero baised by default. 
    //using provided paramiters it creates a range between 1 and RangeMax (or an optional RangeMin and RangeMax), inclusive
    //, so the number of elements is equal to RangeMax - (RangeMin - 1)
    [Serializable]
    public class SystematicCounter
    {
        public enum CounterType { ON_FIRST, ON_LAST, ON_RANDOM }
        public const uint DEFAULT_SEED = 4357;

        [XmlAttribute]
        public CounterType counterMethod
        {
            get;
            set;
        }

        [XmlAttribute]
        public int RangeMax
        {
            get;
            set;
        }

        [XmlAttribute]
        public int RangeMin
        {
            get;
            set;
        }

        [XmlAttribute]
        public int Counter
        {
            get;
            set;
        }

        [XmlAttribute]
        public int TrueValue
        {
            get;
            set;
        }

        [XmlIgnore]
        public MersenneTwister Rand { get; set; }

        protected SystematicCounter()
        {

        }

        //public SystematicCounter(int rangeMax, CounterType counterMethod)
        //    : this(1, rangeMax, counterMethod)
        //{
        //    //calls overloaded constructer
        //}


        public SystematicCounter(int rangeMax, CounterType counterMethod, MersenneTwister rand )
            : this(1, rangeMax, counterMethod, rand)
        {
            //calls overloaded constructer
        }


        //public SystematicCounter(int rangeMin, int rangeMax, CounterType counterMethod)
        //{
        //    this.counterMethod = counterMethod;
        //    this.RangeMin = rangeMin;
        //    this.RangeMax = rangeMax;
        //    this.Counter = -1;
        //    if (counterMethod == CounterType.ON_RANDOM)
        //    {
        //        this.Rand = new PseudoRandom.MersenneTwister(DEFAULT_SEED);
        //    }
        //    this.setTrueValue(counterMethod);
        //}


        public SystematicCounter(int rangeMin, int rangeMax, CounterType counterMethod, MersenneTwister rand)
        {
            this.counterMethod = counterMethod;
            this.RangeMin = rangeMin;
            this.RangeMax = rangeMax;
            this.Counter = -1;
            if (counterMethod == CounterType.ON_RANDOM)
            {
                this.Rand = rand;
            }
            this.setTrueValue(counterMethod);
        }


        public void reset()
        {
            this.Counter = -1;
            this.setTrueValue(this.counterMethod);
        }


        public bool Next()
        {
            this.Counter = ((this.Counter + 1) % (this.RangeMax + 1));
            if (this.Counter == 0)
            {
                this.Counter = this.RangeMin;
                this.setTrueValue(this.counterMethod);
            }

            return this.Check();
        }

        public bool Check()
        {
            if (this.Counter == this.TrueValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void setTrueValue(CounterType counterMethod)
        {
            switch (this.counterMethod)
            {
                case CounterType.ON_FIRST:
                    {
                        this.TrueValue = this.RangeMin;
                        break;
                    }
                case CounterType.ON_LAST:
                    {
                        this.TrueValue = this.RangeMax;
                        break;
                    }
                case CounterType.ON_RANDOM:
                    {
                        this.TrueValue = this.Rand.Next(this.RangeMin, this.RangeMax);
                        break;
                    }
                default:
                    {
                        throw new System.ArgumentException("invalid counterMethod");
                    }
            }
        }
    }
}
