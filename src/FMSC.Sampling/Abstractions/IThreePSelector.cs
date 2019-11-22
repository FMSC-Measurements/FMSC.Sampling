namespace FMSC.Sampling
{
    public interface IThreePSelector
    {
        SampleResult Sample(int kpi);

        SampleResult Sample(int kpi, out int rand);
    }
}