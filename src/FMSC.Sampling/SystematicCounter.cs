using System;

namespace FMSC.Sampling
{
    public class SystematicCounter
    {
        private int _counter;
        private int _hitIndex;
        private int _frequency;

        public enum CounterType { ON_FIRST, ON_LAST, ON_RANDOM }

        public int Frequency
        {
            get => _frequency;
            protected set => _frequency = (value >= 0) ? value : throw new ArgumentOutOfRangeException("Frequency", value, "");
        }

        public int Counter
        {
            get => _counter;
            protected set => _counter = (value >= 0) ? value : throw new ArgumentOutOfRangeException("Counter", value, "");
        }

        public int HitIndex
        {
            get => _hitIndex;
            protected set => _hitIndex = (value >= 0) ? value : throw new ArgumentOutOfRangeException("HitIndex", value, "");
        }

        //public SystematicCounter(int frequency, CounterType counterMethod)
        //    : this(frequency, counterMethod, null)
        //{ }

        public SystematicCounter(int frequency, CounterType counterMethod, Random rand)
        {


            if (counterMethod == CounterType.ON_RANDOM)
            {
                HitIndex = rand.Next(frequency - 1);
            }
            else if (counterMethod == CounterType.ON_LAST)
            { HitIndex = frequency - 1; }
            else
            { HitIndex = 0; }

            Frequency = frequency;
        }

        public SystematicCounter(int frequency, int hitIndex, int count)
        {
            HitIndex = hitIndex;
            Frequency = frequency;
            Counter = count;
        }

        public bool Next()
        {
            var counter = Counter;

            var index = counter % Frequency;
            var isSample = index == HitIndex;
            Counter = counter + 1;

            return isSample;
        }

        public bool Check()
        {
            if (this.Counter == this.HitIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}