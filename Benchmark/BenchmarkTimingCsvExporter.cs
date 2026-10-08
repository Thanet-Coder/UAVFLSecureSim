
using System.Globalization;

namespace UAVFLSecureSim.Benchmark
{
    internal static class BenchmarkTimingCsvExporter
    {
        public static void Export(
            IReadOnlyList<(
                BenchmarkScenario Scenario,
                BenchmarkTimingResult Timing)> results,
            string filePath,
            int warmupRuns)
        {
            string? directory =
                Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using StreamWriter writer =
                new StreamWriter(filePath, false);

            writer.WriteLine(
                "Scenario,WarmupRuns,TimedRepetitions," +
                "MeanMilliseconds,StandardDeviationMilliseconds," +
                "MinimumMilliseconds,MaximumMilliseconds");

            foreach (var result in results)
            {
                string scenarioName =
                    "\"" +
                    result.Scenario.Name.Replace("\"", "\"\"") +
                    "\"";

                BenchmarkTimingResult timing =
                    result.Timing;

                writer.WriteLine(
                    $"{scenarioName}," +
                    $"{warmupRuns}," +
                    $"{timing.Repetitions}," +
                    $"{timing.MeanMilliseconds.ToString(CultureInfo.InvariantCulture)}," +
                    $"{timing.StandardDeviationMilliseconds.ToString(CultureInfo.InvariantCulture)}," +
                    $"{timing.MinimumMilliseconds.ToString(CultureInfo.InvariantCulture)}," +
                    $"{timing.MaximumMilliseconds.ToString(CultureInfo.InvariantCulture)}");
            }
        }
    }
}
