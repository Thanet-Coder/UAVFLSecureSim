
namespace UAVFLSecureSim.Detection
{
    internal class DetectionBenchmarkResult
    {
        public string ScenarioName { get; }

        public int RandomSeed { get; }

        public double DetectionThreshold { get; }

        public DetectionMetrics Metrics { get; }

        public DetectionBenchmarkResult(
            string scenarioName,
            int randomSeed,
            double detectionThreshold,
            DetectionMetrics metrics)
        {
            ScenarioName = scenarioName;
            RandomSeed = randomSeed;
            DetectionThreshold = detectionThreshold;
            Metrics = metrics;
        }
    }
}
