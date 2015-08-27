using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.Sampling
{
    public interface IFrequencyBasedSelecter
    {
        //Properties
        int Frequency { get; set; }
        bool IsSelectingITrees { get; }
        int ITreeFrequency { get; set; }
        SystematicCounter InsuranceCounter { get; set; }
        MersenneTwister Rand { get; }
    }
}
