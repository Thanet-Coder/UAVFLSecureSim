
using System.Diagnostics;

namespace UAVFLSecureSim.Benchmark
{
    internal static class BenchmarkTimingRunner
    {
        public static BenchmarkTimingResult Run(
            BenchmarkScenario scenario,
            int randomSeed = 42,
            int warmupRuns = 3,
            int timedRepetitions = 10)
        {
            if (warmupRuns < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(warmupRuns));
            }

            if (timedRepetitions < 2)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timedRepetitions),
                    "At least two timed repetitions are required.");
            }

            // ---------------------------------------------
            // Untimed warm-up experiments
            // ---------------------------------------------

            for (int i = 0; i < warmupRuns; i++)
            {
                BenchmarkRunner.Run(
                    scenario,
                    randomSeed: randomSeed);
            }

            // ---------------------------------------------
            // Timed experiments
            // ---------------------------------------------

            List<double> measurements =
                new List<double>();

            for (int i = 0; i < timedRepetitions; i++)
            {
                BenchmarkResult result =
                    BenchmarkRunner.Run(
                        scenario,
                        randomSeed: randomSeed);

                measurements.Add(
                    result.ExecutionTimeMilliseconds);
            }

            // ---------------------------------------------
            // Calculate timing statistics
            // ---------------------------------------------

            double mean =
                measurements.Average();

            double variance =
                measurements.Sum(
                    value => Math.Pow(value - mean, 2))
                / (measurements.Count - 1);

            double standardDeviation =
                Math.Sqrt(variance);

            return new BenchmarkTimingResult(
                mean,
                standardDeviation,
                measurements.Min(),
                measurements.Max(),
                measurements.Count);
        }
    }
}
