namespace FMSC.Sampling
{
    public interface ISampleSelector
    {
        int Count { get; }
        int ITreeFrequency { get; }
        bool IsSelectingITrees { get; }
        int InsuranceCounter { get; }
        int InsuranceIndex { get; }

        string StratumCode { get; set; }
        string SampleGroupCode { get; set; }
    }
}