
namespace UAVFLSecureSim.Benchmark
{
    internal class BenchmarkTimingResult
    {
        public double MeanMilliseconds { get; }
        public double StandardDeviationMilliseconds { get; }
        public double MinimumMilliseconds { get; }
        public double MaximumMilliseconds { get; }
        public int Repetitions { get; }

        public BenchmarkTimingResult(
            double meanMilliseconds,
            double standardDeviationMilliseconds,
            double minimumMilliseconds,
            double maximumMilliseconds,
            int repetitions)
        {
            MeanMilliseconds = meanMilliseconds;
            StandardDeviationMilliseconds =
                standardDeviationMilliseconds;
            MinimumMilliseconds = minimumMilliseconds;
            MaximumMilliseconds = maximumMilliseconds;
            Repetitions = repetitions;
        }
    }
}
