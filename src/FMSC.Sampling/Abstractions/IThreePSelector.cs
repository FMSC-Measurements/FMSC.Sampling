namespace FMSC.Sampling
{
    public interface IThreePSelector
    {
        char Sample(int kpi);

        char Sample(int kpi, out int rand);
    }
}