
using System.Globalization;

namespace UAVFLSecureSim.Benchmark
{
    internal static class BenchmarkCsvExporter
    {
        public static void Export(
            IReadOnlyList<BenchmarkResult> results,
            string filePath)
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
                "Scenario,RandomSeed,Accuracy,Precision,Recall," +
                "F1Score,LogLoss,SuccessfulCommunications," +
                "ExpectedCommunications,ExecutionTimeMilliseconds");

            foreach (BenchmarkResult result in results)
            {
                string scenarioName =
                    "\"" + result.ScenarioName.Replace("\"", "\"\"") + "\"";

                writer.WriteLine(
                    $"{scenarioName}," +
                    $"{result.RandomSeed}," +
                    $"{result.Accuracy.ToString(CultureInfo.InvariantCulture)}," +
                    $"{result.Precision.ToString(CultureInfo.InvariantCulture)}," +
                    $"{result.Recall.ToString(CultureInfo.InvariantCulture)}," +
                    $"{result.F1Score.ToString(CultureInfo.InvariantCulture)}," +
                    $"{result.LogLoss.ToString(CultureInfo.InvariantCulture)}," +
                    $"{result.SuccessfulCommunications}," +
                    $"{result.ExpectedCommunications}," +
                    $"{result.ExecutionTimeMilliseconds.ToString(CultureInfo.InvariantCulture)}");
            }
        }
    }
}
