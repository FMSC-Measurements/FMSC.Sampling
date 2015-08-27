using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.Sampling
{
    public interface IFrequencyBasedSelecter
    {
        //Properties
        //Decimal SelectionProbability { get; }
        int Frequency { get; set; }
        //Decimal AjustedProbability { get; set; }
        //Decimal OriginalProbability { get; }
        bool IsSelectingITrees { get; set; }
        int ITreeFrequency { get; set; }
        SystematicCounter InsuranceCounter { get; set; }
        MersenneTwister Rand { get; set; }


        //Methods
        //Decimal CalculateAjustedProbability();

    }

    //interface IThreePSelecter
    //{
    //    int KZ { get; set; }
    //    int maxKPI { get; set; }

    //}

    //interface IListSelecter
    //{

    //    public int CalculateAujustedNumSamples(); 
    //}
}
