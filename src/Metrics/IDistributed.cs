namespace Metrics
{
    public interface IDistributed
    {
        long Count { get; }
        double Max { get; }
        double Min { get; }
        double Mean { get; }
        double StdDev { get; }
        double[] Percentiles(params double[] percentiles);
    }
}