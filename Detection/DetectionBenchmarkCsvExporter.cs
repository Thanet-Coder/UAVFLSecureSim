
using System.Globalization;

namespace UAVFLSecureSim.Detection
{
    internal static class DetectionBenchmarkCsvExporter
    {
        public static void Export(
            IReadOnlyList<DetectionBenchmarkResult> results,
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
                "Scenario,RandomSeed,DetectionThreshold," +
                "TotalEvaluated,TruePositives,FalsePositives," +
                "TrueNegatives,FalseNegatives,Precision,Recall," +
                "F1Score,FalsePositiveRate");

            foreach (DetectionBenchmarkResult result in results)
            {
                DetectionMetrics metrics = result.Metrics;

                string scenarioName =
                    "\"" +
                    result.ScenarioName.Replace("\"", "\"\"") +
                    "\"";

                writer.WriteLine(
                    $"{scenarioName}," +
                    $"{result.RandomSeed}," +
                    $"{result.DetectionThreshold.ToString(CultureInfo.InvariantCulture)}," +
                    $"{metrics.TotalEvaluated}," +
                    $"{metrics.TruePositives}," +
                    $"{metrics.FalsePositives}," +
                    $"{metrics.TrueNegatives}," +
                    $"{metrics.FalseNegatives}," +
                    $"{metrics.Precision.ToString(CultureInfo.InvariantCulture)}," +
                    $"{metrics.Recall.ToString(CultureInfo.InvariantCulture)}," +
                    $"{metrics.F1Score.ToString(CultureInfo.InvariantCulture)}," +
                    $"{metrics.FalsePositiveRate.ToString(CultureInfo.InvariantCulture)}");
            }
        }
    }
}
