using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    public class SystematicSelecter : SampleSelecter , IFrequencyBasedSelecter
    {
        private int _hitIndex;
        private int _currentIndex;
        private int _frequency;
        private int _iHitIndex;
        


        public SystematicSelecter() 
        {
            
        }

        public SystematicSelecter(int frequency): this(frequency, false)
        {

        }

        public SystematicSelecter(int frequency, bool randomStart) : this(frequency, -1, randomStart)
        {
        }

        public SystematicSelecter(int frequency, int iFrequency, bool randomStart)
        {
            this.Frequency = frequency;
            this.ITreeFrequency = iFrequency; 
            this._currentIndex = 0;
            if (randomStart)
            {
                this.HitIndex = Rand.Next(Frequency - 1);
            }
            else
            {
                this.HitIndex = 0;
            }
            if (base.IsSelectingITrees && this.Frequency != 1)
            {
                this._iHitIndex = Rand.Next(Frequency - 1);
                if (this._iHitIndex == this.HitIndex)
                {
                    this._iHitIndex = (this._iHitIndex + 1) % Frequency;
                }
                base.InsuranceCounter = new SystematicCounter(ITreeFrequency, SystematicCounter.CounterType.ON_RANDOM, this.Rand);
            }
        }



        [XmlAttribute]
        public int Frequency
        {
            get { return _frequency; }
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException("Frequency"); }
                _frequency = value;
            }
        }

        [XmlAttribute]
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
            }
        }

        [XmlAttribute]
        public int HitIndex 
        {
            get { return _hitIndex; }
            set
            {
                if (value < 0 || value > Frequency - 1) { throw new ArgumentOutOfRangeException("HitIndex"); }
                _hitIndex = value;
            }
        }

        [XmlAttribute]
        public int IHitIndex
        {
            get { return _iHitIndex; }
            set
            {
                if (value < 0 || value > Frequency - 1) { throw new ArgumentOutOfRangeException("HitIndex"); }
                _iHitIndex = value;
            }
        }


        private void IncrementIndex()
        {
            if (CurrentIndex == Frequency - 1)
            {
                CurrentIndex = 0;
                if (IsSelectingITrees)
                {
                    this.InsuranceCounter.Next();
                }
            }
            else
            {
                CurrentIndex++;
            }
            
        }

        public override SampleItem NextItem()
        {
            boolItem newItem = null;
            if (CurrentIndex == _hitIndex)
            {
                newItem = new boolItem();
                newItem.IsSelected = true;
            }
            else if (base.IsSelectingITrees && this.InsuranceCounter.Check() && CurrentIndex == _iHitIndex)
            {
                newItem = new boolItem();
                newItem.IsInsuranceItem = true;
            }

            IncrementIndex();
            this.Count++;
            return newItem; 
        }

        public override bool Ready(bool throwException)
        {
            return true;
        }

    }
}
