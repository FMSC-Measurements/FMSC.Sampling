namespace FMSC.Sampling
{
    public interface IFrequencyBasedSelecter : ISampleSelector
    {
        //Properties
        int Frequency { get; }

        SampleResult Sample();
    }
}