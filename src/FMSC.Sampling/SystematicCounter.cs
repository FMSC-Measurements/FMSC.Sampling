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
            protected set => _frequency = (value >= 0) ? value : throw new ArgumentOutOfRangeException("Frequency", $"Frequency value:{value} was out of range");
        }

        public int Counter
        {
            get => _counter;
            protected set => _counter = (value >= 0) ? value : throw new ArgumentOutOfRangeException("Counter", $"Counter value:{value} was out of range");
        }

        public int HitIndex
        {
            get => _hitIndex;
            protected set => _hitIndex = (value >= 0) ? value : throw new ArgumentOutOfRangeException("HitIndex", $"HitIndex value:{value} was out of range");
        }

        //public SystematicCounter(int frequency, CounterType counterMethod)
        //    : this(frequency, counterMethod, null)
        //{ }

        public SystematicCounter(int frequency, CounterType counterMethod, Random rand = null)
        {
            if(frequency <= 0) { throw new ArgumentOutOfRangeException(nameof(frequency)); }
            if (counterMethod == CounterType.ON_RANDOM)
            {
                if(rand == null) { throw new ArgumentNullException(nameof(rand)); }
                HitIndex = rand.Next(frequency);
            }
            else if (counterMethod == CounterType.ON_LAST)
            { HitIndex = frequency - 1; }
            else
            { HitIndex = 0; }

            Frequency = frequency;
        }

        public SystematicCounter(int frequency, int hitIndex, int count)
        {
            if (frequency <= 0) { throw new ArgumentOutOfRangeException(nameof(frequency)); }
            if (hitIndex < 0 || hitIndex >= frequency) { throw new ArgumentOutOfRangeException(nameof(hitIndex)); }
            if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count)); }

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